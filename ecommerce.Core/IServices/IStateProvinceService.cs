using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.StateProvince;

namespace ecommerce.Core.IServices;

public interface IStateProvinceService
{
    Task<Result<IEnumerable<StateProvinceResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<StateProvinceResponse>>> GetByCountryAsync(int countryId, CancellationToken cancellationToken = default);
    Task<Result<StateProvinceResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<StateProvinceResponse>> CreateAsync(CreateStateProvinceRequest request, CancellationToken cancellationToken = default);
    Task<Result<StateProvinceResponse>> UpdateAsync(int id, UpdateStateProvinceRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
