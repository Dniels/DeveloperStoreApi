using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SaleNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.SaleNumber)
                .IsUnique();

            builder.Property(x => x.SaleDate)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.IsCancelled)
                .IsRequired();

            // Customer Value Object
            builder.OwnsOne(x => x.Customer, customer =>
            {
                customer.Property(c => c.Id)
                    .HasColumnName("CustomerId")
                    .IsRequired();

                customer.Property(c => c.Name)
                    .HasColumnName("CustomerName")
                    .HasMaxLength(200)
                    .IsRequired();

                customer.Property(c => c.Email)
                    .HasColumnName("CustomerEmail")
                    .HasMaxLength(300)
                    .IsRequired();
            });

            // Branch Value Object
            builder.OwnsOne(x => x.Branch, branch =>
            {
                branch.Property(b => b.Id)
                    .HasColumnName("BranchId")
                    .IsRequired();

                branch.Property(b => b.Name)
                    .HasColumnName("BranchName")
                    .HasMaxLength(200)
                    .IsRequired();

                branch.Property(b => b.Address)
                    .HasColumnName("BranchAddress")
                    .HasMaxLength(500)
                    .IsRequired();
            });

            // Items relationship
            builder.HasMany(typeof(SaleItem), "Items")
                .WithOne()
                .HasForeignKey("SaleId")
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore domain events for EF
            builder.Ignore(x => x.DomainEvents);
        }
    }
}
