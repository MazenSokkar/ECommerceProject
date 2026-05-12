namespace ecommerce.Contracts.Categories;


public record CategoryResponse(
    int Id,
    string Name,
    string Slug,
    string? ImageUrl,
    int? ParentId,
    List<CategoryResponse> Children
);