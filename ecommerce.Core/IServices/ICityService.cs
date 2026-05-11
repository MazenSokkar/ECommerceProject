using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.City;

namespace ecommerce.Core.IServices;

public interface ICityService
{
    Task<Result<IEnumerable<CityResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CityResponse>>> GetByStateProvinceAsync(int stateProvinceId, CancellationToken cancellationToken = default);
    Task<Result<CityResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<CityResponse>> CreateAsync(CreateCityRequest request, CancellationToken cancellationToken = default);
    Task<Result<CityResponse>> UpdateAsync(int id, UpdateCityRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
