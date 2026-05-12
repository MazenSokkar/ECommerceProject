using FluentValidation;

namespace ecommerce.Contracts.Sellers;

public class CreateMerchantRequestValidator : AbstractValidator<CreateMerchantRequest>
{
    public CreateMerchantRequestValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Store name is required.")
            .MaximumLength(150).WithMessage("Store name must not exceed 150 characters.");
    }
}

public class UpdateSellerRequestValidator : AbstractValidator<UpdateMerchantRequest>
{
    public UpdateSellerRequestValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Store name is required.")
            .MaximumLength(150).WithMessage("Store name must not exceed 150 characters.");
    }
}

public class UpdateSellerStatusRequestValidator : AbstractValidator<UpdateMerchantStatusRequest>
{
    private static readonly string[] ValidStatuses = ["approved", "rejected", "suspended"];

    public UpdateSellerStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s => ValidStatuses.Contains(s.ToLower()))
            .WithMessage("Status must be approved, rejected, or suspended.");
    }
}
