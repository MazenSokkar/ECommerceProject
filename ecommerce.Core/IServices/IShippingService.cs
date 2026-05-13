using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Shipping;
using ecommerce.Core.Entities.salah_entities;

namespace ecommerce.Core.IServices
{
    public interface IShippingService
    {
        Task<Result> CreateShipmentAsync(int orderId, CancellationToken cancellationToken = default);

        Task<Result> UpdateStatusAsync(
            int orderId,
            ShippingStatus status,
            CancellationToken cancellationToken = default);

        Task<Result<ShippingResponse>> GetByOrderIdAsync(
            int orderId,
            CancellationToken cancellationToken = default);
    }
}
