using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class AddressRepository(AppDbContext context) : IAddressRepository
{
    public async Task<IEnumerable<Address>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Addresses.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Address> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => (await context.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken))!;

    public async Task<IEnumerable<Address>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await context.Addresses.AsNoTracking().Where(a => a.UserId == userId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Address>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken = default)
        => await context.Addresses.AsNoTracking().Where(a => a.MerchantId == merchantId).ToListAsync(cancellationToken);

    public async Task<Address> AddAsync(Address entity, CancellationToken cancellationToken = default)
    {
        await context.Addresses.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task<Address> UpdateAsync(Address entity, CancellationToken cancellationToken = default)
    {
        context.Addresses.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var address = await GetByIdAsync(id, cancellationToken);
        if (address is not null)
            context.Addresses.Remove(address);
    }
}
