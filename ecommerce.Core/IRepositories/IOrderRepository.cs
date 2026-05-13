using ecommerce.Core.Entities.salah_entities;

namespace ecommerce.Core.IRepositories;

public interface IOrderRepository
{
    Task<Order?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Order?> FindByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    void Update(Order order);
}