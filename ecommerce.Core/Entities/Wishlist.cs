namespace ecommerce.Core.Entities;

public class Wishlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public ICollection<WishlistItem> Items { get; set; } = [];
}