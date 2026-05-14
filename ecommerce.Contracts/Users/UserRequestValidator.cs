using ecommerce.Contracts.Abstractions.Constants;
using FluentValidation;

namespace ecommerce.Contracts.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
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
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

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
    }
}
