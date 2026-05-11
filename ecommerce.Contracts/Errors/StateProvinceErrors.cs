using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class StateProvinceErrors
{
    public static readonly Error NotFound
        = new("StateProvince.NotFound", "State/province not found.", StatusCodes.Status404NotFound);
}
