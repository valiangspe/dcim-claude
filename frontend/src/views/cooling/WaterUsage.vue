<script setup lang="ts">
const currentWue = 1.42
const targetWue = 1.30

const monthly = [
  { month: 'Sep 2025', waterL: 182000, itKwh: 1480000, wue: 1.48 },
  { month: 'Oct 2025', waterL: 178000, itKwh: 1490000, wue: 1.46 },
  { month: 'Nov 2025', waterL: 170000, itKwh: 1485000, wue: 1.44 },
  { month: 'Dec 2025', waterL: 165000, itKwh: 1500000, wue: 1.43 },
  { month: 'Jan 2026', waterL: 162000, itKwh: 1495000, wue: 1.42 },
  { month: 'Feb 2026', waterL: 160000, itKwh: 1500000, wue: 1.42 },
]

function wueColor(wue: number): string {
  if (wue <= 1.30) return 'success'
  if (wue <= 1.50) return 'warning'
  return 'danger'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Water Usage Effectiveness (WUE)</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-4">
        <div :class="`card border-0 shadow-sm text-center bg-${wueColor(currentWue)} bg-opacity-10`">
          <div class="card-body py-4">
            <div :class="`display-3 fw-bold text-${wueColor(currentWue)}`">{{ currentWue.toFixed(2) }}</div>
            <div class="text-muted">Current WUE (L/kWh)</div>
            <small class="text-muted">Target: {{ targetWue.toFixed(2) }}</small>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
          <div class="card-body py-4">
            <h2 class="text-info mb-0">160,000 L</h2>
            <small class="text-muted">Monthly Water Consumption</small>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
          <div class="card-body py-4">
            <h2 class="text-primary mb-0">1.92M L</h2>
            <small class="text-muted">Annual Projection</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Monthly WUE Trend</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Month</th><th class="text-end">Water (L)</th><th class="text-end">IT Energy (kWh)</th><th class="text-end">WUE</th><th class="text-end">vs Target</th></tr>
          </thead>
          <tbody>
            <tr v-for="m in monthly" :key="m.month">
              <td class="fw-semibold">{{ m.month }}</td>
              <td class="text-end">{{ m.waterL.toLocaleString() }}</td>
              <td class="text-end">{{ m.itKwh.toLocaleString() }}</td>
              <td class="text-end" :class="`text-${wueColor(m.wue)} fw-semibold`">{{ m.wue.toFixed(2) }}</td>
              <td class="text-end">
                <span :class="m.wue <= targetWue ? 'text-success' : 'text-danger'">
                  {{ m.wue <= targetWue ? 'On target' : '+' + (m.wue - targetWue).toFixed(2) }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
