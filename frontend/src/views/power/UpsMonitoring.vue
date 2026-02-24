<script setup lang="ts">
const units = [
  { name: 'UPS-A1', capacity: 500, load: 380, loadPct: 76, input: 230.2, output: 230.0, battery: 98, runtime: 18, mode: 'online', status: 'normal' },
  { name: 'UPS-A2', capacity: 500, load: 360, loadPct: 72, input: 229.8, output: 230.0, battery: 97, runtime: 19, mode: 'online', status: 'normal' },
  { name: 'UPS-B1', capacity: 500, load: 410, loadPct: 82, input: 230.1, output: 230.0, battery: 95, runtime: 14, mode: 'online', status: 'warning' },
  { name: 'UPS-B2', capacity: 500, load: 320, loadPct: 64, input: 230.0, output: 230.0, battery: 96, runtime: 22, mode: 'online', status: 'normal' },
  { name: 'UPS-C1', capacity: 500, load: 0, loadPct: 0, input: 230.3, output: 0, battery: 100, runtime: 0, mode: 'standby', status: 'normal' },
]

function loadColor(pct: number): string {
  if (pct >= 80) return 'danger'
  if (pct >= 60) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">UPS Monitoring</h4>

    <div class="row g-3 mb-4">
      <div v-for="u in units" :key="u.name" class="col-md-6 col-xl-4">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
            <span class="fw-semibold">{{ u.name }}</span>
            <div class="d-flex gap-1">
              <span class="badge" :class="u.mode === 'online' ? 'bg-primary' : 'bg-secondary'">{{ u.mode }}</span>
              <span class="badge" :class="u.status === 'normal' ? 'bg-success' : 'bg-warning text-dark'">{{ u.status }}</span>
            </div>
          </div>
          <div class="card-body">
            <div class="text-center mb-3">
              <div :class="`display-6 fw-bold text-${loadColor(u.loadPct)}`">{{ u.loadPct }}%</div>
              <small class="text-muted">{{ u.load }} / {{ u.capacity }} kW</small>
            </div>
            <div class="progress mb-3" style="height: 8px;">
              <div :class="`progress-bar bg-${loadColor(u.loadPct)}`" :style="{ width: u.loadPct + '%' }"></div>
            </div>
            <div class="row small text-muted">
              <div class="col-6">Input: <strong>{{ u.input }}V</strong></div>
              <div class="col-6">Output: <strong>{{ u.output > 0 ? u.output + 'V' : '---' }}</strong></div>
              <div class="col-6">Battery: <strong>{{ u.battery }}%</strong></div>
              <div class="col-6">Runtime: <strong>{{ u.runtime > 0 ? u.runtime + ' min' : '---' }}</strong></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
