using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Core.Entities
{
    public class Product : AuditableEntity
    {
        public int Id { get; set; }
        public int MerchantId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public bool Deleted { get; set; }

        public Merchant Merchant { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> Images { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<WishlistItem> WishlistItems { get; set; } = [];



    }
}
