import { describe, it, expect } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useInventoryStore } from '../src/stores/inventory.ts' // путь к вашему store

describe('Inventory store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('sells crop and increases balance', async () => {
    const store = useInventoryStore()
    store.crops = { '1': 2 } // cropTypeId: quantity
    store.balance = 10

    // Метод sellCrop ожидается: (cropTypeId, qty) -> проверяет qty и обновляет state
    await store.sellCrop('1', 1)

    expect(store.crops['1']).toBe(1)
    expect(store.balance).toBeGreaterThan(10)
  })
})