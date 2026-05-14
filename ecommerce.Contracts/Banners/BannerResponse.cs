namespace ecommerce.Contracts.Banners;

public record BannerResponse(
    int Id,
    string Title,
    string? Content,
    string ImageUrl,
    string LinkUrl,
    bool IsActive,
    int DisplayOrder
);
