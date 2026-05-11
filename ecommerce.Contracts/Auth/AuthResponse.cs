namespace ecommerce.Contracts.Auth;

public record AuthResponse(
    string FirstName,
    string LastName,
    string Email,
    IEnumerable<string> Roles,
    string Token,
    int ExpiresIn
);
