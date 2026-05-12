namespace ecommerce.Contracts.Sellers;

public record MerchantResponse(
    int Id,
    int UserId,
    string StoreName,
    string? StoreLogo,
    string? Description,
    string Status,
    DateTime CreatedAt
);