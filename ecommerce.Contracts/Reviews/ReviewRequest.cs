namespace ecommerce.Contracts.Reviews;

public record CreateReviewRequest(
    int ProductId,
    int Rating,
    string? Comment
);