<template>
  <div class="container">
    <Navbar />
    <section class="inventory-section">
      <h2>Инвентарь</h2>

      <div class="two-col">
        <div>
          <h3>Семена</h3>
          <div v-if="loadingSeeds">Загрузка...</div>
          <div v-if="seedError" class="error">{{ seedError }}</div>
          <ul>
            <li v-for="s in seeds" :key="s.cropTypeId">
              {{ s.cropName }}: {{ s.quantity }}
            </li>
          </ul>
        </div>

        <div>
          <h3>Урожай</h3>
          <div v-if="loadingHarvests">Загрузка...</div>
          <div v-if="harvestError" class="error">{{ harvestError }}</div>
          <ul>
            <li v-for="h in harvests" :key="h.cropTypeId">
              {{ h.cropName }}: {{ h.quantity }}
              <button class="btn primary" @click="sell(h.cropTypeId)" :disabled="selling">
                {{ selling ? 'Продаем...' : 'Продать 1' }}
              </button>
            </li>
          </ul>
        </div>
      </div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import Navbar from '@/components/Navbar.vue';
import { useInventoryStore } from '@/stores/inventory';
import { ref, onMounted } from 'vue';

const inventory = useInventoryStore();

const seeds = inventory.seeds;
const harvests = inventory.harvests;
const loadingSeeds = inventory.loading;
const loadingHarvests = inventory.loading;
const seedError = inventory.error;
const harvestError = inventory.error;

const selling = ref(false);

async function sell(cropTypeId: number) {
  selling.value = true;
  try {
    await inventory.sell(cropTypeId, 1);
  } catch (e:any) {
    console.error(e);
  } finally {
    selling.value = false;
  }
}

onMounted(async () => {
  await inventory.loadSeeds();
  await inventory.loadHarvests();
});
</script>

<style scoped>
.two-col { display:flex; gap:24px; align-items:flex-start; }
</style>