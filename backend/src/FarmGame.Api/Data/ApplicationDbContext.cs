using System.Data.Common;
using FarmGame.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmGame.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Farm> Farms { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<CropType> CropTypes { get; set; }
        public DbSet<InventorySeed> InventorySeeds { get; set; }
        public DbSet<InventoryCrop> InventoryCrops { get; set; }
        public DbSet<PlantedCrop> PlantedCrops { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ключи, индексы, ограничения
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Farm>()
                .HasOne(f => f.User)
                .WithOne(u => u.Farm)
                .HasForeignKey<Farm>(f => f.UserId);

            modelBuilder.Entity<Field>()
                .HasIndex(f => new { f.FarmId, f.SlotIndex })
                .IsUnique();

            // Enum mapping, decimal precision и т.д.
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<CropType>()
                .Property(c => c.SeedPrice).HasColumnType("numeric(18,2)");
            modelBuilder.Entity<CropType>()
                .Property(c => c.SellPrice).HasColumnType("numeric(18,2)");

            modelBuilder.Entity<CropType>().HasData(
                new CropType { Id = 1, Name = "Морковь", SeedPrice = 10m, GrowSeconds = 60, SellPrice = 15m },
                new CropType { Id = 2, Name = "Картофель", SeedPrice = 25m, GrowSeconds = 180, SellPrice = 40m },
                new CropType { Id = 3, Name = "Клубника", SeedPrice = 50m, GrowSeconds = 300, SellPrice = 90m }

            );

            base.OnModelCreating(modelBuilder);
        }
    }
}