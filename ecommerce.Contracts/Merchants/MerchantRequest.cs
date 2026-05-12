namespace ecommerce.Contracts.Sellers;

public record CreateMerchantRequest(
    string StoreName,
    string? Description,
    string? StoreLogo
);

public record UpdateMerchantRequest(
    string StoreName,
    string? Description,
    string? StoreLogo
);

public record UpdateMerchantStatusRequest(
    string Status  
);