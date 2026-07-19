<template>
  <div class="container">
    <div class="auth-card">
      <h2>Вход</h2>
      <form @submit.prevent="onSubmit">
        <label>Логин</label>
        <input v-model="login" required />

        <label>Пароль</label>
        <input v-model="password" type="password" required />

        <div class="actions">
          <button class="btn primary" :disabled="loading">{{ loading ? 'Вход...' : 'Войти' }}</button>
          <router-link to="/register">Регистрация</router-link>
        </div>

        <div v-if="error" class="error">{{ error }}</div>
      </form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';

const auth = useAuthStore();
const router = useRouter();

const login = ref('');
const password = ref('');
const loading = ref(false);
const error = ref<string | null>(null);

async function onSubmit() {
  loading.value = true;
  error.value = null;
  try {
    await auth.login(login.value, password.value);
    // После успешного входа — переходим на ферму
    router.push({ name: 'farm' });
  } catch (e: any) {
    error.value = e.message || 'Ошибка входа';
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
.auth-card { max-width:420px; margin:60px auto; padding:20px; background:#fff; border-radius:8px; }
.auth-card input { width:100%; padding:8px; margin:6px 0 12px; }
.actions { display:flex; justify-content:space-between; align-items:center; gap:12px; }
</style>