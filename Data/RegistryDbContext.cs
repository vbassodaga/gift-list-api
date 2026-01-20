using Microsoft.EntityFrameworkCore;
using HousewarmingRegistry.API.Models;

namespace HousewarmingRegistry.API.Data;

public class RegistryDbContext : DbContext
{
    public RegistryDbContext(DbContextOptions<RegistryDbContext> options) : base(options)
    {
    }

    public DbSet<Gift> Gifts { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).IsRequired();
            entity.HasIndex(e => e.PhoneNumber).IsUnique();
        });

        modelBuilder.Entity<Gift>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            
            // Foreign key relationship
            entity.HasOne(e => e.PurchasedByUser)
                  .WithMany(u => u.PurchasedGifts)
                  .HasForeignKey(e => e.PurchasedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}

