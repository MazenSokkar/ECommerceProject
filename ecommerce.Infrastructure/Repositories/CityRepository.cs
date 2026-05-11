using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class CityRepository(AppDbContext context) : ICityRepository
{
    public async Task<IEnumerable<City>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Cities.AsNoTracking().Where(c => !c.Deleted).ToListAsync(cancellationToken);

    public async Task<City> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => (await context.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && !c.Deleted, cancellationToken))!;

    public async Task<IEnumerable<City>> GetByStateProvinceIdAsync(int stateProvinceId, CancellationToken cancellationToken = default)
        => await context.Cities.AsNoTracking().Where(c => c.StateProvinceId == stateProvinceId && !c.Deleted).ToListAsync(cancellationToken);

    public async Task<City> AddAsync(City entity, CancellationToken cancellationToken = default)
    {
        await context.Cities.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task<City> UpdateAsync(City entity, CancellationToken cancellationToken = default)
    {
        context.Cities.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var city = await GetByIdAsync(id, cancellationToken);
        if (city is not null)
        {
            city.Deleted = true;
            city.Active = false;
            context.Cities.Update(city);
        }
    }
}
