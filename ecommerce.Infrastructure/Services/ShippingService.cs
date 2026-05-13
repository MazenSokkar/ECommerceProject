using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Shipping;
using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;

namespace ecommerce.Infrastructure.Services;

public class ShippingService(IShippingRepository shippingRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService) : IShippingService
{
    public async Task<Result> CreateShipmentAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.FindByIdAsync(orderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        var existing = await shippingRepository.GetByOrderIdAsync(orderId, cancellationToken);

        if (existing is not null)
            return Result.Failure(OrderErrors.InvalidStatus);

        var shipping = new Shipping
        {
            OrderId = orderId,
            TrackingNumber = $"TRK-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Carrier = "Default Carrier",
            Status = ShippingStatus.Preparing
        };

        await shippingRepository.AddAsync(shipping, cancellationToken);

        order.Status = OrderStatus.Shipped;
        order.UpdatedOn = DateTime.UtcNow;

        orderRepository.Update(order);

        await unitOfWork.Complete();

        try
        {
            await emailService.SendEmailAsync(
                order.User.Email,
                "Your Order is Shipped",
                "shipping-update",
                new Dictionary<string, string>
                {
                    { "{{orderNumber}}", order.OrderNumber },
                    { "{{status}}", "Shipped" },
                    { "{{tracking}}", shipping.TrackingNumber ?? "" }
                },
                cancellationToken
            );
        }
        catch { }

        return Result.Success();
    }

    public async Task<Result> UpdateStatusAsync(
        int orderId,
        ShippingStatus status,
        CancellationToken cancellationToken = default)
    {
        var shipping = await shippingRepository.GetByOrderIdAsync(orderId, cancellationToken);

        if (shipping is null)
            return Result.Failure(OrderErrors.NotFound);

        var order = await orderRepository.FindByIdAsync(orderId, cancellationToken);

        shipping.Status = status;

        if (status == ShippingStatus.Shipped)
            shipping.ShippedAt = DateTime.UtcNow;

        if (status == ShippingStatus.Delivered)
            shipping.DeliveredAt = DateTime.UtcNow;

        shippingRepository.Update(shipping);

        if (order is not null)
        {
            order.Status = status == ShippingStatus.Delivered
                ? OrderStatus.Delivered
                : OrderStatus.Shipped;

            order.UpdatedOn = DateTime.UtcNow;

            orderRepository.Update(order);

            try
            {
                await emailService.SendEmailAsync(
                    order.User.Email,
                    "Shipping Update",
                    "shipping-update",
                    new Dictionary<string, string>
                    {
                        { "{{orderNumber}}", order.OrderNumber },
                        { "{{status}}", status.ToString() },
                        { "{{tracking}}", shipping.TrackingNumber ?? "" }
                    },
                    cancellationToken
                );
            }
            catch { }
        }

        await unitOfWork.Complete();

        return Result.Success();
    }

    public async Task<Result<ShippingResponse>> GetByOrderIdAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var shipping = await shippingRepository.GetByOrderIdAsync(orderId, cancellationToken);

        if (shipping is null)
            return Result.Failure<ShippingResponse>(OrderErrors.NotFound);

        return Result.Success(new ShippingResponse(
            shipping.Id,
            shipping.OrderId,
            shipping.TrackingNumber,
            shipping.Carrier,
            shipping.Status.ToString(),
            shipping.ShippedAt,
            shipping.DeliveredAt
        ));
    }
}