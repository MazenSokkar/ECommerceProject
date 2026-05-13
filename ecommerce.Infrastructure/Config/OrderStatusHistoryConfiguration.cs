using System;
using System.Collections.Generic;
using System.Text;
using ecommerce.Core.Entities.salah_entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ecommerce.Infrastructure.Config
{

    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Status)
                .HasConversion<string>();

            builder.Property(h => h.Note)
                .HasMaxLength(500);

            builder.HasOne(h => h.ChangedBy)
                .WithMany()
                .HasForeignKey(h => h.ChangedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
