using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Wishlists;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;

namespace ecommerce.Infrastructure.Services;

public class WishlistService(
    IWishlistRepository repository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IWishlistService
{
    public async Task<Result<WishlistResponse>> GetMyWishlistAsync(int userId, CancellationToken cancellationToken = default)
    {
        var wishlist = await repository.GetOrCreateAsync(userId, cancellationToken);
        await unitOfWork.Complete();
        return Result.Success(MapToResponse(wishlist));
    }

    public async Task<Result<WishlistResponse>> AddItemAsync(int userId, AddToWishlistRequest request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<WishlistResponse>(WishlistErrors.ProductNotFound);

        var wishlist = await repository.GetOrCreateAsync(userId, cancellationToken);

        var existingItem = await repository.FindItemAsync(wishlist.Id, request.ProductId, cancellationToken);
        if (existingItem is not null)
            return Result.Failure<WishlistResponse>(WishlistErrors.AlreadyAdded);

        var item = new WishlistItem
        {
            WishlistId = wishlist.Id,
            ProductId = request.ProductId
        };

        await repository.AddItemAsync(item, cancellationToken);
        await unitOfWork.Complete();

        var updated = await repository.FindByUserIdAsync(userId, cancellationToken);
        return Result.Success(MapToResponse(updated!));
    }

    public async Task<Result> RemoveItemAsync(int userId, int productId, CancellationToken cancellationToken = default)
    {
        var wishlist = await repository.FindByUserIdAsync(userId, cancellationToken);
        if (wishlist is null)
            return Result.Failure(WishlistErrors.ItemNotFound);

        var item = await repository.FindItemAsync(wishlist.Id, productId, cancellationToken);
        if (item is null)
            return Result.Failure(WishlistErrors.ItemNotFound);

        repository.RemoveItem(item);
        await unitOfWork.Complete();

        return Result.Success();
    }

    private static WishlistResponse MapToResponse(Wishlist wishlist) =>
        new(
            wishlist.Id,
            wishlist.Items.Select(i => new WishlistItemResponse(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.Price,
                i.Product.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl,
                i.AddedAt
            )).ToList()
        );
}