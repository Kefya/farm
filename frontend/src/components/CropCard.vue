<template>
  <div class="crop-card">
    <div class="crop-title">{{ crop.name }}</div>
    <div class="crop-meta">
      <div>Цена семян: <strong>{{ crop.seedPrice }}$</strong></div>
      <div>Время созр.: <strong>{{ formatTime(crop.growDurationSeconds) }}</strong></div>
      <div>Цена продажи: <strong>{{ crop.sellPrice }}$</strong></div>
    </div>

    <div class="buy-row">
      <input type="number" min="1" v-model.number="qty" class="qty-input" />
      <button
        class="btn primary"
        :disabled="!canBuy"
        @click="onBuy"
      >
        Купить ({{ totalPrice }}$)
      </button>
    </div>

    <div v-if="error" class="error">{{ error }}</div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue';
import type { CropType } from '@/types';
import { formatDuration } from '@/utils/formatDuration';

const props = defineProps<{
  crop: CropType;
  userBalance?: number;
}>();

const emits = defineEmits<{
  (e: 'buy-seeds', payload: { cropTypeId: number; qty: number }): void;
}>();

const qty = ref<number>(1);
const error = ref<string | null>(null);

const totalPrice = computed(() => qty.value * props.crop.seedPrice);
const canBuy = computed(() => {
  error.value = null;
  if (qty.value <= 0) {
    error.value = 'Количество должно быть >= 1';
    return false;
  }
  if (props.userBalance !== undefined && props.userBalance < totalPrice.value) {
    error.value = 'Недостаточно средств';
    return false;
  }
  return true;
});

function onBuy() {
  if (!canBuy.value) return;
  emits('buy-seeds', { cropTypeId: props.crop.id, qty: qty.value });
}

function formatTime(sec: number) {
  return formatDuration(sec * 1000);
}
</script>

<style scoped>
.crop-card { border:1px solid #e6e6e6; padding:12px; border-radius:8px; background:#fff; display:flex; flex-direction:column; gap:8px; }
.crop-title { font-weight:700; }
.crop-meta { font-size:13px; color:#444; display:flex; gap:12px; flex-wrap:wrap; }
.buy-row { display:flex; gap:8px; align-items:center; }
.qty-input { width:72px; }
.btn.primary { background:#2b8aef; color:#fff; border:none; padding:6px 10px; border-radius:6px; cursor:pointer; }
.error { color:#b22; font-size:12px; }
</style>