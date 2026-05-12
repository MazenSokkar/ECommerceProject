using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class WishlistErrors
{
    public static readonly Error ProductNotFound =
        new("Wishlist.ProductNotFound", "Product not found.", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyAdded =
        new("Wishlist.AlreadyAdded", "Product is already in your wishlist.", StatusCodes.Status409Conflict);

    public static readonly Error ItemNotFound =
        new("Wishlist.ItemNotFound", "Item not found in wishlist.", StatusCodes.Status404NotFound);
}