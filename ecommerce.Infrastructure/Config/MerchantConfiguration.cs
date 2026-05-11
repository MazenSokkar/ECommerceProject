using ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ecommerce.Infrastructure.Config;

public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.HasMany<Address>()
            .WithOne(a => a.Merchant)
            .HasForeignKey(a => a.MerchantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
