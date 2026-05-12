using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class ReviewErrors
{
    public static readonly Error NotFound =
        new("Review.NotFound", "Review not found.", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyReviewed =
        new("Review.AlreadyReviewed", "You have already reviewed this product.", StatusCodes.Status409Conflict);

    public static readonly Error ProductNotFound =
        new("Review.ProductNotFound", "Product not found.", StatusCodes.Status404NotFound);

    public static readonly Error NotOwner =
        new("Review.NotOwner", "You are not the owner of this review.", StatusCodes.Status403Forbidden);
}