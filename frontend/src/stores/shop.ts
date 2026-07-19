import { defineStore } from 'pinia';
import api from '@/api';
import type { CropType } from '@/types';

interface ShopState {
  crops: CropType[];
  loading: boolean;
  error: string | null;
}

export const useShopStore = defineStore('shop', {
  state: (): ShopState => ({
    crops: [],
    loading: false,
    error: null
  }),
  actions: {
    async loadCrops() {
      this.loading = true; this.error = null;
      try {
        const resp = await api.get('/shop/crops');
        this.crops = resp.data;
      } catch (err: any) {
        this.error = err?.response?.data?.message || 'Failed to load crops';
      } finally { this.loading = false; }
    },
    async buySeeds(cropTypeId: number, qty: number) {
      try {
        const resp = await api.post('/shop/buy-seeds', { cropTypeId, qty });
        // server returns updated balance and inventory (optional)
        return resp.data;
      } catch (err: any) {
        throw err;
      }
    },
    async buyField(slotIndex?: number) {
      try {
        const resp = await api.post('/shop/buy-field', { slotIndex });
        return resp.data;
      } catch (err: any) {
        throw err;
      }
    }
  }
});