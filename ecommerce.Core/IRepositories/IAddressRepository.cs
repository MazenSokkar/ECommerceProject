using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories;

public interface IAddressRepository : IGenericRepository<Address>
{
    Task<IEnumerable<Address>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Address>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken = default);
    Task<Address> UpsertUserAddressAsync(int userId, Address address, CancellationToken cancellationToken = default);
}
