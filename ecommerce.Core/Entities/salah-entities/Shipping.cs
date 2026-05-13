using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Core.Entities.salah_entities
{
    public class Shipping: AuditableEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public ShippingStatus Status { get; set; } = ShippingStatus.Preparing;
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
