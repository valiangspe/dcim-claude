<script setup lang="ts">
const switches = [
  { name: 'Core-SW-01', total: 48, used: 42, speed: '100G', status: 'active' },
  { name: 'Core-SW-02', total: 48, used: 38, speed: '100G', status: 'active' },
  { name: 'Agg-SW-A1', total: 48, used: 44, speed: '25G', status: 'active' },
  { name: 'Agg-SW-A2', total: 48, used: 40, speed: '25G', status: 'active' },
  { name: 'Agg-SW-B1', total: 48, used: 36, speed: '25G', status: 'active' },
  { name: 'ToR-A01', total: 48, used: 32, speed: '10G', status: 'active' },
  { name: 'ToR-A02', total: 48, used: 28, speed: '10G', status: 'active' },
  { name: 'ToR-B01', total: 48, used: 45, speed: '10G', status: 'warning' },
]
const totalPorts = switches.reduce((a, s) => a + s.total, 0)
const usedPorts = switches.reduce((a, s) => a + s.used, 0)
</script>

<template>
  <div>
    <h4 class="mb-4">Data Ports</h4>
    <div class="row g-3 mb-4">
      <div class="col-md-3"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-primary mb-0">{{ totalPorts }}</h3><small class="text-muted">Total Ports</small></div></div></div>
      <div class="col-md-3"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-warning mb-0">{{ usedPorts }}</h3><small class="text-muted">Used</small></div></div></div>
      <div class="col-md-3"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-success mb-0">{{ totalPorts - usedPorts }}</h3><small class="text-muted">Free</small></div></div></div>
      <div class="col-md-3"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-info mb-0">{{ (usedPorts/totalPorts*100).toFixed(1) }}%</h3><small class="text-muted">Utilization</small></div></div></div>
    </div>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Switch</th><th>Speed</th><th class="text-end">Total</th><th class="text-end">Used</th><th class="text-end">Free</th><th style="width:20%">Utilization</th><th>Status</th></tr></thead>
      <tbody><tr v-for="s in switches" :key="s.name">
        <td class="fw-semibold">{{ s.name }}</td><td><span class="badge bg-secondary">{{ s.speed }}</span></td>
        <td class="text-end">{{ s.total }}</td><td class="text-end">{{ s.used }}</td><td class="text-end">{{ s.total - s.used }}</td>
        <td><div class="d-flex align-items-center gap-2"><div class="progress flex-fill" style="height:8px"><div class="progress-bar" :class="s.used/s.total>0.9?'bg-danger':'bg-primary'" :style="{width:(s.used/s.total*100)+'%'}"></div></div><small>{{ (s.used/s.total*100).toFixed(0) }}%</small></div></td>
        <td><span class="badge" :class="s.status==='active'?'bg-success':'bg-warning text-dark'">{{ s.status }}</span></td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
