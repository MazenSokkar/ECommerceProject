using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Payment;

namespace ecommerce.Core.IServices;

public interface IPaymentService
{
    Task<Result<PaymentResponse>> CreatePaymentAsync(int userId, CreatePaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentResponse>> GetPaymentByOrderIdAsync(int userId, int orderId, CancellationToken cancellationToken = default);
    Task<Result<List<PaymentResponse>>> GetMyPaymentsAsync(int userId, CancellationToken cancellationToken = default);
}