using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Reviews;

namespace ecommerce.Core.IServices;

public interface IReviewService
{
    Task<Result<IEnumerable<ReviewResponse>>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<Result<ReviewResponse>> CreateAsync(int userId, CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int userId, int id, CancellationToken cancellationToken = default);
}