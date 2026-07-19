import { defineStore } from 'pinia';
import api from '@/api';
import type { Field } from '@/types';
import { ref } from 'vue';

interface FarmState {
  fields: Field[];
  serverNow: string | null; // ISO
  balance: number;
  loading: boolean;
  error: string | null;
  pollTimerId: number | null;
}

export const useFarmStore = defineStore('farm', {
  state: (): FarmState => ({
    fields: [],
    serverNow: null,
    balance: 0,
    loading: false,
    error: null,
    pollTimerId: null
  }),
  actions: {
    async loadFields() {
      this.loading = true; this.error = null;
      try {
        const resp = await api.get('/farm/fields');
        const data = resp.data;
        this.serverNow = data.serverNow;
        this.fields = data.fields;
        this.balance = data.balance;
      } catch (err: any) {
        this.error = err?.response?.data?.message  'Failed to load fields';
      } finally { this.loading = false; }
    },
    startPolling(interval = Number(import.meta.env.VITE_POLL_INTERVAL_MS) 
 10000) {
      if (this.pollTimerId) return;
      this.pollTimerId = window.setInterval(() => {
        this.loadFields();
      }, interval);
    },
    stopPolling() {
      if (this.pollTimerId) {
        clearInterval(this.pollTimerId);
        this.pollTimerId = null;
      }
    },
    async plant(fieldId: number, cropTypeId: number, qty = 1) {
      try {
        const resp = await api.post('/farm/plant', { fieldId, cropTypeId, qty });
        // server returns updated field(s) and balance
        if (resp.data.fields) this.fields = resp.data.fields;
        if (resp.data.balance !== undefined) this.balance = resp.data.balance;
        if (resp.data.serverNow) this.serverNow = resp.data.serverNow;
      } catch (err: any) {
        throw err;
      }
    },
    async harvest(fieldId: number) {
      try {
        const resp = await api.post('/farm/harvest', { fieldId });
        if (resp.data.fields) this.fields = resp.data.fields;
        if (resp.data.balance !== undefined) this.balance = resp.data.balance;
        if (resp.data.serverNow) this.serverNow = resp.data.serverNow;
      } catch (err: any) {
        throw err;
      }
    },
    async buyField(slotIndex?: number) {
      try {
        const resp = await api.post('/shop/buy-field', { slotIndex });
        // get updated fields + balance
        if (resp.data.fields) this.fields = resp.data.fields;
        if (resp.data.balance !== undefined) this.balance = resp.data.balance;
      } catch (err: any) {
        throw err;
      }
    }
  }
});