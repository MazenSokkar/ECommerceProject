using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories
{
    public class CartRepository(AppDbContext context) : ICartRepository
    {
        public async Task<Cart?> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default)
            => await context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        public async Task<Cart?> FindBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
            => await context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId, cancellationToken);

        public async Task<Cart> GetOrCreateByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var cart = await FindByUserIdAsync(userId, cancellationToken);
            if (cart is not null) return cart;

            cart = new Cart { UserId = userId };
            await context.Carts.AddAsync(cart, cancellationToken);
            await context.SaveChangesAsync(cancellationToken); // ✅
            return cart;
        }
        public async Task<Cart> GetOrCreateBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            var cart = await FindBySessionIdAsync(sessionId, cancellationToken);
            if (cart is not null) return cart;

            cart = new Cart { SessionId = sessionId };
            await context.Carts.AddAsync(cart, cancellationToken);
            await context.SaveChangesAsync(cancellationToken); // ✅
            return cart;
        }

        public async Task<CartItem?> FindItemAsync(int cartId, int productId, CancellationToken cancellationToken = default)
            => await context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId, cancellationToken);

        public async Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
            => await context.Carts.AddAsync(cart, cancellationToken);

        public async Task AddItemAsync(CartItem item, CancellationToken cancellationToken = default)
            => await context.CartItems.AddAsync(item, cancellationToken);

        public void UpdateItem(CartItem item)
            => context.CartItems.Update(item);

        public void RemoveItem(CartItem item)
            => context.CartItems.Remove(item);

        public void RemoveCart(Cart cart)
            => context.Carts.Remove(cart);
    }
}
