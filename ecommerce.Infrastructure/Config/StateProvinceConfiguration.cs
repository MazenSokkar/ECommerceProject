using ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ecommerce.Infrastructure.Config;

public class StateProvinceConfiguration : IEntityTypeConfiguration<StateProvince>
{
    public void Configure(EntityTypeBuilder<StateProvince> builder)
    {
        builder.Property(x => x.NameAr).HasMaxLength(256).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Active).HasDefaultValue(true);
        builder.Property(x => x.Deleted).HasDefaultValue(false);

        builder.HasMany<City>()
            .WithOne(c => c.StateProvince)
            .HasForeignKey(c => c.StateProvinceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
