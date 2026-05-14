using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ecommerce.Infrastructure.Repositories;

public class AuthRepository(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<AuthSettings> authSettings) : IAuthRepository
{
    private readonly string _otpProvider = authSettings.Value.OtpProvider;
    private readonly string _otpTokenName = authSettings.Value.OtpTokenName;

    public async Task<ApplicationUser?> FindByEmailAsync(string email) => await userManager.FindByEmailAsync(email);
    public async Task<ApplicationUser?> FindByPhoneAsync(string phone) => await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
    public async Task<ApplicationUser?> FindByIdAsync(string id) => await userManager.FindByIdAsync(id);
    public async Task<IEnumerable<ApplicationUser>> GetAllUsers() => await userManager.Users.ToListAsync();
    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role) => await userManager.GetUsersInRoleAsync(role);

    public async Task<SignInResult> CheckPasswordAsync(ApplicationUser user, string password, bool isPersistent, bool lockoutOnFailure)
        => await signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user) => await userManager.UpdateAsync(user);
    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password) => await userManager.CreateAsync(user, password);

    public async Task<bool> CheckEmailAvailabilityAsync(string email)
        => await userManager.Users.AnyAsync(u => u.Email == email);

    public async Task<bool> CheckPhoneAvailabilityAsync(string phone)
        => await userManager.Users.AnyAsync(u => u.PhoneNumber == phone);

    public async Task AssignRoleAsync(ApplicationUser user, string role)
        => await userManager.AddToRoleAsync(user, role);

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        => await userManager.GetRolesAsync(user);

    public async Task SetOtpAsync(ApplicationUser user, string otp, int expiryMinutes = 10)
    {
        var value = $"{otp}:{DateTime.UtcNow.AddMinutes(expiryMinutes).Ticks}";
        await userManager.SetAuthenticationTokenAsync(user, _otpProvider, _otpTokenName, value);
    }

    public async Task<(string? Otp, DateTime? Expiry)> GetOtpAsync(ApplicationUser user)
    {
        var value = await userManager.GetAuthenticationTokenAsync(user, _otpProvider, _otpTokenName);
        if (value is null) return (null, null);

        var parts = value.Split(':');
        if (parts.Length != 2 || !long.TryParse(parts[1], out var ticks))
            return (null, null);

        return (parts[0], DateTime.SpecifyKind(new DateTime(ticks), DateTimeKind.Utc));
    }

    public async Task RemoveOtpAsync(ApplicationUser user)
        => await userManager.RemoveAuthenticationTokenAsync(user, _otpProvider, _otpTokenName);

    public async Task<IdentityResult> ConfirmUserEmailAsync(ApplicationUser user)
    {
        user.EmailConfirmed = true;
        user.Active = true;
        return await userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword)
    {
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        return await userManager.ResetPasswordAsync(user, resetToken, newPassword);
    }
}
