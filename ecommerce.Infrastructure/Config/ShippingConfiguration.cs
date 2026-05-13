using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Core.Entities.salah_entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ecommerce.Infrastructure.Config
{

    public class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
    {
        public void Configure(EntityTypeBuilder<Shipping> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TrackingNumber)
                .HasMaxLength(100);

            builder.Property(s => s.Carrier)
                .HasMaxLength(100);

            builder.Property(s => s.Status)
                .HasConversion<string>();
        }
    }
}
