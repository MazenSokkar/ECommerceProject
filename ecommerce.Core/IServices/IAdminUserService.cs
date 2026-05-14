using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Users;

namespace ecommerce.Core.IServices;

public interface IAdminUserService
{
    Task<Result<AdminUsersByRoleResponse>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<Result<AdminUserResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<AdminUserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<AdminUserResponse>> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<AdminUserResponse>> ToggleActiveAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
