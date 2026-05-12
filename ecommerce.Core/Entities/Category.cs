using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Core.Entities
{
    public class Category : AuditableEntity
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; } = true;

        //navigation 
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
