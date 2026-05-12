using ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ecommerce.Infrastructure.Config
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(120);
            builder.HasIndex(c => c.Slug).IsUnique();
            builder.HasOne(x => x.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.NoAction);
            builder.HasQueryFilter(c => !c.Deleted);


        }
    }
}
