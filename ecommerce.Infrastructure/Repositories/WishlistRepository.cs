using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class WishlistRepository(AppDbContext context) : IWishlistRepository
{
    public async Task<Wishlist?> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await context.Wishlists
            .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

    public async Task<Wishlist> GetOrCreateAsync(int userId, CancellationToken cancellationToken = default)
    {
        var wishlist = await FindByUserIdAsync(userId, cancellationToken);
        if (wishlist is not null) return wishlist;

        wishlist = new Wishlist { UserId = userId };
        await context.Wishlists.AddAsync(wishlist, cancellationToken);
        return wishlist;
    }

    public async Task<WishlistItem?> FindItemAsync(int wishlistId, int productId, CancellationToken cancellationToken = default)
        => await context.WishlistItems
            .FirstOrDefaultAsync(i => i.WishlistId == wishlistId && i.ProductId == productId, cancellationToken);

    public async Task AddItemAsync(WishlistItem item, CancellationToken cancellationToken = default)
        => await context.WishlistItems.AddAsync(item, cancellationToken);

    public void RemoveItem(WishlistItem item)
        => context.WishlistItems.Remove(item);
}