using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class PaymentErrors
{
    public static readonly Error NotFound =
        new("Payment.NotFound", "Payment not found.", StatusCodes.Status404NotFound);

    public static readonly Error OrderNotFound =
        new("Payment.OrderNotFound", "Order not found.", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyPaid =
        new("Payment.AlreadyPaid", "This order has already been paid.", StatusCodes.Status400BadRequest);

    public static readonly Error Failed =
        new("Payment.Failed", "Payment processing failed.", StatusCodes.Status400BadRequest);

    public static readonly Error Unauthorized =
        new("Payment.Unauthorized", "You are not authorized to access this payment.", StatusCodes.Status403Forbidden);

    public static readonly Error InvalidAmount =
        new("Payment.InvalidAmount", "Payment amount does not match order total.", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidPaymentMethod =
    new("Payment.InvalidMethod", "Invalid payment method.", StatusCodes.Status400BadRequest);
}