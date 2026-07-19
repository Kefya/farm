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

export interface LeaderboardEntry {
  place: number;
  login: string;
  balance: number;
  extraMetric: number;
}