using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class OrderErrors
{
    public static readonly Error NotFound =
        new("Order.NotFound", "Order not found.", StatusCodes.Status404NotFound);

    public static readonly Error EmptyCart =
        new("Order.EmptyCart", "Cannot place order with an empty cart.", StatusCodes.Status400BadRequest);

    public static readonly Error InsufficientStock =
        new("Order.InsufficientStock", "One or more items have insufficient stock.", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidStatus =
        new("Order.InvalidStatus", "Invalid order status transition.", StatusCodes.Status400BadRequest);

    public static readonly Error Unauthorized =
        new("Order.Unauthorized", "You are not authorized to access this order.", StatusCodes.Status403Forbidden);

    public static readonly Error AddressNotFound =
        new("Order.AddressNotFound", "Shipping address not found.", StatusCodes.Status404NotFound);
}