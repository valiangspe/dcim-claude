<script setup lang="ts">
const pdus = [
  { name: 'PDU-A1', total: 42, used: 36, reserved: 2, type: 'C13/C19' },
  { name: 'PDU-A2', total: 42, used: 33, reserved: 3, type: 'C13/C19' },
  { name: 'PDU-B1', total: 42, used: 38, reserved: 1, type: 'C13/C19' },
  { name: 'PDU-B2', total: 42, used: 29, reserved: 4, type: 'C13/C19' },
  { name: 'PDU-C1', total: 42, used: 18, reserved: 0, type: 'C13' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Power Ports</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>PDU</th><th>Connector</th><th class="text-end">Total</th><th class="text-end">Used</th><th class="text-end">Reserved</th><th class="text-end">Free</th><th style="width:20%">Utilization</th></tr></thead>
      <tbody><tr v-for="p in pdus" :key="p.name">
        <td class="fw-semibold">{{ p.name }}</td><td><span class="badge bg-secondary">{{ p.type }}</span></td>
        <td class="text-end">{{ p.total }}</td><td class="text-end">{{ p.used }}</td><td class="text-end">{{ p.reserved }}</td><td class="text-end">{{ p.total-p.used-p.reserved }}</td>
        <td><div class="progress" style="height:8px"><div class="progress-bar bg-primary" :style="{width:(p.used/p.total*100)+'%'}"></div><div class="progress-bar bg-warning" :style="{width:(p.reserved/p.total*100)+'%'}"></div></div></td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
