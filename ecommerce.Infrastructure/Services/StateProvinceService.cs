using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.StateProvince;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using MapsterMapper;

namespace ecommerce.Infrastructure.Services;

public class StateProvinceService(
    IStateProvinceRepository repository,
    ICountryRepository countryRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IStateProvinceService
{
    public async Task<Result<IEnumerable<StateProvinceResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var stateProvinces = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<StateProvinceResponse>>(stateProvinces));
    }

    public async Task<Result<IEnumerable<StateProvinceResponse>>> GetByCountryAsync(int countryId, CancellationToken cancellationToken = default)
    {
        var country = await countryRepository.GetByIdAsync(countryId, cancellationToken);
        if (country is null)
            return Result.Failure<IEnumerable<StateProvinceResponse>>(CountryErrors.NotFound);

        var stateProvinces = await repository.GetByCountryIdAsync(countryId, cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<StateProvinceResponse>>(stateProvinces));
    }

    public async Task<Result<StateProvinceResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var stateProvince = await repository.GetByIdAsync(id, cancellationToken);
        if (stateProvince is null)
            return Result.Failure<StateProvinceResponse>(StateProvinceErrors.NotFound);

        return Result.Success(mapper.Map<StateProvinceResponse>(stateProvince));
    }

    public async Task<Result<StateProvinceResponse>> CreateAsync(CreateStateProvinceRequest request, CancellationToken cancellationToken = default)
    {
        var country = await countryRepository.GetByIdAsync(request.CountryId, cancellationToken);
        if (country is null)
            return Result.Failure<StateProvinceResponse>(CountryErrors.NotFound);

        var stateProvince = mapper.Map<StateProvince>(request);
        stateProvince.Active = true;

        await repository.AddAsync(stateProvince, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<StateProvinceResponse>(stateProvince));
    }

    public async Task<Result<StateProvinceResponse>> UpdateAsync(int id, UpdateStateProvinceRequest request, CancellationToken cancellationToken = default)
    {
        var stateProvince = await repository.GetByIdAsync(id, cancellationToken);
        if (stateProvince is null)
            return Result.Failure<StateProvinceResponse>(StateProvinceErrors.NotFound);

        var country = await countryRepository.GetByIdAsync(request.CountryId, cancellationToken);
        if (country is null)
            return Result.Failure<StateProvinceResponse>(CountryErrors.NotFound);

        mapper.Map(request, stateProvince);

        await repository.UpdateAsync(stateProvince, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<StateProvinceResponse>(stateProvince));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var stateProvince = await repository.GetByIdAsync(id, cancellationToken);
        if (stateProvince is null)
            return Result.Failure(StateProvinceErrors.NotFound);

        await repository.DeleteAsync(id, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success();
    }
}
