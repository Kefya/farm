export interface User {
  id: number;
  login: string;
  balance: number;
  totalEarned?: number;
}

export interface CropType {
  id: number;
  name: string;
  seedPrice: number;
  growDurationSeconds: number;
  sellPrice: number;
}

export interface Field {
  id: number;
  slotIndex: number;
  planted: boolean;
  cropTypeId?: number | null;
  cropName?: string | null;
  plantedAt?: string | null; // ISO
  readyAt?: string | null;   // ISO
  harvested?: boolean;
  purchasedAt?: string | null;
}

export interface ServerFieldsResponse {
  serverNow: string; // ISO
  fields: Field[];
  balance: number;
}
import dayjs from 'dayjs';

export function remainingMs(serverNowIso: string, targetIso: string): number {
  return new Date(targetIso).getTime() - new Date(serverNowIso).getTime();
}

export function formatMs(ms: number): string {
  if (ms <= 0) return '00:00';
  const total = Math.floor(ms / 1000);
  const minutes = Math.floor(total / 60);
  const seconds = total % 60;
  return '${String(minutes).padStart(2,'0')}:${String(seconds).padStart(2,'0')}';
}