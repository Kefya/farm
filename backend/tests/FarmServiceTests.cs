using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using YourProject.Data;
using YourProject.Models;
using YourProject.Services;

namespace YourProject.Tests
{
    public class FarmServiceTests
    {
        private ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task BuySeed_WithInsufficientBalance_ShouldThrow()
        {
            // Arrange
            using var db = CreateInMemoryContext();
            var seedType = new SeedType { Id = 1, Name = "Carrot", SeedPrice = 10, GrowSeconds = 60, SellPrice = 15 };
            db.SeedTypes.Add(seedType);
            var user = new User { Id = 1, Login = "u1", Balance = 5m }; // недостаточно для покупки
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var service = new ShopService(db); // предполагается реализация ShopService

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.BuySeedsAsync(user.Id, seedType.Id, 1));
        }

        [Fact]
        public async Task Planting_ReducesInventoryAndCreatesPlantedRecord()
        {
            // Arrange
            using var db = CreateInMemoryContext();
            var seedType = new SeedType { Id = 1, Name = "Carrot", SeedPrice = 10, GrowSeconds = 60, SellPrice = 15 };
            db.SeedTypes.Add(seedType);
            var user = new User { Id = 1, Login = "u1", Balance = 100m };
            db.Users.Add(user);
            var inv = new InventorySeed { Id = 1, UserId = user.Id, SeedTypeId = seedType.Id, Quantity = 2 };
            db.InventorySeeds.Add(inv);
            var farm = new Farm { Id = 1, UserId = user.Id };
            db.Farms.Add(farm);
            var field = new Field { Id = 1, FarmId = farm.Id, SlotIndex = 0, PurchasedAt = DateTime.UtcNow };
            db.Fields.Add(field);
            await db.SaveChangesAsync();

            var plantService = new FieldService(db);

            // Act
            await plantService.PlantAsync(user.Id, field.Id, seedType.Id);

            // Assert
            var updatedInv = await db.InventorySeeds.FirstAsync(x => x.UserId == user.Id && x.SeedTypeId == seedType.Id);
            Assert.Equal(1, updatedInv.Quantity);

            var planted = await db.Planteds.FirstOrDefaultAsync(p => p.FieldId == field.Id);
            Assert.NotNull(planted);
            Assert.True(planted.HarvestAt > planted.PlantedAt);
        }
    }
}