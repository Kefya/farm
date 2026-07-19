<template>
  <div class="container">
    <div class="auth-card">
      <h2>Регистрация</h2>
      <form @submit.prevent="onSubmit">
        <label>Логин</label>
        <input v-model="login" required />

        <label>Пароль</label>
        <input v-model="password" type="password" required />

        <div class="actions">
          <button class="btn primary" :disabled="loading">{{ loading ? 'Регистрация...' : 'Зарегистрироваться' }}</button>
          <router-link to="/login">Войти</router-link>
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
    await auth.register(login.value, password.value);
    router.push({ name: 'farm' });
  } catch (e: any) {
    error.value = e.message || 'Ошибка регистрации';
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
/* reuse styles from Login.vue */
</style>