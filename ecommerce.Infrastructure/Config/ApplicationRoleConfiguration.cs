using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ecommerce.Infrastructure.Config;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.Property(x => x.IsDefault).HasDefaultValue(false);
        builder.Property(x => x.Active).HasDefaultValue(true);
        builder.Property(x => x.Deleted).HasDefaultValue(false);


        // Default data

        builder.HasData([
            new ApplicationRole
        {
            Id = DefaultRoles.AdminRoleId,
            Name = DefaultRoles.Admin,
            NormalizedName = DefaultRoles.Admin.ToUpper(),
            ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
        },
        new ApplicationRole
        {
            Id = DefaultRoles.MerchantRoleId,
            Name = DefaultRoles.Merchant,
            NormalizedName = DefaultRoles.Merchant.ToUpper(),
            ConcurrencyStamp = DefaultRoles.MerchantRoleConcurrencyStamp,
        },
        new ApplicationRole
        {
            Id = DefaultRoles.CorperateRoleId,
            Name = DefaultRoles.Corperate,
            NormalizedName = DefaultRoles.Corperate.ToUpper(),
            ConcurrencyStamp = DefaultRoles.CorperateRoleConcurrencyStamp,
        },
        new ApplicationRole
        {
            Id = DefaultRoles.ReceiverRoleId,
            Name = DefaultRoles.Receiver,
            NormalizedName = DefaultRoles.Receiver.ToUpper(),
            ConcurrencyStamp = DefaultRoles.ReceiverRoleConcurrencyStamp,
        },
        new ApplicationRole
        {
            Id = DefaultRoles.CustomerRoleId,
            Name = DefaultRoles.Customer,
            NormalizedName = DefaultRoles.Customer.ToUpper(),
            ConcurrencyStamp = DefaultRoles.CustomerRoleConcurrencyStamp,
        }
        ]);
    }
}
