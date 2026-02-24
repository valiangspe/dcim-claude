<script setup lang="ts">
const servers = [
  { name: 'srv-web-01', cpu: 67, gpu: 0, mem: 58, disk: 42, netIn: 1.2, netOut: 0.8, power: 0.42 },
  { name: 'srv-web-02', cpu: 54, gpu: 0, mem: 52, disk: 38, netIn: 0.9, netOut: 0.6, power: 0.38 },
  { name: 'srv-db-01', cpu: 82, gpu: 0, mem: 78, disk: 72, netIn: 2.1, netOut: 1.8, power: 0.81 },
  { name: 'srv-db-02', cpu: 78, gpu: 0, mem: 74, disk: 68, netIn: 1.9, netOut: 1.6, power: 0.76 },
  { name: 'srv-gpu-01', cpu: 45, gpu: 91, mem: 82, disk: 28, netIn: 4.2, netOut: 3.8, power: 2.40 },
  { name: 'srv-gpu-02', cpu: 42, gpu: 89, mem: 80, disk: 25, netIn: 3.9, netOut: 3.5, power: 2.35 },
  { name: 'srv-app-01', cpu: 45, gpu: 0, mem: 48, disk: 35, netIn: 0.6, netOut: 0.4, power: 0.55 },
  { name: 'srv-stor-01', cpu: 22, gpu: 0, mem: 34, disk: 88, netIn: 3.2, netOut: 2.8, power: 0.68 },
]
function metricClass(v: number) { return v >= 85 ? 'text-danger fw-semibold' : v >= 70 ? 'text-warning' : '' }
</script>

<template>
  <div>
    <h4 class="mb-4">Server Metrics</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><div class="table-responsive"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Server</th><th class="text-end">CPU %</th><th class="text-end">GPU %</th><th class="text-end">Mem %</th><th class="text-end">Disk %</th><th class="text-end">Net In (Gbps)</th><th class="text-end">Net Out (Gbps)</th><th class="text-end">Power (kW)</th></tr></thead>
      <tbody><tr v-for="s in servers" :key="s.name">
        <td class="fw-semibold font-monospace">{{ s.name }}</td>
        <td class="text-end" :class="metricClass(s.cpu)">{{ s.cpu }}</td>
        <td class="text-end" :class="s.gpu?metricClass(s.gpu):''">{{ s.gpu||'---' }}</td>
        <td class="text-end" :class="metricClass(s.mem)">{{ s.mem }}</td>
        <td class="text-end" :class="metricClass(s.disk)">{{ s.disk }}</td>
        <td class="text-end">{{ s.netIn }}</td><td class="text-end">{{ s.netOut }}</td>
        <td class="text-end">{{ s.power.toFixed(2) }}</td>
      </tr></tbody>
    </table></div></div></div>
  </div>
</template>
