using FluentValidation;

namespace ecommerce.Contracts.Payment;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    private static readonly string[] ValidMethods =
        [
            "CreditCard",
            "PayPal",
            "CashOnDelivery",
            "Wallet"
        ];
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("Order is required.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required.")
            .Must(m => ValidMethods.Contains(m))
            .WithMessage($"Payment method must be one of: {string.Join(", ", ValidMethods)}");

        RuleFor(x => x.TransactionId)
            .MaximumLength(200).WithMessage("Transaction ID cannot exceed 200 characters.")
            .When(x => x.TransactionId is not null);
    }
}