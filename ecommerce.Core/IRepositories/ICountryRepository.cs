using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories;

public interface ICountryRepository : IGenericRepository<Country>
{
    Task<bool> ExistsByNameAsync(string nameEn, string nameAr, CancellationToken cancellationToken = default);
}
