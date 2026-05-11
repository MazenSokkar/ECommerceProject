namespace ecommerce.Contracts.Auth;

public record ConfirmEmailRequest(string Email, string Code);
