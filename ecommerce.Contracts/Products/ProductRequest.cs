namespace ecommerce.Contracts.Products;

public record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int CategoryId
);

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int CategoryId,
    bool IsActive
);

public record UpdateStockRequest(
    int Stock
);