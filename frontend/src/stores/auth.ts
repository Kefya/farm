import { defineStore } from 'pinia';
import api from '@/api';
import type { User } from '@/types';

interface AuthState {
  token: string | null;
  user: User | null;
  loading: boolean;
  error: string | null;
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: localStorage.getItem('token'),
    user: localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!) : null,
    loading: false,
    error: null
  }),
  getters: {
    isAuthenticated: (s) => !!s.token
  },
  actions: {
    setSession(token: string | null, user: User | null) {
      this.token = token;
      this.user = user;
      if (token) {
        localStorage.setItem('token', token);
      } else {
        localStorage.removeItem('token');
      }
      if (user) {
        localStorage.setItem('user', JSON.stringify(user));
      } else {
        localStorage.removeItem('user');
      }
    },
    async login(login: string, password: string) {
      this.loading = true; this.error = null;
      try {
        const resp = await api.post('/auth/login', { login, password });
        const { token, user } = resp.data;
        this.setSession(token, user);
      } catch (err: any) {
        this.error = err?.response?.data?.message  'Login failed';
        throw err;
      } finally { this.loading = false; }
    },
    async register(login: string, password: string) {
      this.loading = true; this.error = null;
      try {
        const resp = await api.post('/auth/register', { login, password });
        const { token, user } = resp.data;
        this.setSession(token, user);
      } catch (err: any) {
        this.error = err?.response?.data?.message  'Register failed';
        throw err;
      } finally { this.loading = false; }
    },
    logout() {
      this.setSession(null, null);
    }
  }
});