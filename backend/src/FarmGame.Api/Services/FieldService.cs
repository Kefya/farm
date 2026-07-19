using FarmGame.Api.Data;
using FarmGame.Api.DTOs.Field;
using FarmGame.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmGame.Api.Services
{
    public class FieldService : IFieldService
    {
        private readonly ApplicationDbContext _db;
        private readonly IInventoryService _inventoryService;
        public FieldService(ApplicationDbContext db, IInventoryService inventoryService)
        {
            _db = db;
            _inventoryService = inventoryService;
        }

        public async Task<IEnumerable<Field>> GetFieldsForUserAsync(Guid userId)
        {
            var farm = await _db.Farms.AsNoTracking().Include(f => f.Fields).SingleOrDefaultAsync(f => f.UserId == userId);
            return farm?.Fields ?? Enumerable.Empty<Field>();
        }

        public async Task<OperationResult> PlantAsync(Guid userId, PlantRequest req)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var field = await _db.Fields.Include(f => f.PlantedCrop)
                                        .SingleOrDefaultAsync(f => f.Id == req.FieldId);
            if (field == null) throw new DomainException("Field not found", 404);

            // проверка права владения поля
            var farm = await _db.Farms.AsNoTracking().SingleOrDefaultAsync(f => f.Id == field.FarmId && f.UserId == userId);
            if (farm == null) throw new DomainException("Field does not belong to user", 403);

            if (field.PlantedCrop != null && field.PlantedCrop.Status != "Harvested")
                throw new DomainException("Field is occupied", 409);

            var cropType = await _db.CropTypes.SingleOrDefaultAsync(c => c.Id == req.CropTypeId);
            if (cropType == null) throw new DomainException("Crop type not found", 404);

            // вычитаем семя из инвентаря
            var inv = await _db.InventorySeeds.SingleOrDefaultAsync(i => i.UserId == userId && i.CropTypeId == req.CropTypeId);
            if (inv == null || inv.Quantity <= 0) throw new DomainException("No seeds", 403);

            inv.Quantity -= 1;
            _db.InventorySeeds.Update(inv);

            var now = DateTime.UtcNow;
            var planted = new PlantedCrop
            {
                FieldId = field.Id,
                CropTypeId = cropType.Id,
                PlantedAt = now,
                ReadyAt = now.AddSeconds(cropType.GrowSeconds),
                Status = "Planted"
            };

            await _db.PlantedCrops.AddAsync(planted);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return OperationResult.Ok();
        }

        public async Task<HarvestResponse> HarvestAsync(Guid userId, Guid fieldId)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var field = await _db.Fields.Include(f => f.PlantedCrop).ThenInclude(p => p.CropType)
                                        .SingleOrDefaultAsync(f => f.Id == fieldId);
            if (field == null) throw new DomainException("Field not found", 404);

            var farm = await _db.Farms.AsNoTracking().SingleOrDefaultAsync(f => f.Id == field.FarmId && f.UserId == userId);
            if (farm == null) throw new DomainException("Field does not belong to user", 403);

            var planted = field.PlantedCrop;
            if (planted == null) throw new DomainException("Nothing planted", 409);

            if (planted.HarvestedAt != null) throw new DomainException("Already harvested", 409);

            var now = DateTime.UtcNow;
            if (planted.ReadyAt > now) throw new DomainException("Crop not ready", 409);

            // помечаем как собранный и добавляем в inventory_crops
            planted.HarvestedAt = now;
            planted.Status = "Harvested";
            _db.PlantedCrops.Update(planted);
            var invCrop = await _db.InventoryCrops.SingleOrDefaultAsync(ic => ic.UserId == userId && ic.CropTypeId == planted.CropTypeId);
            if (invCrop == null)
            {
                invCrop = new InventoryCrop { UserId = userId, CropTypeId = planted.CropTypeId, Quantity = 1 };
                await _db.InventoryCrops.AddAsync(invCrop);
            }
            else
            {
                invCrop.Quantity += 1;
                _db.InventoryCrops.Update(invCrop);
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return new HarvestResponse
            {
                FieldId = fieldId,
                CropTypeId = planted.CropTypeId,
                QuantityAddedToInventory = 1,
                SellValueIfSold = planted.CropType.SellPrice
            };
        }

        public async Task<OperationResult> BuyFieldAsync(Guid userId, BuyFieldRequest req)
        {
            const decimal FieldPrice = 50m; // пример цены поля, можно вынести в конфиг

            await using var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new DomainException("User not found", 404);

            if (user.Balance < FieldPrice) throw new DomainException("Insufficient balance", 403);

            user.Balance -= FieldPrice;
            _db.Users.Update(user);

            // determine slot
            int nextSlot = req.SlotIndex ?? (await _db.Fields.Where(f => f.Farm.UserId == userId).MaxAsync(f => (int?)f.SlotIndex) ?? -1) + 1;

            var farm = await _db.Farms.SingleOrDefaultAsync(f => f.UserId == userId);
            if (farm == null) throw new DomainException("Farm not found", 404);

            var field = new Field { FarmId = farm.Id, SlotIndex = nextSlot, BoughtAt = DateTime.UtcNow, PriceBought = FieldPrice };
            await _db.Fields.AddAsync(field);

            await _db.Transactions.AddAsync(new Transaction { UserId = userId, Type = "field_buy", Amount = -FieldPrice, Metadata = null });

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return OperationResult.Ok(new { FieldId = field.Id });
        }
    }
}