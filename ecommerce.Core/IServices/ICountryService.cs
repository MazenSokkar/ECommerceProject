using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Country;

namespace ecommerce.Core.IServices;

public interface ICountryService
{
    Task<Result<IEnumerable<CountryResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<CountryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<CountryResponse>> CreateAsync(CreateCountryRequest request, CancellationToken cancellationToken = default);
    Task<Result<CountryResponse>> UpdateAsync(int id, UpdateCountryRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
