using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("SaleItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SaleId)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Discount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.IsCancelled)
                .IsRequired();

            // Product Value Object
            builder.OwnsOne(x => x.Product, product =>
            {
                product.Property(p => p.Id)
                    .HasColumnName("ProductId")
                    .IsRequired();

                product.Property(p => p.Name)
                    .HasColumnName("ProductName")
                    .HasMaxLength(200)
                    .IsRequired();

                product.Property(p => p.Description)
                    .HasColumnName("ProductDescription")
                    .HasMaxLength(1000);

                product.Property(p => p.Category)
                    .HasColumnName("ProductCategory")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // Ignore domain events for EF
            builder.Ignore(x => x.DomainEvents);
        }
    }
}
