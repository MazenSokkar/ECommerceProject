using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories
{
    public class ShippingRepository(AppDbContext context) : IShippingRepository
    {
        public async Task<Shipping?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
            => await context.Shippings
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.OrderId == orderId, cancellationToken);

        public async Task AddAsync(Shipping shipping, CancellationToken cancellationToken = default)
            => await context.Shippings.AddAsync(shipping, cancellationToken);

        public void Update(Shipping shipping)
            => context.Shippings.Update(shipping);
    }
}
