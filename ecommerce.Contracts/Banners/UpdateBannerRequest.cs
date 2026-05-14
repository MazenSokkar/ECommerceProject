using System.ComponentModel.DataAnnotations;

namespace ecommerce.Contracts.Banners;

public record UpdateBannerRequest(
    [Required, MaxLength(100)] string Title,
    [MaxLength(1000)] string? Content,
    [Required, MaxLength(500)] string ImageUrl,
    [MaxLength(500)] string? LinkUrl,
    bool IsActive,
    int DisplayOrder
);
