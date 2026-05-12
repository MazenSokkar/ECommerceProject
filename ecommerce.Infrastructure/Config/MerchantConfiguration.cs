using ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ecommerce.Infrastructure.Config;

public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.StoreName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasDefaultValue("pending");

        builder.HasMany<Address>()
            .WithOne(a => a.Merchant)
            .HasForeignKey(a => a.MerchantId)
            .OnDelete(DeleteBehavior.Restrict);

        //builder.HasMany(m => m.Products)
        //    .WithOne(p => p.Merchant)
        //    .HasForeignKey(p => p.MerchantId)
        //    .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(m => !m.Deleted);
    }
}