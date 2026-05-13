using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Core.Entities.salah_entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ecommerce.Infrastructure.Config
{

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.Method)
                .HasConversion<string>();

            builder.Property(p => p.Status)
                .HasConversion<string>();

            builder.Property(p => p.TransactionId)
                .HasMaxLength(200);
        }
    }
}
