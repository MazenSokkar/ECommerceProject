using System;
using System.Linq;
using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Users;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Infrastructure.Services;

public class AdminUserService(IAuthRepository authRepository) : IAdminUserService
{
    private readonly IAuthRepository _authRepository = authRepository;

    public async Task<Result<IEnumerable<AdminUserResponse>>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var users = await _authRepository.GetUsersInRoleAsync(DefaultRoles.Customer);

        var filtered = includeDeleted
            ? users
            : users.Where(u => !u.Deleted);

        var response = filtered.Select(MapUser).ToList();
        return Result.Success<IEnumerable<AdminUserResponse>>(response);
    }

    public async Task<Result<AdminUserResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(id.ToString());
        if (user is null || user.Deleted)
            return Result.Failure<AdminUserResponse>(UserErrors.UserNotFound);

        var roles = await _authRepository.GetRolesAsync(user);
        if (!roles.Contains(DefaultRoles.Customer))
            return Result.Failure<AdminUserResponse>(UserErrors.Forbidden);

        return Result.Success(MapUser(user));
    }

    public async Task<Result<AdminUserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _authRepository.CheckEmailAvailabilityAsync(request.Email))
            return Result.Failure<AdminUserResponse>(UserErrors.DuplicatedEmail);

        if (await _authRepository.CheckPhoneAvailabilityAsync(request.Phone))
            return Result.Failure<AdminUserResponse>(UserErrors.DuplicatedPhone);

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.Phone,
            Active = true,
            Deleted = false,
            EmailConfirmed = true
        };

        var createResult = await _authRepository.CreateUserAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var error = createResult.Errors.First();
            return Result.Failure<AdminUserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        await _authRepository.AssignRoleAsync(user, DefaultRoles.Customer);

        return Result.Success(MapUser(user));
    }

    public async Task<Result<AdminUserResponse>> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(id.ToString());
        if (user is null || user.Deleted)
            return Result.Failure<AdminUserResponse>(UserErrors.UserNotFound);

        var roles = await _authRepository.GetRolesAsync(user);
        if (!roles.Contains(DefaultRoles.Customer))
            return Result.Failure<AdminUserResponse>(UserErrors.Forbidden);

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _authRepository.CheckEmailAvailabilityAsync(request.Email))
                return Result.Failure<AdminUserResponse>(UserErrors.DuplicatedEmail);
            user.Email = request.Email;
            user.UserName = request.Email;
        }

        if (!string.Equals(user.PhoneNumber, request.Phone, StringComparison.OrdinalIgnoreCase))
        {
            if (await _authRepository.CheckPhoneAvailabilityAsync(request.Phone))
                return Result.Failure<AdminUserResponse>(UserErrors.DuplicatedPhone);
            user.PhoneNumber = request.Phone;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var updateResult = await _authRepository.UpdateUserAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure<AdminUserResponse>(UserErrors.UpdateFailed);

        return Result.Success(MapUser(user));
    }

    public async Task<Result> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(id.ToString());
        if (user is null || user.Deleted)
            return Result.Failure(UserErrors.UserNotFound);

        var roles = await _authRepository.GetRolesAsync(user);
        if (!roles.Contains(DefaultRoles.Customer))
            return Result.Failure(UserErrors.Forbidden);

        user.Deleted = true;
        user.Active = false;

        var updateResult = await _authRepository.UpdateUserAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure(UserErrors.UpdateFailed);

        return Result.Success();
    }

    private static AdminUserResponse MapUser(ApplicationUser user)
        => new(
            user.Id,
            user.FirstName,
            user.LastName,
            user.PhoneNumber ?? string.Empty,
            user.Email ?? string.Empty,
            user.Active,
            user.Deleted
        );
}
