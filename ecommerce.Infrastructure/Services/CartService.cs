using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Cart;
using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;

namespace ecommerce.Infrastructure.Services
{
    public class CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICartService
    {
        public async Task<Result<CartResponse>> GetCartAsync(int? userId, string? sessionId, CancellationToken cancellationToken = default)
        {
            var cart = userId.HasValue
                ? await cartRepository.FindByUserIdAsync(userId.Value, cancellationToken)
                : sessionId is not null
                    ? await cartRepository.FindBySessionIdAsync(sessionId, cancellationToken)
                    : null;

            if (cart is null)
                return Result.Success(new CartResponse(0, [], 0, 0));

            return Result.Success(MapToResponse(cart));
        }

        public async Task<Result<CartResponse>> AddItemAsync(int? userId, string? sessionId, AddToCartRequest request, CancellationToken cancellationToken = default)
        {
            var product = await productRepository.FindByIdAsync(request.ProductId, cancellationToken);
            if (product is null)
                return Result.Failure<CartResponse>(CartErrors.ProductNotFound);

            if (product.Stock < request.Quantity)
                return Result.Failure<CartResponse>(CartErrors.InsufficientStock);

            var cart = userId.HasValue
                ? await cartRepository.GetOrCreateByUserIdAsync(userId.Value, cancellationToken)
                : await cartRepository.GetOrCreateBySessionIdAsync(sessionId!, cancellationToken);

            var existingItem = await cartRepository.FindItemAsync(cart.Id, request.ProductId, cancellationToken);

            if (existingItem is not null)
            {
                var newQty = existingItem.Quantity + request.Quantity;
                if (product.Stock < newQty)
                    return Result.Failure<CartResponse>(CartErrors.InsufficientStock);

                existingItem.Quantity = newQty;
                existingItem.UnitPrice = product.Price;
                cartRepository.UpdateItem(existingItem);
            }
            else
            {
                var item = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price
                };
                await cartRepository.AddItemAsync(item, cancellationToken);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.Complete();

            var updated = await cartRepository.FindByUserIdAsync(cart.UserId ?? 0, cancellationToken)
                          ?? await cartRepository.FindBySessionIdAsync(cart.SessionId ?? "", cancellationToken);

            return Result.Success(MapToResponse(updated!));
        }

        public async Task<Result<CartResponse>> UpdateItemAsync(int? userId, string? sessionId, int productId, UpdateCartItemRequest request, CancellationToken cancellationToken = default)
        {
            var cart = userId.HasValue
                ? await cartRepository.FindByUserIdAsync(userId.Value, cancellationToken)
                : sessionId is not null
                    ? await cartRepository.FindBySessionIdAsync(sessionId, cancellationToken)
                    : null;

            if (cart is null)
                return Result.Failure<CartResponse>(CartErrors.NotFound);

            var item = await cartRepository.FindItemAsync(cart.Id, productId, cancellationToken);
            if (item is null)
                return Result.Failure<CartResponse>(CartErrors.ItemNotFound);

            var product = await productRepository.FindByIdAsync(productId, cancellationToken);
            if (product is null)
                return Result.Failure<CartResponse>(CartErrors.ProductNotFound);

            if (product.Stock < request.Quantity)
                return Result.Failure<CartResponse>(CartErrors.InsufficientStock);

            item.Quantity = request.Quantity;
            item.UnitPrice = product.Price;
            cartRepository.UpdateItem(item);

            cart.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.Complete();

            return Result.Success(MapToResponse(cart));
        }

        public async Task<Result> RemoveItemAsync(int? userId, string? sessionId, int productId, CancellationToken cancellationToken = default)
        {
            var cart = userId.HasValue
                ? await cartRepository.FindByUserIdAsync(userId.Value, cancellationToken)
                : sessionId is not null
                    ? await cartRepository.FindBySessionIdAsync(sessionId, cancellationToken)
                    : null;

            if (cart is null)
                return Result.Failure(CartErrors.NotFound);

            var item = await cartRepository.FindItemAsync(cart.Id, productId, cancellationToken);
            if (item is null)
                return Result.Failure(CartErrors.ItemNotFound);

            cartRepository.RemoveItem(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.Complete();

            return Result.Success();
        }

        public async Task<Result> ClearCartAsync(int? userId, string? sessionId, CancellationToken cancellationToken = default)
        {
            var cart = userId.HasValue
                ? await cartRepository.FindByUserIdAsync(userId.Value, cancellationToken)
                : sessionId is not null
                    ? await cartRepository.FindBySessionIdAsync(sessionId, cancellationToken)
                    : null;

            if (cart is null)
                return Result.Failure(CartErrors.NotFound);

            cartRepository.RemoveCart(cart);
            await unitOfWork.Complete();

            return Result.Success();
        }

        private static CartResponse MapToResponse(Cart cart)
        {
            var items = cart.Items.Select(i => new CartItemResponse(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl,
                i.UnitPrice,
                i.Quantity,
                i.UnitPrice * i.Quantity
            )).ToList();

            var total = items.Sum(i => i.Subtotal);
            var totalItems = items.Sum(i => i.Quantity);

            return new CartResponse(cart.Id, items, total, totalItems);
        }
    }
}
