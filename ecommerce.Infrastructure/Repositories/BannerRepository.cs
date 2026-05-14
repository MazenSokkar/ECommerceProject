using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class BannerRepository(AppDbContext context) : IBannerRepository
{
    public async Task<IEnumerable<Banner>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Banners.ToListAsync(cancellationToken);

    public async Task<Banner> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Banners.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<Banner> AddAsync(Banner entity, CancellationToken cancellationToken = default)
    {
        await context.Banners.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task<Banner> UpdateAsync(Banner entity, CancellationToken cancellationToken = default)
    {
        context.Banners.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            context.Banners.Remove(entity);
        }
    }
}
