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

public class AdminUserService(
    IAuthRepository authRepository,
    IAddressRepository addressRepository,
    ICountryRepository countryRepository,
    IStateProvinceRepository stateProvinceRepository,
    ICityRepository cityRepository,
    IMerchantRepository merchantRepository,
    IUnitOfWork unitOfWork) : IAdminUserService
{
    private readonly IAuthRepository _authRepository = authRepository;
    private readonly IAddressRepository _addressRepository = addressRepository;
    private readonly ICountryRepository _countryRepository = countryRepository;
    private readonly IStateProvinceRepository _stateProvinceRepository = stateProvinceRepository;
    private readonly ICityRepository _cityRepository = cityRepository;
    private readonly IMerchantRepository _merchantRepository = merchantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<AdminUsersByRoleResponse>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var admins = await _authRepository.GetUsersInRoleAsync(DefaultRoles.Admin);
        var merchants = await _authRepository.GetUsersInRoleAsync(DefaultRoles.Merchant);
        var customers = await _authRepository.GetUsersInRoleAsync(DefaultRoles.Customer);

        var response = new AdminUsersByRoleResponse(
            FilterUsers(admins, includeDeleted),
            FilterUsers(merchants, includeDeleted),
            FilterUsers(customers, includeDeleted)
        );

        return Result.Success(response);
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

        var addressResult = await ValidateAndUpsertUserAddressAsync(user.Id, request.Address, cancellationToken);
        if (addressResult.IsFailure)
            return Result.Failure<AdminUserResponse>(addressResult.Error);

        await _unitOfWork.Complete();

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

        var addressResult = await ValidateAndUpsertUserAddressAsync(user.Id, request.Address, cancellationToken);
        if (addressResult.IsFailure)
            return Result.Failure<AdminUserResponse>(addressResult.Error);

        await _unitOfWork.Complete();

        return Result.Success(MapUser(user));
    }

    public async Task<Result<AdminUserResponse>> ToggleActiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(id.ToString());
        if (user is null || user.Deleted)
            return Result.Failure<AdminUserResponse>(UserErrors.UserNotFound);

        var roles = await _authRepository.GetRolesAsync(user);
        if (!roles.Contains(DefaultRoles.Customer))
            return Result.Failure<AdminUserResponse>(UserErrors.Forbidden);

        user.Active = !user.Active;

        var updateResult = await _authRepository.UpdateUserAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure<AdminUserResponse>(UserErrors.UpdateFailed);

        if (!user.Active)
        {
            var merchant = await _merchantRepository.FindByUserIdAsync(user.Id, cancellationToken);
            if (merchant is not null && !merchant.Deleted && merchant.Active)
            {
                merchant.Active = false;
                _merchantRepository.Update(merchant);
                await _unitOfWork.Complete();
            }
        }

        return Result.Success(MapUser(user));
    }

    public async Task<Result> ToggleDeletedAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(id.ToString());
        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        var roles = await _authRepository.GetRolesAsync(user);
        if (!roles.Contains(DefaultRoles.Customer))
            return Result.Failure(UserErrors.Forbidden);

        if (user.Deleted)
        {
            // --- Restore ---
            user.Deleted = false;
            user.Active = true;

            var restoreResult = await _authRepository.UpdateUserAsync(user);
            if (!restoreResult.Succeeded)
                return Result.Failure(UserErrors.UpdateFailed);

            return Result.Success();
        }
        else
        {
            // --- Soft-delete ---
            user.Deleted = true;
            user.Active = false;

            var deleteResult = await _authRepository.UpdateUserAsync(user);
            if (!deleteResult.Succeeded)
                return Result.Failure(UserErrors.UpdateFailed);

            var merchant = await _merchantRepository.FindByUserIdAsync(user.Id, cancellationToken);
            if (merchant is not null && !merchant.Deleted)
            {
                merchant.Deleted = true;
                merchant.Active = false;
                _merchantRepository.Update(merchant);
                await _unitOfWork.Complete();
            }

            return Result.Success();
        }
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

    private static IEnumerable<AdminUserResponse> FilterUsers(IEnumerable<ApplicationUser> users, bool includeDeleted)
    {
        var filtered = includeDeleted
            ? users
            : users.Where(u => !u.Deleted);

        return filtered.Select(MapUser).ToList();
    }

    private async Task<Result> ValidateAndUpsertUserAddressAsync(int userId, ecommerce.Contracts.Auth.RegisterAddressRequest address, CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByIdAsync(address.CountryId, cancellationToken);
        if (country is null)
            return Result.Failure(CountryErrors.NotFound);

        var stateProvince = await _stateProvinceRepository.GetByIdAsync(address.StateProvinceId, cancellationToken);
        if (stateProvince is null)
            return Result.Failure(StateProvinceErrors.NotFound);

        var city = await _cityRepository.GetByIdAsync(address.CityId, cancellationToken);
        if (city is null)
            return Result.Failure(CityErrors.NotFound);

        var upsert = new Address
        {
            LocationName = address.LocationName,
            CityId = address.CityId,
            StateProvinceId = address.StateProvinceId,
            CountryId = address.CountryId,
            Longitude = address.Longitude ?? string.Empty,
            Latitude = address.Latitude ?? string.Empty
        };

        await _addressRepository.UpsertUserAddressAsync(userId, upsert, cancellationToken);
        return Result.Success();
    }
}
