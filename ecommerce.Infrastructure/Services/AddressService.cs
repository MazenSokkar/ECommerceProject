using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Address;
using ecommerce.Contracts.Errors;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using MapsterMapper;

namespace ecommerce.Infrastructure.Services;

public class AddressService(
    IAddressRepository repository,
    ICountryRepository countryRepository,
    IStateProvinceRepository stateProvinceRepository,
    ICityRepository cityRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IAddressService
{
    public async Task<Result<IEnumerable<AddressResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var addresses = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<AddressResponse>>(addresses));
    }

    public async Task<Result<IEnumerable<AddressResponse>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var addresses = await repository.GetByUserIdAsync(userId, cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<AddressResponse>>(addresses));
    }

    public async Task<Result<IEnumerable<AddressResponse>>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken = default)
    {
        var addresses = await repository.GetByMerchantIdAsync(merchantId, cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<AddressResponse>>(addresses));
    }

    public async Task<Result<AddressResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var address = await repository.GetByIdAsync(id, cancellationToken);
        if (address is null)
            return Result.Failure<AddressResponse>(AddressErrors.NotFound);

        return Result.Success(mapper.Map<AddressResponse>(address));
    }

    public async Task<Result<AddressResponse>> CreateAsync(CreateAddressRequest request, CancellationToken cancellationToken = default)
    {
        var country = await countryRepository.GetByIdAsync(request.CountryId, cancellationToken);
        if (country is null)
            return Result.Failure<AddressResponse>(CountryErrors.NotFound);

        var stateProvince = await stateProvinceRepository.GetByIdAsync(request.StateProvinceId, cancellationToken);
        if (stateProvince is null)
            return Result.Failure<AddressResponse>(StateProvinceErrors.NotFound);

        var city = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
            return Result.Failure<AddressResponse>(CityErrors.NotFound);

        var address = mapper.Map<Address>(request);

        await repository.AddAsync(address, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<AddressResponse>(address));
    }

    public async Task<Result<AddressResponse>> UpdateAsync(int id, UpdateAddressRequest request, CancellationToken cancellationToken = default)
    {
        var address = await repository.GetByIdAsync(id, cancellationToken);
        if (address is null)
            return Result.Failure<AddressResponse>(AddressErrors.NotFound);

        var country = await countryRepository.GetByIdAsync(request.CountryId, cancellationToken);
        if (country is null)
            return Result.Failure<AddressResponse>(CountryErrors.NotFound);

        var stateProvince = await stateProvinceRepository.GetByIdAsync(request.StateProvinceId, cancellationToken);
        if (stateProvince is null)
            return Result.Failure<AddressResponse>(StateProvinceErrors.NotFound);

        var city = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
            return Result.Failure<AddressResponse>(CityErrors.NotFound);

        mapper.Map(request, address);

        await repository.UpdateAsync(address, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<AddressResponse>(address));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var address = await repository.GetByIdAsync(id, cancellationToken);
        if (address is null)
            return Result.Failure(AddressErrors.NotFound);

        await repository.DeleteAsync(id, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success();
    }
}
