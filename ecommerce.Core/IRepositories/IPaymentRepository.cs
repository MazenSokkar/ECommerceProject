using ecommerce.Core.Entities.salah_entities;

namespace ecommerce.Core.IRepositories;

public interface IPaymentRepository
{
    Task<Payment?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Payment?> FindByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    void Update(Payment payment);
}