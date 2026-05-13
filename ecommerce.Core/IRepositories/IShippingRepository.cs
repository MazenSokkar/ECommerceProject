using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Core.Entities.salah_entities;

namespace ecommerce.Core.IRepositories
{
    public interface IShippingRepository
    {
        Task<Shipping?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
        Task AddAsync(Shipping shipping, CancellationToken cancellationToken = default);
        void Update(Shipping shipping);
    }
}
