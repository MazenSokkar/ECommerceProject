namespace ecommerce.Contracts.Payment;

public record PaymentResponse(
    int Id,
    int OrderId,
    string OrderNumber,
    string Status,
    string PaymentMethod,
    decimal Amount,
    string? TransactionId,
    DateTime CreatedAt
);