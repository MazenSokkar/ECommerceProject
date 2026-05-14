namespace ecommerce.Contracts.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    string Password
);

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Phone,
    string Email
);
