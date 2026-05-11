using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class CountryErrors
{
    public static readonly Error NotFound
        = new("Country.NotFound", "Country not found.", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedName
        = new("Country.DuplicatedName", "A country with this name already exists.", StatusCodes.Status409Conflict);
}
