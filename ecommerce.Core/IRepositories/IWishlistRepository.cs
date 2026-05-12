using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories;

public interface IWishlistRepository
{
    Task<Wishlist?> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Wishlist> GetOrCreateAsync(int userId, CancellationToken cancellationToken = default);
    Task<WishlistItem?> FindItemAsync(int wishlistId, int productId, CancellationToken cancellationToken = default);
    Task AddItemAsync(WishlistItem item, CancellationToken cancellationToken = default);
    void RemoveItem(WishlistItem item);
}