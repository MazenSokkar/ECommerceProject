namespace ecommerce.Contracts.Auth;

public record ResetPasswordRequest(string Email, string Code, string NewPassword);
