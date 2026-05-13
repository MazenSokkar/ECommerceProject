using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Order;

namespace ecommerce.Core.IServices;

public interface IOrderService
{
    Task<Result<OrderResponse>> PlaceOrderAsync(int userId, PlaceOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<OrderSummaryResponse>>> GetMyOrdersAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> GetOrderByIdAsync(int userId, int orderId, CancellationToken cancellationToken = default);
    Task<Result> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result> CancelOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderSummaryResponse>>> GetAllOrdersAsync(CancellationToken cancellationToken = default);

    Task<Result<OrderResponse>> GetOrderByIdAdminAsync(int orderId, CancellationToken cancellationToken = default);
}