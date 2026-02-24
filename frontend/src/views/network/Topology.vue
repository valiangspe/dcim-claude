<script setup lang="ts">
const layers = [
  { name: 'Core Layer', devices: ['Core-SW-01', 'Core-SW-02'], connections: 'Full mesh, 100G DAC' },
  { name: 'Aggregation Layer', devices: ['Agg-SW-A1', 'Agg-SW-A2', 'Agg-SW-B1'], connections: 'Dual uplink to Core, 100G SMF' },
  { name: 'Access Layer (Hall A)', devices: ['ToR-A01', 'ToR-A02', 'ToR-A03', 'ToR-A04'], connections: 'Dual 25G uplink to Agg-A' },
  { name: 'Access Layer (Hall B)', devices: ['ToR-B01', 'ToR-B02', 'ToR-B03'], connections: 'Dual 25G uplink to Agg-B' },
  { name: 'Access Layer (Hall C)', devices: ['ToR-C01', 'ToR-C02'], connections: 'Single 25G uplink to Agg-A' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Network Topology</h4>
    <div class="text-center mb-4">
      <div class="d-inline-block border rounded p-3 bg-primary bg-opacity-10 mb-2"><strong>Internet / ISP</strong></div>
      <div class="text-muted">|</div>
      <div v-for="(layer, i) in layers" :key="layer.name" class="mb-2">
        <div class="card border-0 shadow-sm mx-auto" style="max-width:700px">
          <div class="card-body py-2">
            <div class="fw-semibold small text-muted mb-1">{{ layer.name }}</div>
            <div class="d-flex flex-wrap justify-content-center gap-2 mb-1">
              <span v-for="d in layer.devices" :key="d" class="badge bg-dark">{{ d }}</span>
            </div>
            <div class="small text-muted">{{ layer.connections }}</div>
          </div>
        </div>
        <div v-if="i < layers.length - 1" class="text-muted">↕</div>
      </div>
    </div>
  </div>
</template>
