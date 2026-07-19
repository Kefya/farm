using FarmGame.Api.Data;
using FarmGame.Api.DTOs.Inventory;
using Microsoft.EntityFrameworkCore;

namespace FarmGame.Api.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _db;
        public InventoryService(ApplicationDbContext db) => _db = db;

        public async Task<int> GetSeedQuantityAsync(Guid userId, int cropTypeId)
        {
            var inv = await _db.InventorySeeds.AsNoTracking()
                        .SingleOrDefaultAsync(i => i.UserId == userId && i.CropTypeId == cropTypeId);
            return inv?.Quantity ?? 0;
        }

        public async Task<IEnumerable<InventorySeed>> GetSeedsAsync(Guid userId)
        {
            return await _db.InventorySeeds.AsNoTracking().Where(i => i.UserId == userId).Include(i => i.CropType).ToListAsync();
        }

        public async Task<IEnumerable<InventoryCrop>> GetCropsAsync(Guid userId)
        {
            return await _db.InventoryCrops.AsNoTracking().Where(i => i.UserId == userId).Include(i => i.CropType).ToListAsync();
        }

        public async Task<OperationResult> SellCropAsync(Guid userId, SellCropRequest req)
        {
            if (req.Quantity <= 0) throw new DomainException("Quantity must be > 0");

            await using var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var inv = await _db.InventoryCrops.SingleOrDefaultAsync(ic => ic.UserId == userId && ic.CropTypeId == req.CropTypeId);
            if (inv == null || inv.Quantity < req.Quantity) throw new DomainException("Not enough crops", 403);

            var cropType = await _db.CropTypes.SingleOrDefaultAsync(c => c.Id == req.CropTypeId);
            if (cropType == null) throw new DomainException("Crop type not found", 404);

            inv.Quantity -= req.Quantity;
            _db.InventoryCrops.Update(inv);

            var total = cropType.SellPrice * req.Quantity;

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new DomainException("User not found", 404);

            user.Balance += total;
            user.TotalEarned += total;
            _db.Users.Update(user);

            await _db.Transactions.AddAsync(new Transaction
            {
                UserId = userId,
                Type = "crop_sell",
                Amount = total,
                Metadata = System.Text.Json.JsonSerializer.Serialize(new { CropTypeId = req.CropTypeId, Quantity = req.Quantity })
            });

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return OperationResult.Ok(new { NewBalance = user.Balance });
        }
    }
}