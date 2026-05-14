using ecommerce.Contracts.Auth;

namespace ecommerce.Contracts.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    string Password,
    RegisterAddressRequest Address
);

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    RegisterAddressRequest Address
);

public record UpdateUserProfileRequest(
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    RegisterAddressRequest Address
);
