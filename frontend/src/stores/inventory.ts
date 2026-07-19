import { defineStore } from 'pinia';
import api from '@/api';
import type { SeedInventoryItem, HarvestInventoryItem } from '@/types';
import { useAuthStore } from '@/stores/auth';

export const useInventoryStore = defineStore('inventory', {
  state: () => ({
    seeds: [] as SeedInventoryItem[],
    harvests: [] as HarvestInventoryItem[],
    loading: false,
    error: null as string | null
  }),
  actions: {
    async loadSeeds() {
      this.loading = true;
      this.error = null;
      try {
        const resp = await api.get('/inventory/seeds');
        this.seeds = resp.data as SeedInventoryItem[];
      } catch (e: any) {
        this.error = e.message;
      } finally {
        this.loading = false;
      }
    },

    async loadHarvests() {
      this.loading = true;
      this.error = null;
      try {
        const resp = await api.get('/inventory/harvests');
        this.harvests = resp.data as HarvestInventoryItem[];
      } catch (e: any) {
        this.error = e.message;
      } finally {
        this.loading = false;
      }
    },

    // Продать выращенную культуру
    async sell(cropTypeId: number, qty = 1) {
      this.loading = true;
      this.error = null;
      try {
        const resp = await api.post('/inventory/sell', { cropTypeId, qty });
        // Обновляем инвентарь и баланс
        await this.loadHarvests();
        await this.loadSeeds();
        const auth = useAuthStore();
        if (resp.data?.user) auth.setUser(resp.data.user);
      } catch (e: any) {
        this.error = e.message;
        throw e;
      } finally {
        this.loading = false;
      }
    }
  }
});