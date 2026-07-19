import { defineStore } from 'pinia';
import api from '@/api';
import type { Field } from '@/types';
import { useAuthStore } from '@/stores/auth';
import { useInventoryStore } from '@/stores/inventory';

const DEFAULT_POLL_MS = Number(import.meta.env.VITE_POLL_INTERVAL_MS || 10000);
const TICK_MS = 1000; // локальный тик для плавного countdown

export const useFarmStore = defineStore('farm', {
  state: () => ({
    fields: [] as Field[],
    serverNow: '' as string,
    loading: false,
    error: null as string | null,
    pollHandle: null as number | null,
    tickHandle: null as number | null
  }),
  getters: {
    isLoaded: (s) => s.fields.length > 0
  },
  actions: {
    async loadFields() {
      this.loading = true;
      this.error = null;
      try {
        const resp = await api.get('/farm/fields');
        // Ожидаем: { serverNow: string, fields: Field[] }
        this.serverNow = resp.data.serverNow;
        this.fields = resp.data.fields;
      } catch (e: any) {
        this.error = e.message;
      } finally {
        this.loading = false;
      }
    },

    startPolling(pollMs = DEFAULT_POLL_MS) {
      // Убедимся, что не запускаем дважды
      if (this.pollHandle) return;
      // Запрашиваем один раз и запускаем интервал синхронизации
      this.loadFields();

      // Проводим polling сервера с заданным интервалом
      this.pollHandle = window.setInterval(() => {
        this.loadFields();
      }, pollMs);

      // Локальный tick каждую секунду для плавного countdown
      this.tickHandle = window.setInterval(() => {
        if (!this.serverNow) return;
        const d = new Date(this.serverNow).getTime() + TICK_MS;
        this.serverNow = new Date(d).toISOString();
      }, TICK_MS);
    },

    stopPolling() {
      if (this.pollHandle) {
        clearInterval(this.pollHandle);
        this.pollHandle = null;
      }
      if (this.tickHandle) {
        clearInterval(this.tickHandle);
        this.tickHandle = null;
      }
    },

    // Посадить культуру: сервер сам посчитает readyAt и снимет семена/баланс
    async plant(fieldId: number, cropTypeId: number, qty = 1) {
      this.loading = true;
      this.error = null;
      try {
        const resp = await api.post('/farm/plant', { fieldId, cropTypeId, qty });
        // Ожидаем обновлённое состояние поля/фермы или хотя бы success
        // best-effort: обновим списки
        await this.loadFields();
        // Обновим инвентарь и баланс
        const inventory = useInventoryStore();
        await inventory.loadSeeds();
        const auth = useAuthStore();
        if (resp.data?.user) auth.setUser(resp.data.user);
      } catch (e: any) {
        this.error = e.message;
        throw e;
      } finally {
        this.loading = false;
      }
    },

    // Сбор урожая: сервер проверит readyAt и harvested flag
    async harvest(fieldId: number) {
      this.loading = true;
      this.error = null;
      try {
        const resp = await api.post('/farm/harvest', { fieldId });
        // Обновляем поля и инвентарь
        await this.loadFields();
        const inventory = useInventoryStore();
        await inventory.loadHarvests();
        const auth = useAuthStore();
        if (resp.data?.user) auth.setUser(resp.data.user);
      } catch (e: any) {
        this.error = e.message;
        throw e;
      } finally {
        this.loading = false;
      }
    },

    // Покупка дополнительного поля (через shop endpoint)
    async buyField(slotIndex: number) {
      this.loading = true;
      this.error = null;
      try {
        const resp = await api.post('/shop/buy-field', { slotIndex });
        await this.loadFields();
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