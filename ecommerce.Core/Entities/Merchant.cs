using System;

namespace ecommerce.Core.Entities;

public class Merchant : AuditableEntity
{
    public int Id { get; set; }
    public int UserId {  get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? StoreLogo { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "pending";

    public bool Deleted { get; set; }
    public bool Active { get; set; } = true;

    public ApplicationUser User { get; set; } = null!;
     public ICollection<Product> Products { get; set; } = [];
}
