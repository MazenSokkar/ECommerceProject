using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class ReviewRepository(AppDbContext context) : IReviewRepository
{
    public async Task<Review?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        => await context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken = default)
        => await context.Reviews
            .AnyAsync(r => r.UserId == userId && r.ProductId == productId, cancellationToken);

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
        => await context.Reviews.AddAsync(review, cancellationToken);

    public void Delete(Review review)
        => review.Deleted = true;
}