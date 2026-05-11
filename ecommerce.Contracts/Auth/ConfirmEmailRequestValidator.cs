using FluentValidation;

namespace ecommerce.Contracts.Auth;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(6, 6)
            .Matches(@"^\d{6}$")
            .WithMessage("Code must be a 6-digit number");
    }
}
