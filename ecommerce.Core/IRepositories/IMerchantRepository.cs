using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories
{
    public interface IMerchantRepository
    {
        Task<IEnumerable<Merchant>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Merchant> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Merchant> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task AddAsync(Merchant merchant, CancellationToken cancellationToken = default);
        void Update(Merchant merchant);
       
    }
}
