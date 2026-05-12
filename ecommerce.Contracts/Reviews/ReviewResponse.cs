namespace ecommerce.Contracts.Reviews;

public record ReviewResponse(
    int Id,
    int ProductId,
    int UserId,
    string UserName,
    int Rating,
    string? Comment,
    DateTime CreatedAt
);