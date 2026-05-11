namespace ecommerce.Contracts.Auth;

public record RegisterRequest
(
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    string Password,
    RegisterAddressRequest Address
);
