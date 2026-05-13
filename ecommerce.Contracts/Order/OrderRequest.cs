namespace ecommerce.Contracts.Order;

public record PlaceOrderRequest(
    int ShippingAddressId,
    string? Notes
);

public record UpdateOrderStatusRequest(
    string Status
);