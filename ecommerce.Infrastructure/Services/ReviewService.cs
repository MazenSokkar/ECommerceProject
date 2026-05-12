using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Reviews;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;

namespace ecommerce.Infrastructure.Services;

public class ReviewService(
    IReviewRepository repository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IReviewService
{
    public async Task<Result<IEnumerable<ReviewResponse>>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var reviews = await repository.GetByProductIdAsync(productId, cancellationToken);

        var response = reviews.Select(r => new ReviewResponse(
            r.Id,
            r.ProductId,
            r.UserId,
            $"{r.User.FirstName} {r.User.LastName}",
            r.Rating,
            r.Comment,
            r.CreatedOn
        ));

        return Result.Success(response);
    }

    public async Task<Result<ReviewResponse>> CreateAsync(int userId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<ReviewResponse>(ReviewErrors.ProductNotFound);

        var exists = await repository.ExistsByUserAndProductAsync(userId, request.ProductId, cancellationToken);
        if (exists)
            return Result.Failure<ReviewResponse>(ReviewErrors.AlreadyReviewed);

        var review = new Review
        {
            UserId = userId,
            ProductId = request.ProductId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        await repository.AddAsync(review, cancellationToken);
        await unitOfWork.Complete();

        var created = await repository.FindByIdAsync(review.Id, cancellationToken);

        return Result.Success(new ReviewResponse(
            created!.Id,
            created.ProductId,
            created.UserId,
            $"{created.User.FirstName} {created.User.LastName}",
            created.Rating,
            created.Comment,
            created.CreatedOn
        ));
    }

    public async Task<Result> DeleteAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        var review = await repository.FindByIdAsync(id, cancellationToken);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        if (review.UserId != userId)
            return Result.Failure(ReviewErrors.NotOwner);

        repository.Delete(review);
        await unitOfWork.Complete();

        return Result.Success();
    }
}