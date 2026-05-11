using FluentValidation;

namespace ecommerce.Contracts.StateProvince;

public class UpdateStateProvinceRequestValidator : AbstractValidator<UpdateStateProvinceRequest>
{
    public UpdateStateProvinceRequestValidator()
    {
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(256);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(256);
        RuleFor(x => x.CountryId).GreaterThan(0);
    }
}
