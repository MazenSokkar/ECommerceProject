using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class BannerErrors
{
    public static readonly Error NotFound = new("Banner.NotFound", "Banner not found.", StatusCodes.Status404NotFound);
}
