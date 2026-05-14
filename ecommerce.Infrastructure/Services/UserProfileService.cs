using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Users;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using MapsterMapper;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Infrastructure.Services;

public class UserProfileService(
    IAuthRepository authRepository,
    IAddressRepository addressRepository,
    ICountryRepository countryRepository,
    IStateProvinceRepository stateProvinceRepository,
    ICityRepository cityRepository,
    IMerchantRepository merchantRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IUserProfileService
{
    private readonly IAuthRepository _authRepository = authRepository;
    private readonly IAddressRepository _addressRepository = addressRepository;
    private readonly ICountryRepository _countryRepository = countryRepository;
    private readonly IStateProvinceRepository _stateProvinceRepository = stateProvinceRepository;
    private readonly ICityRepository _cityRepository = cityRepository;
    private readonly IMerchantRepository _merchantRepository = merchantRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<UserProfileResponse>> GetAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(userId.ToString());
        if (user is null || user.Deleted)
            return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

        var addresses = await _addressRepository.GetByUserIdAsync(userId, cancellationToken);
        var response = new UserProfileResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.PhoneNumber ?? string.Empty,
            user.Email ?? string.Empty,
            user.Active,
            _mapper.Map<IEnumerable<ecommerce.Contracts.Address.AddressResponse>>(addresses)
        );

        return Result.Success(response);
    }

    public async Task<Result<UserProfileResponse>> UpdateAsync(int userId, UpdateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(userId.ToString());
        if (user is null || user.Deleted)
            return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _authRepository.CheckEmailAvailabilityAsync(request.Email))
                return Result.Failure<UserProfileResponse>(UserErrors.DuplicatedEmail);
            user.Email = request.Email;
            user.UserName = request.Email;
        }

        if (!string.Equals(user.PhoneNumber, request.Phone, StringComparison.OrdinalIgnoreCase))
        {
            if (await _authRepository.CheckPhoneAvailabilityAsync(request.Phone))
                return Result.Failure<UserProfileResponse>(UserErrors.DuplicatedPhone);
            user.PhoneNumber = request.Phone;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var updateResult = await _authRepository.UpdateUserAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure<UserProfileResponse>(UserErrors.UpdateFailed);

        var addressResult = await ValidateAndUpsertUserAddressAsync(user.Id, request.Address, cancellationToken);
        if (addressResult.IsFailure)
            return Result.Failure<UserProfileResponse>(addressResult.Error);

        await _unitOfWork.Complete();

        return await GetAsync(userId, cancellationToken);
    }

    public async Task<Result> DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByIdAsync(userId.ToString());
        if (user is null || user.Deleted)
            return Result.Failure(UserErrors.UserNotFound);

        user.Deleted = true;
        user.Active = false;

        var updateResult = await _authRepository.UpdateUserAsync(user);
        if (!updateResult.Succeeded)
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