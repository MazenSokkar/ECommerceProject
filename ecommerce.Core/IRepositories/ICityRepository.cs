using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories;

public interface ICityRepository : IGenericRepository<City>
{
    Task<IEnumerable<City>> GetByStateProvinceIdAsync(int stateProvinceId, CancellationToken cancellationToken = default);
}
