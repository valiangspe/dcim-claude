<script setup lang="ts">
const halls = [
  { name: 'Hall A', current: 1200, max: 1800, planned: 200, available: 400, growth: 8.2 },
  { name: 'Hall B', current: 800, max: 1200, planned: 150, available: 250, growth: 12.5 },
  { name: 'Hall C', current: 432, max: 800, planned: 80, available: 288, growth: 6.1 },
]

const forecast = [
  { quarter: 'Q2 2026', projectedLoad: 2580, capacity: 3200, headroom: 620 },
  { quarter: 'Q3 2026', projectedLoad: 2720, capacity: 3200, headroom: 480 },
  { quarter: 'Q4 2026', projectedLoad: 2850, capacity: 3200, headroom: 350 },
  { quarter: 'Q1 2027', projectedLoad: 2990, capacity: 3600, headroom: 610 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Cooling Capacity Planning</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
          <div class="card-body"><h3 class="text-primary mb-0">3,200 kW</h3><small class="text-muted">Total Capacity</small></div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
          <div class="card-body"><h3 class="text-warning mb-0">2,432 kW</h3><small class="text-muted">Current Load</small></div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-success bg-opacity-10">
          <div class="card-body"><h3 class="text-success mb-0">768 kW</h3><small class="text-muted">Available Headroom</small></div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">Per-Hall Capacity</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Hall</th><th class="text-end">Current (kW)</th><th class="text-end">Max (kW)</th><th class="text-end">Planned (kW)</th><th class="text-end">Available (kW)</th><th class="text-end">Growth %/yr</th><th style="width:20%">Utilization</th></tr>
          </thead>
          <tbody>
            <tr v-for="h in halls" :key="h.name">
              <td class="fw-semibold">{{ h.name }}</td>
              <td class="text-end">{{ h.current.toLocaleString() }}</td>
              <td class="text-end">{{ h.max.toLocaleString() }}</td>
              <td class="text-end">{{ h.planned }}</td>
              <td class="text-end text-success fw-semibold">{{ h.available }}</td>
              <td class="text-end">{{ h.growth }}%</td>
              <td>
                <div class="progress" style="height: 8px;">
                  <div class="progress-bar bg-primary" :style="{ width: (h.current / h.max * 100) + '%' }"></div>
                  <div class="progress-bar bg-warning" :style="{ width: (h.planned / h.max * 100) + '%' }"></div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Quarterly Forecast</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Quarter</th><th class="text-end">Projected Load (kW)</th><th class="text-end">Capacity (kW)</th><th class="text-end">Headroom (kW)</th></tr>
          </thead>
          <tbody>
            <tr v-for="f in forecast" :key="f.quarter">
              <td class="fw-semibold">{{ f.quarter }}</td>
              <td class="text-end">{{ f.projectedLoad.toLocaleString() }}</td>
              <td class="text-end">{{ f.capacity.toLocaleString() }}</td>
              <td class="text-end" :class="f.headroom < 400 ? 'text-danger fw-semibold' : 'text-success'">{{ f.headroom }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
