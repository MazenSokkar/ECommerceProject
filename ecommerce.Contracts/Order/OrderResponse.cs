namespace ecommerce.Contracts.Order;

public record OrderResponse(
    int Id,
    string OrderNumber,
    string Status,
    decimal TotalAmount,
    string? Notes,
    DateTime CreatedAt,
    OrderAddressResponse ShippingAddress,
    List<OrderItemResponse> Items
);

public record OrderItemResponse(
    int Id,
    int ProductId,
    string ProductName,
    string? ProductImage,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);

public record OrderAddressResponse(
    string FullName,
    string Phone,
    string AddressLine,
    string City,
    string Country
);

public record OrderSummaryResponse(
    int Id,
    string OrderNumber,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    int ItemsCount
);