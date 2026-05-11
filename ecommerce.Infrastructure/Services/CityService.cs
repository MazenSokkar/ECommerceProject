using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.City;
using ecommerce.Contracts.Errors;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using MapsterMapper;

namespace ecommerce.Infrastructure.Services;

public class CityService(
    ICityRepository repository,
    ICountryRepository countryRepository,
    IStateProvinceRepository stateProvinceRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : ICityService
{
    public async Task<Result<IEnumerable<CityResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cities = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<CityResponse>>(cities));
    }

    public async Task<Result<IEnumerable<CityResponse>>> GetByStateProvinceAsync(int stateProvinceId, CancellationToken cancellationToken = default)
    {
        var stateProvince = await stateProvinceRepository.GetByIdAsync(stateProvinceId, cancellationToken);
        if (stateProvince is null)
            return Result.Failure<IEnumerable<CityResponse>>(StateProvinceErrors.NotFound);

        var cities = await repository.GetByStateProvinceIdAsync(stateProvinceId, cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<CityResponse>>(cities));
    }

    public async Task<Result<CityResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var city = await repository.GetByIdAsync(id, cancellationToken);
        if (city is null)
            return Result.Failure<CityResponse>(CityErrors.NotFound);

        return Result.Success(mapper.Map<CityResponse>(city));
    }

    public async Task<Result<CityResponse>> CreateAsync(CreateCityRequest request, CancellationToken cancellationToken = default)
    {
        var country = await countryRepository.GetByIdAsync(request.CountryId, cancellationToken);
        if (country is null)
            return Result.Failure<CityResponse>(CountryErrors.NotFound);

        var stateProvince = await stateProvinceRepository.GetByIdAsync(request.StateProvinceId, cancellationToken);
        if (stateProvince is null)
            return Result.Failure<CityResponse>(StateProvinceErrors.NotFound);

        var city = mapper.Map<City>(request);
        city.Active = true;

        await repository.AddAsync(city, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<CityResponse>(city));
    }

    public async Task<Result<CityResponse>> UpdateAsync(int id, UpdateCityRequest request, CancellationToken cancellationToken = default)
    {
        var city = await repository.GetByIdAsync(id, cancellationToken);
        if (city is null)
            return Result.Failure<CityResponse>(CityErrors.NotFound);

        var country = await countryRepository.GetByIdAsync(request.CountryId, cancellationToken);
        if (country is null)
            return Result.Failure<CityResponse>(CountryErrors.NotFound);

        var stateProvince = await stateProvinceRepository.GetByIdAsync(request.StateProvinceId, cancellationToken);
        if (stateProvince is null)
            return Result.Failure<CityResponse>(StateProvinceErrors.NotFound);

        mapper.Map(request, city);

        await repository.UpdateAsync(city, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<CityResponse>(city));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var city = await repository.GetByIdAsync(id, cancellationToken);
        if (city is null)
            return Result.Failure(CityErrors.NotFound);

        await repository.DeleteAsync(id, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success();
    }
}
