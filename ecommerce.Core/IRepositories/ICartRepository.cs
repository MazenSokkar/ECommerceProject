using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Core.Entities.salah_entities;

namespace ecommerce.Core.IRepositories
{

    public interface ICartRepository
    {
        Task<Cart?> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Cart?> FindBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
        Task<Cart> GetOrCreateByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Cart> GetOrCreateBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
        Task<CartItem?> FindItemAsync(int cartId, int productId, CancellationToken cancellationToken = default);
        Task AddAsync(Cart cart, CancellationToken cancellationToken = default);
        Task AddItemAsync(CartItem item, CancellationToken cancellationToken = default);
        void UpdateItem(CartItem item);
        void RemoveItem(CartItem item);
        void RemoveCart(Cart cart);
    }
}
