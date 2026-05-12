namespace ecommerce.Contracts.Categories;

public record CreateCategoryRequest(
    string Name,
    int? ParentId,
    string? ImageUrl
);

public record UpdateCategoryRequest(
    string Name,
    int? ParentId,
    string? ImageUrl
);