<template>
  <nav class="navbar">
    <div class="left">
      <router-link to="/" class="brand">Мини‑ферма</router-link>
      <router-link to="/shop" class="nav-link">Магазин</router-link>
      <router-link to="/inventory" class="nav-link">Инвентарь</router-link>
      <router-link to="/leaderboard" class="nav-link">Таблица лидеров</router-link>
    </div>

    <div class="right">
      <div v-if="isAuthenticated" class="user-info">
        <span class="balance">Баланс: <strong>{{ user?.balance ?? 0 }}$</strong></span>
        <button class="btn link" @click="onLogout">Выйти</button>
      </div>
      <div v-else>
        <router-link to="/login" class="nav-link">Войти</router-link>
        <router-link to="/register" class="nav-link">Регистрация</router-link>
      </div>
    </div>
  </nav>
</template>

<script lang="ts" setup>
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';

const auth = useAuthStore();
const { user } = storeToRefs(auth);

const isAuthenticated = auth.isAuthenticated;

function onLogout() {
  auth.logout();
}
</script>

<style scoped>
.navbar { display:flex; justify-content:space-between; align-items:center; padding:12px 16px; background:#fafafa; border-bottom:1px solid #eee; }
.left, .right { display:flex; gap:12px; align-items:center; }
.brand { font-weight:700; font-size:16px; color:#2b8aef; }
.nav-link { color:#333; text-decoration:none; }
.user-info { display:flex; gap:12px; align-items:center; }
.btn.link { background:none; border:none; color:#b22; cursor:pointer; }
.balance { color:#333; }
</style>