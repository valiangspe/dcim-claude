<script setup lang="ts">
const kpis = [
  { label: 'Total Capacity', value: '3,200', unit: 'kW', color: 'primary' },
  { label: 'Current Load', value: '2,432', unit: 'kW', color: 'info' },
  { label: 'Utilization', value: '76', unit: '%', color: 'warning' },
  { label: 'Avg Supply Temp', value: '14.2', unit: '°C', color: 'success' },
  { label: 'Avg Return Temp', value: '28.6', unit: '°C', color: 'danger' },
  { label: 'Delta-T', value: '14.4', unit: '°C', color: 'dark' },
]

const units = [
  { name: 'CRAH-A1', type: 'CRAH', capacity: 600, load: 482, supply: 13.8, ret: 28.2, fan: 78, status: 'running' },
  { name: 'CRAH-A2', type: 'CRAH', capacity: 600, load: 510, supply: 14.1, ret: 29.0, fan: 82, status: 'running' },
  { name: 'CRAH-B1', type: 'CRAH', capacity: 600, load: 445, supply: 14.5, ret: 28.4, fan: 72, status: 'running' },
  { name: 'CRAH-B2', type: 'CRAH', capacity: 600, load: 498, supply: 14.0, ret: 28.8, fan: 80, status: 'running' },
  { name: 'CRAC-C1', type: 'CRAC', capacity: 400, load: 312, supply: 14.6, ret: 27.9, fan: 68, status: 'running' },
  { name: 'CRAC-C2', type: 'CRAC', capacity: 400, load: 185, supply: 14.3, ret: 27.5, fan: 55, status: 'running' },
  { name: 'CRAH-D1', type: 'CRAH', capacity: 600, load: 0, supply: 0, ret: 0, fan: 0, status: 'standby' },
]

function loadColor(pct: number): string {
  if (pct >= 85) return 'danger'
  if (pct >= 70) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cooling Overview</h4>

    <div class="row g-3 mb-4">
      <div v-for="k in kpis" :key="k.label" class="col-sm-6 col-lg-4 col-xl-2">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body text-center">
            <h3 :class="`text-${k.color} mb-0`">{{ k.value }}<small class="text-muted fs-6">{{ k.unit }}</small></h3>
            <small class="text-muted">{{ k.label }}</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Cooling Units</div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Unit</th><th>Type</th><th class="text-end">Capacity (kW)</th><th class="text-end">Load (kW)</th>
                <th style="width:15%">Utilization</th><th class="text-end">Supply (°C)</th><th class="text-end">Return (°C)</th>
                <th class="text-end">Fan %</th><th>Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="u in units" :key="u.name">
                <td class="fw-semibold">{{ u.name }}</td>
                <td><span class="badge bg-secondary">{{ u.type }}</span></td>
                <td class="text-end">{{ u.capacity }}</td>
                <td class="text-end">{{ u.load > 0 ? u.load : '---' }}</td>
                <td>
                  <div v-if="u.load > 0" class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 8px;">
                      <div :class="`progress-bar bg-${loadColor(u.load / u.capacity * 100)}`" :style="{ width: (u.load / u.capacity * 100) + '%' }"></div>
                    </div>
                    <small>{{ (u.load / u.capacity * 100).toFixed(0) }}%</small>
                  </div>
                  <span v-else class="text-muted">---</span>
                </td>
                <td class="text-end">{{ u.supply > 0 ? u.supply.toFixed(1) : '---' }}</td>
                <td class="text-end">{{ u.ret > 0 ? u.ret.toFixed(1) : '---' }}</td>
                <td class="text-end">{{ u.fan > 0 ? u.fan + '%' : '---' }}</td>
                <td><span class="badge" :class="u.status === 'running' ? 'bg-success' : 'bg-secondary'">{{ u.status }}</span></td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
