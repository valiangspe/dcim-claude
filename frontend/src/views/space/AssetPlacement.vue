<script setup lang="ts">
import { ref } from 'vue'

const selectedRack = ref('CAB-B03')

interface Slot {
  u: number
  device: string | null
  size: number
}

const rackSlots: Record<string, Slot[]> = {
  'CAB-B03': [
    { u: 42, device: 'Switch-B3', size: 1 },
    { u: 41, device: 'Patch Panel', size: 1 },
    { u: 40, device: null, size: 1 },
    { u: 39, device: null, size: 1 },
    { u: 38, device: 'srv-app-05', size: 2 },
    { u: 36, device: 'srv-app-06', size: 2 },
    { u: 34, device: null, size: 1 },
    { u: 33, device: null, size: 1 },
    { u: 32, device: null, size: 1 },
    { u: 31, device: null, size: 1 },
    { u: 30, device: 'srv-app-07', size: 2 },
    { u: 28, device: 'srv-app-08', size: 2 },
    { u: 26, device: null, size: 1 },
    { u: 25, device: null, size: 1 },
    { u: 24, device: null, size: 1 },
    { u: 23, device: null, size: 1 },
    { u: 22, device: null, size: 1 },
    { u: 21, device: null, size: 1 },
    { u: 20, device: null, size: 1 },
    { u: 19, device: null, size: 1 },
    { u: 18, device: 'srv-app-09', size: 2 },
    { u: 16, device: 'srv-app-10', size: 2 },
    { u: 14, device: 'srv-app-11', size: 2 },
    { u: 12, device: null, size: 1 },
    { u: 11, device: null, size: 1 },
    { u: 10, device: null, size: 1 },
    { u: 9, device: null, size: 1 },
    { u: 8, device: null, size: 1 },
    { u: 7, device: null, size: 1 },
    { u: 6, device: null, size: 1 },
    { u: 5, device: null, size: 1 },
    { u: 4, device: 'PDU-B3', size: 4 },
  ],
}

const pendingAssets = [
  { name: 'srv-ml-01', size: 4, type: 'GPU Server' },
  { name: 'srv-ml-02', size: 4, type: 'GPU Server' },
  { name: 'nas-expand-01', size: 2, type: 'Storage' },
]

const slots = rackSlots[selectedRack.value] ?? []
const freeU = slots.filter(s => !s.device).length
</script>

<template>
  <div>
    <h4 class="mb-4">Asset Placement</h4>

    <div class="row g-3">
      <div class="col-lg-4">
        <div class="card border-0 shadow-sm mb-3">
          <div class="card-header bg-transparent fw-semibold">Pending Assets</div>
          <div class="card-body">
            <div v-for="a in pendingAssets" :key="a.name" class="border rounded p-2 mb-2 d-flex justify-content-between align-items-center" style="cursor: grab;">
              <div>
                <span class="fw-semibold">{{ a.name }}</span>
                <br><small class="text-muted">{{ a.type }} &middot; {{ a.size }}U</small>
              </div>
              <span class="badge bg-primary">{{ a.size }}U</span>
            </div>
            <small class="text-muted">Drag assets to an empty slot in the rack.</small>
          </div>
        </div>

        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h4 class="text-success mb-0">{{ freeU }}U</h4>
            <small class="text-muted">Free slots in {{ selectedRack }}</small>
          </div>
        </div>
      </div>

      <div class="col-lg-8">
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-transparent fw-semibold">{{ selectedRack }} — Rack View</div>
          <div class="card-body p-0">
            <table class="table table-bordered table-sm mb-0 text-center" style="table-layout: fixed;">
              <thead class="table-dark">
                <tr><th style="width:50px">U</th><th>Device</th></tr>
              </thead>
              <tbody>
                <tr v-for="slot in slots" :key="slot.u" :class="slot.device ? '' : 'table-success'" style="height: 26px;">
                  <td class="small fw-semibold align-middle">{{ slot.u }}</td>
                  <td v-if="slot.device" class="bg-primary bg-opacity-75 text-white small align-middle fw-semibold">{{ slot.device }}</td>
                  <td v-else class="small align-middle text-success">[ empty ]</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
