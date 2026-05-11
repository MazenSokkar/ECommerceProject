using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Country;
using ecommerce.Contracts.Errors;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using MapsterMapper;

namespace ecommerce.Infrastructure.Services;

public class CountryService(
    ICountryRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : ICountryService
{
    public async Task<Result<IEnumerable<CountryResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var countries = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<CountryResponse>>(countries));
    }

    public async Task<Result<CountryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var country = await repository.GetByIdAsync(id, cancellationToken);
        if (country is null)
            return Result.Failure<CountryResponse>(CountryErrors.NotFound);

        return Result.Success(mapper.Map<CountryResponse>(country));
    }

    public async Task<Result<CountryResponse>> CreateAsync(CreateCountryRequest request, CancellationToken cancellationToken = default)
    {
        var duplicate = await repository.ExistsByNameAsync(request.NameEn, request.NameAr, cancellationToken);
        if (duplicate)
            return Result.Failure<CountryResponse>(CountryErrors.DuplicatedName);

        var country = mapper.Map<Country>(request);
        country.Active = true;

        await repository.AddAsync(country, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<CountryResponse>(country));
    }

    public async Task<Result<CountryResponse>> UpdateAsync(int id, UpdateCountryRequest request, CancellationToken cancellationToken = default)
    {
        var country = await repository.GetByIdAsync(id, cancellationToken);
        if (country is null)
            return Result.Failure<CountryResponse>(CountryErrors.NotFound);

        mapper.Map(request, country);

        await repository.UpdateAsync(country, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<CountryResponse>(country));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var country = await repository.GetByIdAsync(id, cancellationToken);
        if (country is null)
            return Result.Failure(CountryErrors.NotFound);

        await repository.DeleteAsync(id, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success();
    }
}
