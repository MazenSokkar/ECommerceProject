using FluentValidation;

namespace ecommerce.Contracts.Address;

public class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressRequestValidator()
    {
        RuleFor(x => x.LocationName).NotEmpty().MaximumLength(512);
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.StateProvinceId).GreaterThan(0);
        RuleFor(x => x.CountryId).GreaterThan(0);
        RuleFor(x => x.Longitude).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Latitude).NotEmpty().MaximumLength(50);
    }
}
