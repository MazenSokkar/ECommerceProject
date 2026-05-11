using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class CountryRepository(AppDbContext context) : ICountryRepository
{
    public async Task<IEnumerable<Country>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Countries.AsNoTracking().Where(c => !c.Deleted).ToListAsync(cancellationToken);

    public async Task<Country> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => (await context.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && !c.Deleted, cancellationToken))!;

    public async Task<Country> AddAsync(Country entity, CancellationToken cancellationToken = default)
    {
        await context.Countries.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task<Country> UpdateAsync(Country entity, CancellationToken cancellationToken = default)
    {
        context.Countries.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var country = await GetByIdAsync(id, cancellationToken);
        if (country is not null)
        {
            country.Deleted = true;
            country.Active = false;
            context.Countries.Update(country);
        }
    }

    public async Task<bool> ExistsByNameAsync(string nameEn, string nameAr, CancellationToken cancellationToken = default)
        => await context.Countries.AsNoTracking().AnyAsync(
            c => !c.Deleted && (c.NameEn == nameEn || c.NameAr == nameAr),
            cancellationToken);
}
