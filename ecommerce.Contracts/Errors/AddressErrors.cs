using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class AddressErrors
{
    public static readonly Error NotFound
        = new("Address.NotFound", "Address not found.", StatusCodes.Status404NotFound);
}
