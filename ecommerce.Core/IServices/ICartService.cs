using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Cart;

namespace ecommerce.Core.IServices
{
    public interface ICartService
    {
        Task<Result<CartResponse>> GetCartAsync(int? userId, string? sessionId, CancellationToken cancellationToken = default);
        Task<Result<CartResponse>> AddItemAsync(int? userId, string? sessionId, AddToCartRequest request, CancellationToken cancellationToken = default);
        Task<Result<CartResponse>> UpdateItemAsync(int? userId, string? sessionId, int productId, UpdateCartItemRequest request, CancellationToken cancellationToken = default);
        Task<Result> RemoveItemAsync(int? userId, string? sessionId, int productId, CancellationToken cancellationToken = default);
        Task<Result> ClearCartAsync(int? userId, string? sessionId, CancellationToken cancellationToken = default);
    }
}
