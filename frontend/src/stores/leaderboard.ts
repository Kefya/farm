import { defineStore } from 'pinia';
import api from '@/api';
import type { LeaderboardEntry } from '@/types';

export const useLeaderboardStore = defineStore('leaderboard', {
  state: () => ({
    entries: [] as LeaderboardEntry[],
    loading: false,
    error: null as string | null
  }),
  actions: {
    async loadTop(limit = 10) {
      this.loading = true;
      this.error = null;
      try {
        const resp = await api.get('/leaderboard/top?limit=${limit}');
        this.entries = resp.data as LeaderboardEntry[];
      } catch (e: any) {
        this.error = e.message;
      } finally {
        this.loading = false;
      }
    }
  }
});