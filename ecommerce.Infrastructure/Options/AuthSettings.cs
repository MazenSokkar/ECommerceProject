namespace ecommerce.Infrastructure.Options;

public class AuthSettings
{
    public string OtpProvider { get; init; } = "GiftWave";
    public string OtpTokenName { get; init; } = "EmailOTP";
    public int OtpExpiryMinutes { get; init; } = 10;
    public int MaxLoginAttempts { get; init; } = 5;
    public int LockoutMinutes { get; init; } = 15;
    public int SessionRevokedCacheDays { get; init; } = 30;
}
