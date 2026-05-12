using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound =
        new("Product.NotFound", "Product not found.", StatusCodes.Status404NotFound);

    public static readonly Error NotOwner =
        new("Product.NotOwner", "You are not the owner of this product.", StatusCodes.Status403Forbidden);

    public static readonly Error CategoryNotFound =
        new("Product.CategoryNotFound", "Category not found.", StatusCodes.Status404NotFound);

    public static readonly Error MerchantNotFound =
        new("Product.MerchantNotFound", "Merchant profile not found.", StatusCodes.Status404NotFound);

    public static readonly Error MerchantNotApproved =
        new("Product.MerchantNotApproved", "Your merchant account is not approved yet.", StatusCodes.Status403Forbidden);
}