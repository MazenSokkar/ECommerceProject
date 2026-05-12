namespace ecommerce.Core.Entities;

public class Review : AuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool Deleted { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}