using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Wishlists;

namespace ecommerce.Core.IServices;

public interface IWishlistService
{
    Task<Result<WishlistResponse>> GetMyWishlistAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<WishlistResponse>> AddItemAsync(int userId, AddToWishlistRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveItemAsync(int userId, int productId, CancellationToken cancellationToken = default);
}