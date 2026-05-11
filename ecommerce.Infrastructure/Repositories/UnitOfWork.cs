using System;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;

namespace ecommerce.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{

    public readonly AppDbContext _context = context;

    public async Task<int> Complete() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
