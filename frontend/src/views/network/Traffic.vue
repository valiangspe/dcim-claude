<script setup lang="ts">
const hourly = [
  { time: '00:00', inbound: 98, outbound: 82, total: 180 },
  { time: '04:00', inbound: 72, outbound: 65, total: 137 },
  { time: '08:00', inbound: 145, outbound: 118, total: 263 },
  { time: '12:00', inbound: 168, outbound: 142, total: 310 },
  { time: '16:00', inbound: 155, outbound: 135, total: 290 },
  { time: '20:00', inbound: 132, outbound: 110, total: 242 },
]
const max = 400
</script>

<template>
  <div>
    <h4 class="mb-4">Network Traffic</h4>
    <div class="row g-3 mb-4">
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-success bg-opacity-10"><div class="card-body"><h3 class="text-success mb-0">142 Gbps</h3><small class="text-muted">Inbound</small></div></div></div>
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10"><div class="card-body"><h3 class="text-primary mb-0">105 Gbps</h3><small class="text-muted">Outbound</small></div></div></div>
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-info bg-opacity-10"><div class="card-body"><h3 class="text-info mb-0">247 Gbps</h3><small class="text-muted">Total</small></div></div></div>
    </div>
    <div class="card border-0 shadow-sm"><div class="card-header bg-transparent fw-semibold">24h Traffic Profile (Gbps)</div><div class="card-body p-0"><table class="table table-hover mb-0">
      <thead class="table-light"><tr><th>Time</th><th class="text-end">In</th><th class="text-end">Out</th><th class="text-end">Total</th><th style="width:35%">Bar</th></tr></thead>
      <tbody><tr v-for="h in hourly" :key="h.time">
        <td>{{ h.time }}</td><td class="text-end text-success">{{ h.inbound }}</td><td class="text-end text-primary">{{ h.outbound }}</td><td class="text-end fw-semibold">{{ h.total }}</td>
        <td><div class="progress" style="height:12px"><div class="progress-bar bg-success" :style="{width:(h.inbound/max*100)+'%'}"></div><div class="progress-bar bg-primary" :style="{width:(h.outbound/max*100)+'%'}"></div></div></td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
