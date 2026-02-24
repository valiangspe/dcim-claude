<script setup lang="ts">
const phases = [
  { phase: 'Procurement', count: 8, items: 'DGX H100 x4, R760 x4', eta: 'Mar 2026' },
  { phase: 'Staging', count: 4, items: 'Arista 7280 x2, NAS x2', eta: 'Feb 2026' },
  { phase: 'Deployment', count: 12, items: 'R750 batch C', eta: 'This week' },
  { phase: 'Production', count: 218, items: 'All active assets', eta: '---' },
  { phase: 'End of Life', count: 6, items: 'R630 x4, Nexus 3048 x2', eta: 'Decom Q2 2026' },
  { phase: 'Decommission', count: 8, items: 'R620 x6, SAN old x2', eta: 'Pending disposal' },
]
function phaseColor(p: string) { return p==='Production'?'bg-success':p==='Deployment'?'bg-primary':p==='Staging'?'bg-info text-dark':p==='Procurement'?'bg-warning text-dark':p==='End of Life'?'bg-danger':'bg-secondary' }
</script>

<template>
  <div>
    <h4 class="mb-4">Asset Lifecycle</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Phase</th><th class="text-end">Assets</th><th>Items</th><th>ETA / Notes</th></tr></thead>
      <tbody><tr v-for="p in phases" :key="p.phase">
        <td><span class="badge" :class="phaseColor(p.phase)">{{ p.phase }}</span></td>
        <td class="text-end fw-semibold">{{ p.count }}</td><td class="text-muted small">{{ p.items }}</td><td class="small">{{ p.eta }}</td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
