using System;
using FluentValidation;
using ecommerce.Contracts.Abstractions.Constants;
namespace ecommerce.Contracts.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("password must have special chars, lower case, upper case and at least 8 digits");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+20(10|11|12|15)\d{8}$")
            .WithMessage("Phone must be a valid Egyptian number (e.g. +201012345678)");

        RuleFor(x => x.Address)
            .NotNull()
            .SetValidator(new RegisterAddressRequestValidator());
    }
}