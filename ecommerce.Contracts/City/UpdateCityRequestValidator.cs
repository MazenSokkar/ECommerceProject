using FluentValidation;

namespace ecommerce.Contracts.City;

public class UpdateCityRequestValidator : AbstractValidator<UpdateCityRequest>
{
    public UpdateCityRequestValidator()
    {
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(256);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(256);
        RuleFor(x => x.CountryId).GreaterThan(0);
        RuleFor(x => x.StateProvinceId).GreaterThan(0);
    }
}
