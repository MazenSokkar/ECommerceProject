using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Users;

namespace ecommerce.Core.IServices;

public interface IUserProfileService
{
    Task<Result<UserProfileResponse>> GetAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<UserProfileResponse>> UpdateAsync(int userId, UpdateUserProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int userId, CancellationToken cancellationToken = default);
}