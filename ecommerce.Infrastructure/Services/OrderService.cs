using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Order;
using ecommerce.Core.Entities;
using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Identity;

namespace ecommerce.Infrastructure.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IAddressRepository addressRepository,
    IProductRepository productRepository,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    UserManager<ApplicationUser> userManager
) : IOrderService
{
    public async Task<Result<OrderResponse>> PlaceOrderAsync(
        int userId,
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var cart = await cartRepository.FindByUserIdAsync(userId, cancellationToken);

        if (cart is null || !cart.Items.Any())
            return Result.Failure<OrderResponse>(OrderErrors.EmptyCart);

        var addresses = await addressRepository.GetByUserIdAsync(userId, cancellationToken);

        var address = addresses.FirstOrDefault(a => a.Id == request.ShippingAddressId);

        if (address is null || address.UserId != userId)
            return Result.Failure<OrderResponse>(OrderErrors.AddressNotFound);

        var orderItems = new List<OrderItem>();

        foreach (var item in cart.Items)
        {
            var product = await productRepository.FindByIdAsync(item.ProductId, cancellationToken);

            if (product is null || product.Stock < item.Quantity)
                return Result.Failure<OrderResponse>(OrderErrors.InsufficientStock);

            product.Stock -= item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                MerchantId = product.MerchantId,
                UnitPrice = item.UnitPrice
            });
        }

        var order = new Order
        {
            UserId = userId,
            OrderNumber = GenerateOrderNumber(),
            Status = OrderStatus.Pending,
            AddressId = request.ShippingAddressId,
            Total = orderItems.Sum(x => x.UnitPrice * x.Quantity),
            Items = orderItems,
            CreatedOn = DateTime.UtcNow
        };

        await orderRepository.AddAsync(order, cancellationToken);

        // تنظيف الكارت (safe way)
        foreach (var item in cart.Items.ToList())
        {
            cartRepository.RemoveItem(item);
        }

        cartRepository.RemoveCart(cart);

        await unitOfWork.Complete();

        // 🟢 Fetch user safely (NO navigation properties)
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user != null)
        {
            await emailService.SendEmailAsync(
                user.Email!,
                "Order Confirmation",
                "order-confirmation",
                new Dictionary<string, string>
                {
                    { "{{name}}", user.FirstName },
                    { "{{orderNumber}}", order.OrderNumber },
                    { "{{total}}", order.Total.ToString() }
                },
                cancellationToken
            );
        }

        var created = await orderRepository.FindByIdAsync(order.Id, cancellationToken);

        return Result.Success(MapToResponse(created!));
    }

    public async Task<Result<List<OrderSummaryResponse>>> GetMyOrdersAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.GetByUserIdAsync(userId, cancellationToken);

        var result = orders.Select(o => new OrderSummaryResponse(
            o.Id,
            o.OrderNumber,
            o.Status.ToString(),
            o.Total,
            o.CreatedOn,
            o.Items.Sum(i => i.Quantity)
        )).ToList();

        return Result.Success(result);
    }

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(
        int userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.FindByIdAsync(orderId, cancellationToken);

        if (order is null)
            return Result.Failure<OrderResponse>(OrderErrors.NotFound);

        if (order.UserId != userId)
            return Result.Failure<OrderResponse>(OrderErrors.Unauthorized);

        return Result.Success(MapToResponse(order));
    }

    public async Task<Result> UpdateOrderStatusAsync(
        int orderId,
        UpdateOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.FindByIdAsync(orderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
            return Result.Failure(OrderErrors.InvalidStatus);

        order.Status = status;
        order.UpdatedOn = DateTime.UtcNow;

        orderRepository.Update(order);
        await unitOfWork.Complete();

        return Result.Success();
    }

    public async Task<Result> CancelOrderAsync(
        int userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.FindByIdAsync(orderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (order.UserId != userId)
            return Result.Failure(OrderErrors.Unauthorized);

        if (order.Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.InvalidStatus);

        foreach (var item in order.Items)
        {
            var product = await productRepository.FindByIdAsync(item.ProductId, cancellationToken);
            if (product is not null)
                product.Stock += item.Quantity;
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedOn = DateTime.UtcNow;

        orderRepository.Update(order);
        await unitOfWork.Complete();

        return Result.Success();
    }

    private static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

    private static OrderResponse MapToResponse(Order order)
    {
        var items = order.Items.Select(i => new OrderItemResponse(
            i.Id,
            i.ProductId,
            i.Product.Name,
            i.Product.Images.FirstOrDefault(x => x.IsPrimary)?.ImageUrl,
            i.UnitPrice,
            i.Quantity,
            i.UnitPrice * i.Quantity
        )).ToList();

        var address = new OrderAddressResponse(
            order.Address?.LocationName ?? "",
            "",
            "",
            order.Address?.City?.NameEn ?? "",
            order.Address?.Country?.NameEn ?? ""
        );

        return new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.Status.ToString(),
            order.Total,
            order.Notes,
            order.CreatedOn,
            address,
            items
        );
    }

    public async Task<Result<List<OrderSummaryResponse>>> GetAllOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.GetAllAsync(cancellationToken);

        var result = orders.Select(o => new OrderSummaryResponse(
            o.Id,
            o.OrderNumber,
            o.Status.ToString(),
            o.Total,
            o.CreatedOn,
            o.Items.Sum(i => i.Quantity)
        )).ToList();

        return Result.Success(result);
    }

    public async Task<Result<OrderResponse>> GetOrderByIdAdminAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.FindByIdAsync(orderId, cancellationToken);

        if (order is null)
            return Result.Failure<OrderResponse>(OrderErrors.NotFound);

        return Result.Success(MapToResponse(order));
    }
}