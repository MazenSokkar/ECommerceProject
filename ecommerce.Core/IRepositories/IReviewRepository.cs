using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories;

public interface IReviewRepository
{
    Task<Review?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    void Delete(Review review);
}