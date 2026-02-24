<script setup lang="ts">
const sites = [
  { name: 'DC-East-01', budget: 2100, actual: 1480, reserved: 200 },
  { name: 'DC-West-01', budget: 1500, actual: 980, reserved: 150 },
  { name: 'DC-EU-01', budget: 900, actual: 627, reserved: 100 },
  { name: 'DC-EU-02', budget: 600, actual: 412, reserved: 50 },
  { name: 'DC-APAC-01', budget: 500, actual: 0, reserved: 0 },
  { name: 'DC-South-01', budget: 400, actual: 215, reserved: 30 },
]

function utilColor(pct: number): string {
  if (pct >= 85) return 'danger'
  if (pct >= 70) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Budget Capacity</h4>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Site</th>
              <th class="text-end">Budget (kW)</th>
              <th class="text-end">Actual (kW)</th>
              <th class="text-end">Reserved (kW)</th>
              <th class="text-end">Available (kW)</th>
              <th style="width:25%">Utilization</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="s in sites" :key="s.name">
              <td class="fw-semibold">{{ s.name }}</td>
              <td class="text-end">{{ s.budget.toLocaleString() }}</td>
              <td class="text-end">{{ s.actual > 0 ? s.actual.toLocaleString() : '---' }}</td>
              <td class="text-end">{{ s.reserved }}</td>
              <td class="text-end">{{ (s.budget - s.actual - s.reserved).toLocaleString() }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="progress flex-fill" style="height: 10px;">
                    <div :class="`progress-bar bg-${utilColor(s.actual / s.budget * 100)}`" :style="{ width: (s.actual / s.budget * 100) + '%' }"></div>
                  </div>
                  <small class="text-nowrap">{{ (s.actual / s.budget * 100).toFixed(1) }}%</small>
                </div>
              </td>
            </tr>
          </tbody>
          <tfoot class="table-light">
            <tr class="fw-semibold">
              <td>Total</td>
              <td class="text-end">{{ sites.reduce((a, s) => a + s.budget, 0).toLocaleString() }}</td>
              <td class="text-end">{{ sites.reduce((a, s) => a + s.actual, 0).toLocaleString() }}</td>
              <td class="text-end">{{ sites.reduce((a, s) => a + s.reserved, 0) }}</td>
              <td class="text-end">{{ sites.reduce((a, s) => a + s.budget - s.actual - s.reserved, 0).toLocaleString() }}</td>
              <td></td>
            </tr>
          </tfoot>
        </table>
      </div>
    </div>
  </div>
</template>
