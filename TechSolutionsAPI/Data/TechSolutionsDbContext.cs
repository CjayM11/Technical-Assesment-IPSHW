using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechSolutionsClassLibrary.Models;

namespace TechSolutionsAPI.Data;

public class TechSolutionsDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }

    public TechSolutionsDbContext(DbContextOptions<TechSolutionsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Your model configuration goes here, you can adjust it based on your schema.
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId);

            entity.Property(e => e.City).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Country).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.PostalCode).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Province).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.StreetAddress).HasMaxLength(255).IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CustomerId);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Email).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.InvoiceDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsUnicode(false);
        });

        base.OnModelCreating(modelBuilder);
    }
}
