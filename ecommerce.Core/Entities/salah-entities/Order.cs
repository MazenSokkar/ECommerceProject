using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Core.Entities.salah_entities
{
    public class Order : AuditableEntity
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int? AddressId { get; set; }

        // Snapshot of address at order time
        public string ShippingAddress { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? Notes { get; set; }

        // Navigation
        public ApplicationUser User { get; set; } = null!;
        public Address? Address { get; set; }
        public ICollection<OrderItem> Items { get; set; } = [];
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = [];
        public Shipping? Shipping { get; set; }
        public Payment? Payment { get; set; }
    }
}
