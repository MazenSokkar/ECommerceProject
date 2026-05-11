using FluentValidation;

namespace ecommerce.Contracts.Auth;

public class RegisterAddressRequestValidator : AbstractValidator<RegisterAddressRequest>
{
    public RegisterAddressRequestValidator()
    {
        RuleFor(x => x.LocationName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.StateProvinceId).GreaterThan(0);
        RuleFor(x => x.CountryId).GreaterThan(0);
        When(x => x.Longitude is not null, () => RuleFor(x => x.Longitude).NotEmpty());
        When(x => x.Latitude is not null, () => RuleFor(x => x.Latitude).NotEmpty());
    }
}
