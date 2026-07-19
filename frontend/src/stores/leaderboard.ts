import { defineStore } from 'pinia';
import api from '@/api';

interface LeaderItem {
  place: number;
  login: string;
  balance: number;
  extra: number; // candidate-defined metric
}

interface LeaderState {
  list: LeaderItem[];
  loading: boolean;
  error: string | null;
}

export const useLeaderboardStore = defineStore('leaderboard', {
  state: (): LeaderState => ({
    list: [],
    loading: false,
    error: null
  }),
  actions: {
    async loadTop(limit = 10) {
      this.loading = true; this.error = null;
      try {
        const resp = await api.get('/leaderboard/top', { params: { limit }});
        this.list = resp.data;
      } catch (err: any) {
        this.error = err?.response?.data?.message || 'Failed to load leaderboard';
      } finally { this.loading = false; }
    }
  }
});