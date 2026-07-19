import { defineStore } from 'pinia';
import api from '@/api';

interface SeedItem {
  cropTypeId: number;
  name: string;
  quantity: number;
}

interface HarvestItem {
  cropTypeId: number;
  name: string;
  quantity: number;
}

interface InventoryState {
  seeds: SeedItem[];
  harvests: HarvestItem[];
  loading: boolean;
  error: string | null;
}

export const useInventoryStore = defineStore('inventory', {
  state: (): InventoryState => ({
    seeds: [],
    harvests: [],
    loading: false,
    error: null
  }),
  actions: {
    async loadInventory() {
      this.loading = true; this.error = null;
      try {
        const [s1, s2] = await Promise.all([api.get('/inventory/seeds'), api.get('/inventory/harvests')]);
        this.seeds = s1.data;
        this.harvests = s2.data;
      } catch (err: any) {
        this.error = err?.response?.data?.message || 'Failed to load inventory';
      } finally { this.loading = false; }
    },
    async sellHarvest(cropTypeId: number, qty: number) {
      try {
        const resp = await api.post('/inventory/sell', { cropTypeId, qty });
        // server returns updated inventory and balance
        if (resp.data.harvests) this.harvests = resp.data.harvests;
        return resp.data;
      } catch (err: any) {
        throw err;
      }
    }
  }
});