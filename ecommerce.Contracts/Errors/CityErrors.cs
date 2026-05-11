using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class CityErrors
{
    public static readonly Error NotFound
        = new("City.NotFound", "City not found.", StatusCodes.Status404NotFound);
}
