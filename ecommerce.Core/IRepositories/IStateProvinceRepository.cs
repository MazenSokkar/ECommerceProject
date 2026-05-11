using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories;

public interface IStateProvinceRepository : IGenericRepository<StateProvince>
{
    Task<IEnumerable<StateProvince>> GetByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);
}
