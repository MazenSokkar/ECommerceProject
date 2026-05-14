namespace ecommerce.Contracts.Users;

public record AdminUserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    bool Active,
    bool Deleted
);
