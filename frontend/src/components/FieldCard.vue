<template>
  <div class="field-card">
    <div v-if="!field.planted">Свободно</div>
    <div v-else>
      <div>{{ field.cropName }}</div>
      <div v-if="!isReady">Осталось: {{ remaining }}</div>
      <div v-else><button @click="harvest">Собрать</button></div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, computed } from 'vue';
import { formatDuration } from '@/utils/time';
export default defineComponent({
  props: { field: Object, serverNow: String },
  emits: ['harvest', 'plant'],
  setup(props, { emit }) {
    const remaining = computed(() => {
      if (!props.field.planted) return null;
      const ready = new Date(props.field.readyAt).getTime();
      const server = new Date(props.serverNow).getTime();
      const leftMs = ready - server;
      return leftMs > 0 ? formatDuration(leftMs) : 'Готово';
    });
    const isReady = computed(() => new Date(props.field.readyAt).getTime() <= new Date(props.serverNow).getTime());
    const harvest = () => emit('harvest', props.field.id);
    return { remaining, isReady, harvest };
  }
});
</script>