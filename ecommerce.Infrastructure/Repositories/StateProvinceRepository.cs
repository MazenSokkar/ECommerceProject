using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class StateProvinceRepository(AppDbContext context) : IStateProvinceRepository
{
    public async Task<IEnumerable<StateProvince>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.StateProvinces.AsNoTracking().Where(s => !s.Deleted).ToListAsync(cancellationToken);

    public async Task<StateProvince> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => (await context.StateProvinces.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && !s.Deleted, cancellationToken))!;

    public async Task<IEnumerable<StateProvince>> GetByCountryIdAsync(int countryId, CancellationToken cancellationToken = default)
        => await context.StateProvinces.AsNoTracking().Where(s => s.CountryId == countryId && !s.Deleted).ToListAsync(cancellationToken);

    public async Task<StateProvince> AddAsync(StateProvince entity, CancellationToken cancellationToken = default)
    {
        await context.StateProvinces.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task<StateProvince> UpdateAsync(StateProvince entity, CancellationToken cancellationToken = default)
    {
        context.StateProvinces.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var stateProvince = await GetByIdAsync(id, cancellationToken);
        if (stateProvince is not null)
        {
            stateProvince.Deleted = true;
            stateProvince.Active = false;
            context.StateProvinces.Update(stateProvince);
        }
    }
}
