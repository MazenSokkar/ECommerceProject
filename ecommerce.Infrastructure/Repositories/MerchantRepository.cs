using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories
{
    public class MerchantRepository(AppDbContext context) : IMerchantRepository
    {
        public async Task AddAsync(Merchant merchant, CancellationToken cancellationToken = default)
        => await context.Merchants.AddAsync(merchant, cancellationToken);

       
        public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await context.Merchants
            .AnyAsync(x => x.UserId == userId, cancellationToken);

        public async Task<Merchant> FindByIdAsync(int id, CancellationToken cancellationToken = default)
         => await context.Merchants
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<Merchant> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default)
          => await context.Merchants
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);


        public async Task<IEnumerable<Merchant>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Merchants
            .Include(s => s.User)
            .ToListAsync(cancellationToken);

        public void Update(Merchant merchant)
     => context.Merchants.Update(merchant);

    }
}
