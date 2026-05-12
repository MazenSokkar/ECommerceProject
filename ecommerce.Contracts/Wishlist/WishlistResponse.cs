namespace ecommerce.Contracts.Wishlists;

public record WishlistResponse(
    int Id,
    List<WishlistItemResponse> Items
);

public record WishlistItemResponse(
    int Id,
    int ProductId,
    string ProductName,
    decimal ProductPrice,
    string? ProductImage,
    DateTime AddedAt
);