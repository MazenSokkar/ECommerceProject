using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Cart
{

    public static class CartErrors
    {
        public static readonly Error NotFound =
            new("Cart.NotFound", "Cart not found.", StatusCodes.Status404NotFound);

        public static readonly Error ItemNotFound =
            new("Cart.ItemNotFound", "Item not found in cart.", StatusCodes.Status404NotFound);

        public static readonly Error ProductNotFound =
            new("Cart.ProductNotFound", "Product not found.", StatusCodes.Status404NotFound);

        public static readonly Error InsufficientStock =
            new("Cart.InsufficientStock", "Insufficient stock for this product.", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidQuantity =
            new("Cart.InvalidQuantity", "Quantity must be greater than zero.", StatusCodes.Status400BadRequest);
    }
}
