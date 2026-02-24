<script setup lang="ts">
const kpis = [
  { label: 'Total Bandwidth', value: '400', unit: 'Gbps', color: 'primary' },
  { label: 'Current Throughput', value: '247', unit: 'Gbps', color: 'info' },
  { label: 'Uptime', value: '99.97', unit: '%', color: 'success' },
  { label: 'Active IPs', value: '12,480', unit: '', color: 'dark' },
  { label: 'Redundancy', value: 'Dual', unit: '', color: 'secondary' },
  { label: 'Active Alerts', value: '3', unit: '', color: 'danger' },
]
const links = [
  { name: 'ISP-A (Primary)', capacity: 200, usage: 142, latency: 1.2, loss: 0.01, status: 'active' },
  { name: 'ISP-B (Secondary)', capacity: 200, usage: 105, latency: 1.8, loss: 0.02, status: 'active' },
  { name: 'DC-East ↔ DC-West', capacity: 100, usage: 38, latency: 18.4, loss: 0.00, status: 'active' },
  { name: 'DC-East ↔ DC-EU', capacity: 50, usage: 22, latency: 82.1, loss: 0.03, status: 'active' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Network Overview</h4>
    <div class="row g-3 mb-4">
      <div v-for="k in kpis" :key="k.label" class="col-sm-6 col-lg-4 col-xl-2">
        <div class="card border-0 shadow-sm h-100"><div class="card-body text-center">
          <h3 :class="`text-${k.color} mb-0`">{{ k.value }}<small class="text-muted fs-6">{{ k.unit }}</small></h3>
          <small class="text-muted">{{ k.label }}</small>
        </div></div>
      </div>
    </div>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Uplinks & Inter-DC Links</div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
        <thead class="table-light"><tr><th>Link</th><th class="text-end">Capacity (Gbps)</th><th class="text-end">Usage (Gbps)</th><th style="width:18%">Utilization</th><th class="text-end">Latency (ms)</th><th class="text-end">Loss %</th><th>Status</th></tr></thead>
        <tbody><tr v-for="l in links" :key="l.name">
          <td class="fw-semibold">{{ l.name }}</td>
          <td class="text-end">{{ l.capacity }}</td><td class="text-end">{{ l.usage }}</td>
          <td><div class="d-flex align-items-center gap-2"><div class="progress flex-fill" style="height:8px"><div class="progress-bar bg-primary" :style="{width:(l.usage/l.capacity*100)+'%'}"></div></div><small>{{ (l.usage/l.capacity*100).toFixed(0) }}%</small></div></td>
          <td class="text-end">{{ l.latency }}</td><td class="text-end">{{ l.loss.toFixed(2) }}%</td>
          <td><span class="badge bg-success">{{ l.status }}</span></td>
        </tr></tbody>
      </table></div>
    </div>
  </div>
</template>
