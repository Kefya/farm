<template>
  <div class="container">
    <Navbar />
    <section class="shop-section">
      <h2>Магазин</h2>
      <div v-if="loading" class="info">Загрузка...</div>
      <div v-if="error" class="error">{{ error }}</div>

      <div class="grid">
        <CropCard
          v-for="c in crops"
          :key="c.id"
          :crop="c"
          :userBalance="user?.balance"
          @buy-seeds="onBuySeeds"
        />
      </div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import Navbar from '@/components/Navbar.vue';
import CropCard from '@/components/CropCard.vue';
import { useShopStore } from '@/stores/shop';
import { useAuthStore } from '@/stores/auth';
import { useInventoryStore } from '@/stores/inventory';

const shop = useShopStore();
const auth = useAuthStore();
const inventory = useInventoryStore();

const crops = shop.crops;
const loading = shop.loading;
const error = shop.error;
const user = auth.user;

async function onBuySeeds(payload: { cropTypeId: number; qty: number }) {
  try {
    await shop.buySeeds(payload.cropTypeId, payload.qty);
    // обновляем инвентарь и баланс
    await inventory.loadSeeds();
    // backend может вернуть user, но если нет — можем получить user через /auth/me
  } catch (e: any) {
    console.error(e);
  }
}

onMounted(async () => {
  if (!shop.crops.length) await shop.loadCrops();
});
</script>

<style scoped>
.shop-section { margin-top:16px; }
.grid { display:grid; grid-template-columns: repeat(auto-fill, minmax(240px,1fr)); gap:12px; }
</style>