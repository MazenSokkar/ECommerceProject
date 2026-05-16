namespace ecommerce.Contracts.Products;

public record AddProductImageRequest(
    string ImageUrl,
    bool IsPrimary,
    int SortOrder
);