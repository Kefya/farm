<template>
  <div class="container">
    <Navbar />
    <section class="leader-section">
      <h2>Таблица лидеров</h2>
      <div v-if="loading" class="info">Загрузка...</div>
      <div v-if="error" class="error">{{ error }}</div>

      <table v-if="entries.length" class="leader-table">
        <thead>
          <tr><th>Место</th><th>Логин</th><th>Баланс</th><th>Доп. метрика</th></tr>
        </thead>
        <tbody>
          <tr v-for="e in entries" :key="e.place">
            <td>{{ e.place }}</td>
            <td>{{ e.login }}</td>
            <td>{{ e.balance }}$</td>
            <td>{{ e.extraMetric }}</td>
          </tr>
        </tbody>
      </table>
      <div v-else class="info">Нет данных.</div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import Navbar from '@/components/Navbar.vue';
import { useLeaderboardStore } from '@/stores/leaderboard';
import { onMounted } from 'vue';

const lb = useLeaderboardStore();
const entries = lb.entries;
const loading = lb.loading;
const error = lb.error;

onMounted(async () => {
  await lb.loadTop(10);
});
</script>

<style scoped>
.leader-table { width:100%; border-collapse:collapse; margin-top:12px; }
.leader-table th, .leader-table td { padding:8px; border-bottom:1px solid #eee; text-align:left; }
</style>