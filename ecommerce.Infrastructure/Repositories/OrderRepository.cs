using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<Order?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
       => await context.Orders
           .Include(o => o.User)
           .Include(o => o.Items)
               .ThenInclude(i => i.Product)
                   .ThenInclude(p => p.Images)
           .Include(o => o.Address)
               .ThenInclude(a => a.City)
                   .ThenInclude(c => c.Country)
           .Include(o => o.Payment)
           .Include(o => o.Shipping)
           .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<Order?> FindByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
     => await context.Orders
         .Include(o => o.User)
         .Include(o => o.Items)
             .ThenInclude(i => i.Product)
                 .ThenInclude(p => p.Images)
         .Include(o => o.Address)
             .ThenInclude(a => a.City)
                 .ThenInclude(c => c.Country)
         .Include(o => o.Payment)
         .Include(o => o.Shipping)
         .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);

    public async Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
      => await context.Orders
          .Include(o => o.Items)
              .ThenInclude(i => i.Product)
                  .ThenInclude(p => p.Images)
          .Where(o => o.UserId == userId)
          .OrderByDescending(o => o.CreatedOn)
          .ToListAsync(cancellationToken);

    public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
     => await context.Orders
         .Include(o => o.User)
         .Include(o => o.Items)
         .OrderByDescending(o => o.CreatedOn)
         .ToListAsync(cancellationToken);

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        => await context.Orders.AddAsync(order, cancellationToken);

    public void Update(Order order)
        => context.Orders.Update(order);
}