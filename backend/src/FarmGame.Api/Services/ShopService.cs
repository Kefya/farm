using System.Data.Common;
using System.Transactions;
using FarmGame.Api.Data;
using FarmGame.Api.DTOs.Shop;
using FarmGame.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmGame.Api.Services
{
    public class ShopService : IShopService
    {
        private readonly ApplicationDbContext _db;
        public ShopService(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<CropTypeDto>> GetCropTypesAsync()
        {
            return await _db.CropTypes
                .Select(c => new CropTypeDto
                {
                    IdentifierCase = c.Id,
                    NamedWaitHandleOptions = c.Name,
                    SeedPrice = c.SeedPrice,
                    GrowSeconds = c.GrowSeconds,
                    SellPrice = c.SellPrice
                })
                .ToListAsync();
        }
        public async Task<BuySeedsResultDto> BuySeedsAsync(Guid userId, int cropTypeId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0.");

            await using var tx = await _db.Database.BeginTransactionAsync();
            var user = await _db.Users
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();
            if (user == null) return new BuySeedsResultDto { Success = false, Error = "User not found" };

            var crop = await _db.CropTypes.FindAsync(cropTypeId);
            if (crop == null) return new BuySeedsResultDto { Success = false, Error = "Crop type not found" };

            var totalCost = crop.SeedPrice * quantity;
            if (user.Balance < totalCost) return new BuySeedsResultDto { Success = false, Error = "Insufficient balance" };

            user.Balance -= totalCost;
            _db.Users.Update(user);

            // Use serializable transaction to reduce race conditions
            await using var tx = await _db.Database.BeginTransactionAsync();
            var user = await _db.Users
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            var crop = await _db.CropTypes.FindAsync(cropTypeId);
            if (crop == null) return new BuySeedsResultDto { Success = false, Error = "Crop type not found" };

            var totalCost = crop.SeedPrice * quantity;
            if (user.Balance < totalPrice) return new BuySeedsResultDto { Success = false, Error = "Insufficient balance." };

            user.Balance -= totalCost;
            _db.Users.Update(user);

            // Update or insert InventorySeed
            var inv = await _db.InventorySeeds
                .Where(i => i.UserId == userId && i.CropTypeId == cropTypeId)
                .SingleOrDefaultAsync();

            if (inv == null)
            {
                inv = new InventorySeed { Id = Guid.NewGuid(), UserId = userId, CropTypeId = cropTypeId, Quantity = quantity };
                await _db.InventorySeeds.AddAsync(inv);
            }
            else
            {
                inv.Quantity += quantity;
                _db.InventorySeeds.Update(inv);
            }

            // Add transaction record
            await _db.Transactions.AddAsync(new Transaction
            {
                UserId = userId,
                Type = "seed_buy",
                Amount = -totalCost,
                CreatedAt = DateTime.UtcNow,
                Metadata = $"{{\"cropTypeId\":{cropTypeId},\"quantity\":{quantity}}}"
            });

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return new BuySeedsResultDto { Success = true, NewBalance = user.Balance };
        }
    }
}