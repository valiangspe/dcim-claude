<script setup lang="ts">
const traces = [
  { id: 'CBL-0001', from: 'Core-SW-01:Eth1/1', to: 'Core-SW-02:Eth1/1', type: 'DAC', length: '3m', color: 'Blue', path: 'Tray A → Cross-connect → Tray B' },
  { id: 'CBL-0002', from: 'Core-SW-01:Eth1/2', to: 'Agg-SW-A1:Eth1/49', type: 'SMF', length: '15m', color: 'Yellow', path: 'Tray A → Conduit 1 → Patch Panel A → Tray C' },
  { id: 'CBL-0003', from: 'Agg-SW-A1:Eth1/1', to: 'ToR-A01:Eth1/49', type: 'MMF', length: '8m', color: 'Orange', path: 'Tray C → Conduit 2 → Tray D' },
  { id: 'CBL-0004', from: 'ToR-A01:Eth1/1', to: 'srv-web-01:eth0', type: 'Cat6A', length: '2m', color: 'Green', path: 'Rack A01 patch → Cabinet top' },
  { id: 'CBL-0005', from: 'ToR-B01:Eth1/48', to: 'srv-gpu-01:eth0', type: 'Cat6A', length: '1.5m', color: 'Red', path: 'Rack B02 patch → Cabinet top' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Cable Tracing</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><div class="table-responsive"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Cable ID</th><th>From</th><th>To</th><th>Type</th><th>Length</th><th>Color</th><th>Path</th></tr></thead>
      <tbody><tr v-for="t in traces" :key="t.id">
        <td class="fw-semibold font-monospace">{{ t.id }}</td>
        <td class="font-monospace small">{{ t.from }}</td><td class="font-monospace small">{{ t.to }}</td>
        <td><span class="badge bg-secondary">{{ t.type }}</span></td><td>{{ t.length }}</td>
        <td><span class="badge" :class="{'bg-primary':t.color==='Blue','bg-warning text-dark':t.color==='Yellow','bg-danger':t.color==='Red','bg-success':t.color==='Green','bg-orange':t.color==='Orange'}" style="background-color: orange;" v-if="t.color==='Orange'">{{ t.color }}</span><span v-else class="badge" :class="{'bg-primary':t.color==='Blue','bg-warning text-dark':t.color==='Yellow','bg-danger':t.color==='Red','bg-success':t.color==='Green'}">{{ t.color }}</span></td>
        <td class="small text-muted">{{ t.path }}</td>
      </tr></tbody>
    </table></div></div></div>
  </div>
</template>
