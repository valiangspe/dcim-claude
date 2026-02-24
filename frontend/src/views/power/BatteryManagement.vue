<script setup lang="ts">
const banks = [
  { name: 'Bank A-1', type: 'Li-Ion', cells: 48, soc: 98, soh: 94, voltage: 48.6, temp: 24, runtime: 18, status: 'charged' },
  { name: 'Bank A-2', type: 'Li-Ion', cells: 48, soc: 97, soh: 92, voltage: 48.4, temp: 25, runtime: 17, status: 'charged' },
  { name: 'Bank B-1', type: 'VRLA', cells: 40, soc: 95, soh: 87, voltage: 48.1, temp: 26, runtime: 14, status: 'charged' },
  { name: 'Bank B-2', type: 'VRLA', cells: 40, soc: 42, soh: 85, voltage: 46.2, temp: 27, runtime: 6, status: 'charging' },
]

function socColor(soc: number): string {
  if (soc >= 80) return 'success'
  if (soc >= 50) return 'warning'
  return 'danger'
}

function sohColor(soh: number): string {
  if (soh >= 90) return 'success'
  if (soh >= 75) return 'warning'
  return 'danger'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Battery Management</h4>

    <div class="row g-3 mb-4">
      <div v-for="b in banks" :key="b.name" class="col-md-6 col-xl-3">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
            <span class="fw-semibold">{{ b.name }}</span>
            <span class="badge" :class="b.status === 'charged' ? 'bg-success' : 'bg-warning text-dark'">{{ b.status }}</span>
          </div>
          <div class="card-body">
            <div class="text-center mb-3">
              <div :class="`display-5 fw-bold text-${socColor(b.soc)}`">{{ b.soc }}%</div>
              <small class="text-muted">State of Charge</small>
            </div>
            <div class="mb-2">
              <div class="d-flex justify-content-between small mb-1">
                <span>SOH</span>
                <span :class="`text-${sohColor(b.soh)}`">{{ b.soh }}%</span>
              </div>
              <div class="progress" style="height: 6px;">
                <div :class="`progress-bar bg-${sohColor(b.soh)}`" :style="{ width: b.soh + '%' }"></div>
              </div>
            </div>
            <ul class="list-unstyled small text-muted mb-0 mt-3">
              <li>Type: <strong>{{ b.type }}</strong></li>
              <li>Cells: <strong>{{ b.cells }}</strong></li>
              <li>Voltage: <strong>{{ b.voltage.toFixed(1) }}V</strong></li>
              <li>Temp: <strong>{{ b.temp }}&deg;C</strong></li>
              <li>Runtime: <strong>{{ b.runtime }} min</strong></li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
