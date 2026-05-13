using System;
using System.Reflection;
using System.Security.Claims;
using ecommerce.Core.Entities;
using ecommerce.Core.Entities.salah_entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options)
{

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<ApplicationRole> Roles { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<StateProvince> StateProvinces { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Wishlist>  Wishlists { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    #region salah
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<Shipping> Shippings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    #endregion


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType)) continue;

            modelBuilder.Entity(entityType.ClrType)
                .HasOne(typeof(ApplicationUser), nameof(AuditableEntity.CreatedBy))
                .WithMany()
                .HasForeignKey(nameof(AuditableEntity.CreatedById))
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity(entityType.ClrType)
                .HasOne(typeof(ApplicationUser), nameof(AuditableEntity.UpdatedBy))
                .WithMany()
                .HasForeignKey(nameof(AuditableEntity.UpdatedById))
                .OnDelete(DeleteBehavior.Restrict);
        }

        var cascadeFKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entityEntry in entries)
        {
            var userIdString = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserId = int.TryParse(userIdString, out var id) ? (int?)id : null;

            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;
                if (currentUserId.HasValue)
                {
                    entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId.Value;
                }
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                if (currentUserId.HasValue)
                {
                    entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId.Value;
                }
                else
                {
                    entityEntry.Property(x => x.UpdatedById).IsModified = false;
                }
                entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
