namespace ecommerce.Contracts.Payment;

public record CreatePaymentRequest(
    int OrderId,
    string PaymentMethod,
    string? TransactionId
);