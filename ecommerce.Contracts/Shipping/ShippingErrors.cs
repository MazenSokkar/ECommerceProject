using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;


namespace ecommerce.Contracts.Shipping
{
    public static class ShippingErrors
    {
        public static readonly Error NotFound =
            new("Shipping.NotFound", "Shipping not found.", StatusCodes.Status404NotFound);

        public static readonly Error AlreadyExists =
            new("Shipping.AlreadyExists", "Shipping already exists for this order.", StatusCodes.Status400BadRequest);
    }
}
