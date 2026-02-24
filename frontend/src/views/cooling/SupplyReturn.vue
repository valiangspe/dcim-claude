<script setup lang="ts">
const units = [
  { name: 'CRAH-A1', supplyTemp: 13.8, returnTemp: 28.2, supplyFlow: 4200, returnFlow: 4180, deltaT: 14.4, status: 'normal' },
  { name: 'CRAH-A2', supplyTemp: 14.1, returnTemp: 29.0, supplyFlow: 4400, returnFlow: 4380, deltaT: 14.9, status: 'normal' },
  { name: 'CRAH-B1', supplyTemp: 14.5, returnTemp: 28.4, supplyFlow: 3800, returnFlow: 3770, deltaT: 13.9, status: 'normal' },
  { name: 'CRAH-B2', supplyTemp: 14.0, returnTemp: 28.8, supplyFlow: 4100, returnFlow: 4060, deltaT: 14.8, status: 'warning' },
  { name: 'CRAC-C1', supplyTemp: 14.6, returnTemp: 27.9, supplyFlow: 2800, returnFlow: 2780, deltaT: 13.3, status: 'normal' },
  { name: 'CRAC-C2', supplyTemp: 14.3, returnTemp: 27.5, supplyFlow: 2200, returnFlow: 2190, deltaT: 13.2, status: 'normal' },
]

function deltaTColor(dt: number): string {
  if (dt > 15) return 'text-danger fw-semibold'
  if (dt > 14) return 'text-warning'
  return ''
}
</script>

<template>
  <div>
    <h4 class="mb-4">Supply & Return Air</h4>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Unit</th>
                <th class="text-end">Supply (°C)</th><th class="text-end">Return (°C)</th>
                <th class="text-end">Delta-T (°C)</th>
                <th class="text-end">Supply CFM</th><th class="text-end">Return CFM</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="u in units" :key="u.name">
                <td class="fw-semibold">{{ u.name }}</td>
                <td class="text-end text-info">{{ u.supplyTemp.toFixed(1) }}</td>
                <td class="text-end text-danger">{{ u.returnTemp.toFixed(1) }}</td>
                <td class="text-end" :class="deltaTColor(u.deltaT)">{{ u.deltaT.toFixed(1) }}</td>
                <td class="text-end">{{ u.supplyFlow.toLocaleString() }}</td>
                <td class="text-end">{{ u.returnFlow.toLocaleString() }}</td>
                <td>
                  <span class="badge" :class="u.status === 'normal' ? 'bg-success' : 'bg-warning text-dark'">{{ u.status }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
