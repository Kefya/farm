<template>
  <div class="field-card">
    <div class="slot-header">
      <div class="slot-index">#{{ field.slotIndex }}</div>
      <div class="slot-status" :class="{ occupied: !!field.planting }">
        <span v-if="!field.planting">Свободно</span>
        <span v-else>{{ field.planting.cropName }}</span>
      </div>
    </div>

    <div class="slot-body">
      <div v-if="!field.planting" class="empty-actions">
        <label class="small">Посадить:</label>
        <select v-model.number="selCropId" aria-label="Select crop">
          <option :value="0" disabled>Выберите культуру</option>
          <option v-for="c in crops" :key="c.id" :value="c.id">
            {{ c.name }} — {{ c.seedPrice }}$
          </option>
        </select>

        <input type="number" min="1" v-model.number="selQty" class="qty-input" />

        <button
          :disabled="!canPlant"
          @click="onPlant"
          class="btn primary"
        >
          Посадить
        </button>

        <div v-if="plantError" class="error">{{ plantError }}</div>
      </div>

      <div v-else class="planted-info">
        <div class="crop-name">{{ field.planting.cropName }}</div>
        <div class="countdown">
          <span v-if="!isReady">Осталось: {{ remaining }}</span>
          <span v-else>Готово к сбору</span>
        </div>

        <button
          class="btn success"
          :disabled="!isReady || harvesting"
          @click="onHarvest"
        >
          {{ harvesting ? 'Сбор...' : 'Собрать' }}
        </button>

        <div v-if="harvestError" class="error">{{ harvestError }}</div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, watch } from 'vue';
import type { Field, CropType, SeedInventoryItem } from '@/types';
import { formatDuration } from '@/utils/formatDuration';

const props = defineProps<{
  field: Field;
  serverNow: string;
  crops: CropType[];
  seedInventory: SeedInventoryItem[];
}>();

const emits = defineEmits<{
  (e: 'plant', payload: { fieldId: number; cropTypeId: number; qty: number }): void;
  (e: 'harvest', fieldId: number): void;
}>();

const selCropId = ref<number>(0);
const selQty = ref<number>(1);
const plantError = ref<string | null>(null);
const harvestError = ref<string | null>(null);
const harvesting = ref(false);

// Remaining ms computed from serverNow and planting.readyAt
const remainingMs = computed<number | null>(() => {
  if (!props.field.planting) return null;
  const ready = new Date(props.field.planting.readyAt).getTime();
  const server = new Date(props.serverNow).getTime();
  return ready - server;
});

const isReady = computed(() => remainingMs.value !== null && remainingMs.value <= 0);

const remaining = computed(() => {
  if (remainingMs.value === null) return '';
  if (remainingMs.value <= 0) return '00:00';
  return formatDuration(remainingMs.value);
});

// Проверка наличия семян в инвентаре
function availableSeedsFor(cropId: number) {
  const item = props.seedInventory.find((s) => s.cropTypeId === cropId);
  return item ? item.quantity : 0;
}

const canPlant = computed(() => {
  plantError.value = null;
  if (props.field.planting) {
    plantError.value = 'Поле занято';
    return false;
  }
  if (!selCropId.value) {
    plantError.value = 'Выберите культуру';
    return false;
  }
  if (selQty.value <= 0) {
    plantError.value = 'Количество должно быть >= 1';
    return false;
  }
  const avail = availableSeedsFor(selCropId.value);
  if (avail < selQty.value) {
    plantError.value = 'Недостаточно семян';
    return false;
  }
  return true;
});

async function onPlant() {
  plantError.value = null;
  if (!canPlant.value) return;
  try {
    emits('plant', { fieldId: props.field.id, cropTypeId: selCropId.value, qty: selQty.value });
  } catch (e: any) {
    plantError.value = e?.message || 'Ошибка посадки';
  }
}

async function onHarvest() {
  harvestError.value = null;
  if (!isReady.value || !props.field.planting) {
    harvestError.value = 'Растение ещё не созрело';
    return;
  }
  harvesting.value = true;
  try {
    emits('harvest', props.field.id);
  } catch (e: any) {
    harvestError.value = e?.message || 'Ошибка сбора';
  } finally {
    harvesting.value = false;
  }
}

// Обнуляем выбор при смене поля
watch(() => props.field.id, () => {
  selCropId.value = 0;
  selQty.value = 1;
  plantError.value = null;
});
</script>

<style scoped>
.field-card {
  border: 1px solid #ddd;
  padding: 12px;
  border-radius: 8px;
  background: #fff;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.slot-header { display:flex; justify-content:space-between; align-items:center; }
.slot-index { font-weight:600; color:#333; }
.slot-status.occupied { color: #b22; font-weight:600; }
.slot-body { display:flex; flex-direction:column; gap:8px; }
.small { font-size:12px; color:#666; }
.qty-input { width:72px; margin-left:8px; }
.btn { padding:6px 10px; border-radius:6px; border:none; cursor:pointer; }
.btn.primary { background:#2b8aef; color:white; }
.btn.success { background:#2bb673; color:white; }
.error { color:#b22; font-size:12px; margin-top:6px; }
</style>