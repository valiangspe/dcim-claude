<script setup lang="ts">
import { ref, computed } from 'vue'

interface Device {
  name: string
  startU: number
  sizeU: number
  type: string
  color: string
}

const selectedRack = ref('CAB-A01')

const rackData: Record<string, { totalU: number; devices: Device[] }> = {
  'CAB-A01': {
    totalU: 42,
    devices: [
      { name: 'Patch Panel', startU: 42, sizeU: 1, type: 'Network', color: 'bg-info' },
      { name: 'Switch-A1', startU: 41, sizeU: 1, type: 'Network', color: 'bg-info' },
      { name: 'Switch-A2', startU: 40, sizeU: 1, type: 'Network', color: 'bg-info' },
      { name: 'srv-web-01', startU: 38, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-02', startU: 36, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-03', startU: 34, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-04', startU: 32, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-05', startU: 30, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-06', startU: 28, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-07', startU: 26, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-08', startU: 24, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-09', startU: 22, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-10', startU: 20, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-11', startU: 18, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'srv-web-12', startU: 16, sizeU: 2, type: 'Server', color: 'bg-primary' },
      { name: 'UPS-Rack-A1', startU: 10, sizeU: 5, type: 'UPS', color: 'bg-warning' },
      { name: 'PDU-A1-L', startU: 1, sizeU: 9, type: 'PDU', color: 'bg-secondary' },
    ],
  },
  'CAB-B02': {
    totalU: 42,
    devices: [
      { name: 'Patch Panel', startU: 42, sizeU: 1, type: 'Network', color: 'bg-info' },
      { name: 'Switch-B2', startU: 41, sizeU: 1, type: 'Network', color: 'bg-info' },
      { name: 'srv-gpu-01', startU: 37, sizeU: 4, type: 'GPU Server', color: 'bg-danger' },
      { name: 'srv-gpu-02', startU: 33, sizeU: 4, type: 'GPU Server', color: 'bg-danger' },
      { name: 'srv-gpu-03', startU: 29, sizeU: 4, type: 'GPU Server', color: 'bg-danger' },
      { name: 'srv-gpu-04', startU: 25, sizeU: 4, type: 'GPU Server', color: 'bg-danger' },
      { name: 'srv-gpu-05', startU: 21, sizeU: 4, type: 'GPU Server', color: 'bg-danger' },
      { name: 'srv-gpu-06', startU: 17, sizeU: 4, type: 'GPU Server', color: 'bg-danger' },
      { name: 'srv-gpu-07', startU: 13, sizeU: 4, type: 'GPU Server', color: 'bg-danger' },
      { name: 'srv-gpu-08', startU: 9, sizeU: 4, type: 'GPU Server', color: 'bg-danger' },
      { name: 'PDU-B2', startU: 1, sizeU: 8, type: 'PDU', color: 'bg-secondary' },
    ],
  },
}

const rackOptions = Object.keys(rackData)

interface SlotRow {
  u: number
  device: Device | null
  isTop: boolean
}

const slots = computed((): SlotRow[] => {
  const data = rackData[selectedRack.value]
  if (!data) return []
  const result: SlotRow[] = []
  for (let u = data.totalU; u >= 1; u--) {
    const dev = data.devices.find(d => u >= d.startU && u < d.startU + d.sizeU) ?? null
    const isTop = dev ? u === dev.startU + dev.sizeU - 1 : false
    result.push({ u, device: dev, isTop })
  }
  return result
})

const devices = computed(() => rackData[selectedRack.value]?.devices ?? [])
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Rack Elevation</h4>
      <select v-model="selectedRack" class="form-select" style="width: 200px;">
        <option v-for="r in rackOptions" :key="r" :value="r">{{ r }}</option>
      </select>
    </div>

    <div class="row">
      <div class="col-lg-6">
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-transparent fw-semibold">{{ selectedRack }} — Front View</div>
          <div class="card-body p-0">
            <table class="table table-bordered table-sm mb-0" style="table-layout: fixed;">
              <thead class="table-dark">
                <tr><th style="width:50px" class="text-center">U</th><th>Device</th></tr>
              </thead>
              <tbody>
                <tr v-for="slot in slots" :key="slot.u" style="height: 24px;">
                  <td class="align-middle text-center small fw-semibold">{{ slot.u }}</td>
                  <td
                    v-if="slot.device"
                    :class="[slot.device.color, 'text-white', 'align-middle', 'small', 'fw-semibold']"
                    :style="slot.isTop ? {} : { borderTop: 'none' }"
                  >
                    <template v-if="slot.isTop">{{ slot.device.name }} ({{ slot.device.sizeU }}U)</template>
                  </td>
                  <td v-else class="bg-light text-muted small align-middle text-center">—</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-transparent fw-semibold">Device List</div>
          <div class="card-body p-0">
            <table class="table table-hover mb-0">
              <thead class="table-light">
                <tr><th>Device</th><th>Type</th><th class="text-end">Start U</th><th class="text-end">Size</th></tr>
              </thead>
              <tbody>
                <tr v-for="d in devices" :key="d.name">
                  <td class="fw-semibold">{{ d.name }}</td>
                  <td><span class="badge" :class="d.color">{{ d.type }}</span></td>
                  <td class="text-end">U{{ d.startU }}</td>
                  <td class="text-end">{{ d.sizeU }}U</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
