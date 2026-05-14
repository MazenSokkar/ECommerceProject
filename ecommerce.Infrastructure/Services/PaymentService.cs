using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Payment;
using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using ecommerce.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Stripe;
using PaymentMethod = ecommerce.Core.Entities.salah_entities.PaymentMethod;

namespace ecommerce.Infrastructure.Services;

public class PaymentService(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IOptions<StripeOptions> stripeOptions) : IPaymentService
{
    private readonly StripeOptions _stripe = stripeOptions.Value;

    public async Task<Result<PaymentResponse>> CreatePaymentAsync(
        int userId,
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Get Order
        var order = await orderRepository.FindByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure<PaymentResponse>(PaymentErrors.OrderNotFound);

        if (order.UserId != userId)
            return Result.Failure<PaymentResponse>(PaymentErrors.Unauthorized);

        // 2. Prevent double payment
        var existing = await paymentRepository.FindByOrderIdAsync(request.OrderId, cancellationToken);

        if (existing is not null && existing.Status == PaymentStatus.Completed)
            return Result.Failure<PaymentResponse>(PaymentErrors.AlreadyPaid);

        // 3. Parse Payment Method
        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var method))
            return Result.Failure<PaymentResponse>(PaymentErrors.InvalidPaymentMethod);

        // 4. Create Payment
        var payment = new Payment
        {
            OrderId = request.OrderId,
            Method = method,
            TransactionId = request.TransactionId,
            Amount = order.Total,
            Status = PaymentStatus.Completed,
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await paymentRepository.AddAsync(payment, cancellationToken);

        // 5. Update Order
        order.Status = OrderStatus.Confirmed;
        order.UpdatedOn = DateTime.UtcNow;

        orderRepository.Update(order);

        await unitOfWork.Complete();

        // 6. Email Notification
        try
        {
            await emailService.SendEmailAsync(
                order.User.Email,
                "Payment Successful 🎉",
                "payment-success",
                new Dictionary<string, string>
                {
                    { "{{name}}", order.User.FirstName },
                    { "{{orderNumber}}", order.OrderNumber },
                    { "{{amount}}", order.Total.ToString() },
                    { "{{status}}", "Paid" }
                },
                cancellationToken
            );
        }
        catch
        {
            // ignore email failure
        }

        return Result.Success(MapToResponse(payment, order.OrderNumber));
    }

    public async Task<Result<PaymentResponse>> GetPaymentByOrderIdAsync(
        int userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.FindByOrderIdAsync(orderId, cancellationToken);

        if (payment is null)
            return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

        if (payment.Order.UserId != userId)
            return Result.Failure<PaymentResponse>(PaymentErrors.Unauthorized);

        return Result.Success(MapToResponse(payment, payment.Order.OrderNumber));
    }

    public async Task<Result<List<PaymentResponse>>> GetMyPaymentsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var payments = await paymentRepository.GetByUserIdAsync(userId, cancellationToken);

        var result = payments
            .Select(p => MapToResponse(p, p.Order.OrderNumber))
            .ToList();

        return Result.Success(result);
    }

    private static PaymentResponse MapToResponse(Payment payment, string orderNumber)
        => new(
            payment.Id,
            payment.OrderId,
            orderNumber,
            payment.Status.ToString(),
            payment.Method.ToString(),
            payment.Amount,
            payment.TransactionId,
            payment.CreatedAt
        );



    public async Task<Result<StripeIntentResponse>> CreateStripeIntentAsync(
    int userId,
    int orderId,
    CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.FindByIdAsync(orderId, cancellationToken);

        if (order is null)
            return Result.Failure<StripeIntentResponse>(PaymentErrors.OrderNotFound);

        if (order.UserId != userId)
            return Result.Failure<StripeIntentResponse>(PaymentErrors.Unauthorized);

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(order.Total * 100),
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            Metadata = new Dictionary<string, string>
        {
            { "orderId", orderId.ToString() }
        }
        };

        var service = new Stripe.PaymentIntentService();
        var intent = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return Result.Success(new StripeIntentResponse(intent.ClientSecret, _stripe.PublishableKey));
    }


}