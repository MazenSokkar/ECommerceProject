using ecommerce.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace ecommerce.Core.IRepositories;

public interface IAuthRepository
{
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByPhoneAsync(string phone);
    Task<ApplicationUser?> FindByIdAsync(string id);
    Task<IEnumerable<ApplicationUser>> GetAllUsers();
    Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role);
    Task<SignInResult> CheckPasswordAsync(ApplicationUser user, string password, bool isPersistent, bool lockoutOnFailure);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<bool> CheckEmailAvailabilityAsync(string email);
    Task<bool> CheckPhoneAvailabilityAsync(string phone);
    Task AssignRoleAsync(ApplicationUser user, string role);
    Task RemoveRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task SetOtpAsync(ApplicationUser user, string otp, int expiryMinutes = 10);
    Task<(string? Otp, DateTime? Expiry)> GetOtpAsync(ApplicationUser user);
    Task RemoveOtpAsync(ApplicationUser user);
    Task<IdentityResult> ConfirmUserEmailAsync(ApplicationUser user);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword);
}
