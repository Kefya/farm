<template>
  <div class="container">
    <Navbar />
    <section class="farm-section">
      <h2>Моя ферма</h2>

      <div class="meta-row">
        <div>Баланс: <strong>{{ user?.balance ?? 0 }}$</strong></div>
        <div>
          <button class="btn primary" @click="onBuyField">Купить поле (100$)</button>
        </div>
      </div>

      <div v-if="loading" class="info">Загрузка полей...</div>
      <div v-if="error" class="error">{{ error }}</div>

      <div class="grid" v-if="fields.length">
        <FieldCard
          v-for="f in fields"
          :key="f.id"
          :field="f"
          :serverNow="serverNow"
          :crops="crops"
          :seedInventory="seeds"
          @plant="handlePlant"
          @harvest="handleHarvest"
        />
      </div>
      <div v-else class="info">Поля не найдены. Купите поле в магазине.</div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, computed } from 'vue';
import Navbar from '@/components/Navbar.vue';
import FieldCard from '@/components/FieldCard.vue';
import { useFarmStore } from '@/stores/farm';
import { useShopStore } from '@/stores/shop';
import { useInventoryStore } from '@/stores/inventory';
import { useAuthStore } from '@/stores/auth';

const farm = useFarmStore();
const shop = useShopStore();
const inventory = useInventoryStore();
const auth = useAuthStore();

const fields = computed(() => farm.fields);
const serverNow = computed(() => farm.serverNow);
const loading = computed(() => farm.loading);
const error = computed(() => farm.error);
const seeds = computed(() => inventory.seeds);
const crops = computed(() => shop.crops);
const user = computed(() => auth.user);

async function handlePlant(payload: { fieldId: number; cropTypeId: number; qty: number }) {
  try {
    await farm.plant(payload.fieldId, payload.cropTypeId, payload.qty);
  } catch (e: any) {
    // FieldCard покажет ошибку на своей стороне; здесь можно добавить глобальный toast
    console.error(e);
  }
}

async function handleHarvest(fieldId: number) {
  try {
    await farm.harvest(fieldId);
  } catch (e: any) {
    console.error(e);
  }
}

async function onBuyField() {
  // Для примера: покупка поля с индексом максим+1
  const next = (fields.value.length ? Math.max(...fields.value.map(f => f.slotIndex)) + 1 : 1);
  try {
    await farm.buyField(next);
  } catch (e: any) {
    console.error(e);
  }
}

onMounted(async () => {
  await shop.loadCrops();
  await inventory.loadSeeds();
  await inventory.loadHarvests();
  await farm.loadFields();
  farm.startPolling();
});
</script>

<style scoped>
.farm-section { margin-top:16px; }
.meta-row { display:flex; justify-content:space-between; align-items:center; margin-bottom:12px; }
.grid { display:grid; grid-template-columns: repeat(auto-fill, minmax(260px,1fr)); gap:12px; }
</style>