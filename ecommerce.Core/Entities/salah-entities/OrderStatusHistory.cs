using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Core.Entities.salah_entities
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string? Note { get; set; }
        public int? ChangedById { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Order Order { get; set; } = null!;
        public ApplicationUser? ChangedBy { get; set; }
    }
}
