<script setup lang="ts">
const groups = [
  { name: 'Web Servers', racks: ['CAB-A01', 'CAB-A02', 'CAB-A03'], totalU: 126, usedU: 108, remaining: 18 },
  { name: 'Database Cluster', racks: ['CAB-A04', 'CAB-B01'], totalU: 84, usedU: 75, remaining: 9 },
  { name: 'GPU Compute', racks: ['CAB-B02', 'CAB-B03'], totalU: 84, usedU: 63, remaining: 21 },
  { name: 'Storage', racks: ['CAB-C01', 'CAB-C02'], totalU: 84, usedU: 48, remaining: 36 },
  { name: 'Network Equipment', racks: ['CAB-D01'], totalU: 48, usedU: 12, remaining: 36 },
  { name: 'Development', racks: ['CAB-D02', 'CAB-D03'], totalU: 84, usedU: 28, remaining: 56 },
]

function utilColor(pct: number): string {
  if (pct >= 90) return 'danger'
  if (pct >= 75) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cabinet Space Remaining</h4>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Group / Application</th><th>Racks</th><th class="text-end">Total U</th><th class="text-end">Used U</th><th class="text-end">Remaining U</th><th style="width:22%">Utilization</th></tr>
          </thead>
          <tbody>
            <tr v-for="g in groups" :key="g.name">
              <td class="fw-semibold">{{ g.name }}</td>
              <td><span v-for="r in g.racks" :key="r" class="badge bg-light text-dark border me-1">{{ r }}</span></td>
              <td class="text-end">{{ g.totalU }}</td>
              <td class="text-end">{{ g.usedU }}</td>
              <td class="text-end text-success fw-semibold">{{ g.remaining }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="progress flex-fill" style="height: 8px;">
                    <div :class="`progress-bar bg-${utilColor(g.usedU / g.totalU * 100)}`" :style="{ width: (g.usedU / g.totalU * 100) + '%' }"></div>
                  </div>
                  <small>{{ (g.usedU / g.totalU * 100).toFixed(0) }}%</small>
                </div>
              </td>
            </tr>
          </tbody>
          <tfoot class="table-light">
            <tr class="fw-semibold">
              <td>Total</td><td></td>
              <td class="text-end">{{ groups.reduce((a, g) => a + g.totalU, 0) }}</td>
              <td class="text-end">{{ groups.reduce((a, g) => a + g.usedU, 0) }}</td>
              <td class="text-end text-success">{{ groups.reduce((a, g) => a + g.remaining, 0) }}</td>
              <td></td>
            </tr>
          </tfoot>
        </table>
      </div>
    </div>
  </div>
</template>
