using ElectronicsShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsShop.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<RepairRequest> RepairRequests => Set<RepairRequest>();
    public DbSet<RepairStatusHistory> RepairStatusHistories => Set<RepairStatusHistory>();
    public DbSet<Sale> Sales => Set<Sale>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        builder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RepairRequest>()
            .HasOne(r => r.Customer)
            .WithMany(u => u.RepairRequests)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<RepairStatusHistory>()
            .HasOne(h => h.RepairRequest)
            .WithMany(r => r.StatusHistory)
            .HasForeignKey(h => h.RepairRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
