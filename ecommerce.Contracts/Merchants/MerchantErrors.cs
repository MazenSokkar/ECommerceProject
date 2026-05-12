using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class MerchantErrors
{
    public static readonly Error NotFound =
        new("Merchant.NotFound", "Merchant not found.", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyRegistered =
        new("Merchant.AlreadyRegistered", "You are already registered as a Merchant.", StatusCodes.Status409Conflict);

    public static readonly Error NotApproved =
        new("Merchant.NotApproved", "Your Merchant account is not approved yet.", StatusCodes.Status403Forbidden);

    public static readonly Error InvalidStatus =
        new("Merchant.InvalidStatus", "Invalid status value.", StatusCodes.Status400BadRequest);
}
