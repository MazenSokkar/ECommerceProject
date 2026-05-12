namespace ecommerce.Contracts.Products;

public record ProductResponse(
    int Id,
    int MerchantId,
    string MerchantStoreName,
    int CategoryId,
    string CategoryName,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    bool IsActive,
    double AverageRating,
    int ReviewCount,
    List<string> Images,
    DateTime CreatedAt
);

public record ProductListItemResponse(
    int Id,
    string Name,
    decimal Price,
    int Stock,
    bool IsActive,
    string? PrimaryImageUrl,
    double AverageRating,
    int ReviewCount,
    string CategoryName,
    string MerchantStoreName
);
public record ProductListResponse(
    IEnumerable<ProductListItemResponse> Items,
    int Total
);