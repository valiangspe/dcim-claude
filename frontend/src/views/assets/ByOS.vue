<script setup lang="ts">
const osList = [
  { os: 'Ubuntu 22.04 LTS', count: 48, pct: 38.7 },
  { os: 'RHEL 9', count: 28, pct: 22.6 },
  { os: 'Debian 12', count: 18, pct: 14.5 },
  { os: 'Windows Server 2022', count: 14, pct: 11.3 },
  { os: 'Ubuntu 24.04 LTS', count: 8, pct: 6.5 },
  { os: 'FreeBSD 14', count: 4, pct: 3.2 },
  { os: 'VMware ESXi 8', count: 4, pct: 3.2 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Hosts by OS</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Operating System</th><th class="text-end">Count</th><th style="width:40%">Distribution</th><th class="text-end">%</th></tr></thead>
      <tbody><tr v-for="o in osList" :key="o.os"><td class="fw-semibold">{{ o.os }}</td><td class="text-end">{{ o.count }}</td>
        <td><div class="progress" style="height:10px"><div class="progress-bar bg-primary" :style="{width:o.pct+'%'}"></div></div></td><td class="text-end">{{ o.pct }}%</td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
