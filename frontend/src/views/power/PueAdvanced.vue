<script setup lang="ts">
const metrics = [
  { name: 'PUE1 (Total)', value: 1.38, target: 1.40, stdDev: 0.03 },
  { name: 'PUE2 (IT + Cooling)', value: 1.22, target: 1.25, stdDev: 0.02 },
  { name: 'PUE3 (IT Only)', value: 1.08, target: 1.10, stdDev: 0.01 },
]

const monthly = [
  { month: 'Sep 2025', pue1: 1.42, pue2: 1.26, pue3: 1.10 },
  { month: 'Oct 2025', pue1: 1.41, pue2: 1.25, pue3: 1.09 },
  { month: 'Nov 2025', pue1: 1.40, pue2: 1.24, pue3: 1.09 },
  { month: 'Dec 2025', pue1: 1.39, pue2: 1.23, pue3: 1.08 },
  { month: 'Jan 2026', pue1: 1.39, pue2: 1.23, pue3: 1.08 },
  { month: 'Feb 2026', pue1: 1.38, pue2: 1.22, pue3: 1.08 },
]

function pueColor(pue: number, target: number): string {
  return pue <= target ? 'success' : pue <= target + 0.05 ? 'warning' : 'danger'
}
</script>

<template>
  <div>
    <h4 class="mb-4">PUE Advanced</h4>

    <div class="row g-3 mb-4">
      <div v-for="m in metrics" :key="m.name" class="col-md-4">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body text-center">
            <h2 :class="`text-${pueColor(m.value, m.target)} mb-1`">{{ m.value.toFixed(2) }}</h2>
            <div class="fw-semibold">{{ m.name }}</div>
            <small class="text-muted">Target: {{ m.target.toFixed(2) }} &middot; &sigma; {{ m.stdDev.toFixed(2) }}</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">6-Month Trend</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Month</th><th class="text-end">PUE1</th><th class="text-end">PUE2</th><th class="text-end">PUE3</th></tr>
          </thead>
          <tbody>
            <tr v-for="r in monthly" :key="r.month">
              <td class="fw-semibold">{{ r.month }}</td>
              <td class="text-end">{{ r.pue1.toFixed(2) }}</td>
              <td class="text-end">{{ r.pue2.toFixed(2) }}</td>
              <td class="text-end">{{ r.pue3.toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
