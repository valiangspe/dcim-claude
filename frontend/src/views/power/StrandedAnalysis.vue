<script setup lang="ts">
const stranded = [
  { location: 'Hall A, Row 1', allocated: 120, actual: 84, stranded: 36, pct: 30.0 },
  { location: 'Hall A, Row 2', allocated: 96, actual: 61, stranded: 35, pct: 36.5 },
  { location: 'Hall B, Row 1', allocated: 160, actual: 122, stranded: 38, pct: 23.8 },
  { location: 'Hall B, Row 2', allocated: 128, actual: 75, stranded: 53, pct: 41.4 },
  { location: 'Hall C, Row 1', allocated: 64, actual: 42, stranded: 22, pct: 34.4 },
]

const totalStranded = stranded.reduce((a, s) => a + s.stranded, 0)
const totalAllocated = stranded.reduce((a, s) => a + s.allocated, 0)
</script>

<template>
  <div>
    <h4 class="mb-4">Stranded Power Analysis</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-danger bg-opacity-10">
          <div class="card-body py-4">
            <h2 class="text-danger mb-0">{{ totalStranded }} kW</h2>
            <small class="text-muted">Total Stranded Power</small>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
          <div class="card-body py-4">
            <h2 class="text-warning mb-0">{{ (totalStranded / totalAllocated * 100).toFixed(1) }}%</h2>
            <small class="text-muted">Stranded Ratio</small>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
          <div class="card-body py-4">
            <h2 class="text-info mb-0">${{ (totalStranded * 0.087 * 8760 / 1000).toFixed(0) }}k</h2>
            <small class="text-muted">Est. Annual Waste</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Location</th><th class="text-end">Allocated (kW)</th><th class="text-end">Actual (kW)</th><th class="text-end">Stranded (kW)</th><th style="width:25%">Stranded %</th></tr>
          </thead>
          <tbody>
            <tr v-for="s in stranded" :key="s.location">
              <td class="fw-semibold">{{ s.location }}</td>
              <td class="text-end">{{ s.allocated }}</td>
              <td class="text-end">{{ s.actual }}</td>
              <td class="text-end text-danger fw-semibold">{{ s.stranded }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="progress flex-fill" style="height: 8px;">
                    <div class="progress-bar bg-danger" :style="{ width: s.pct + '%' }"></div>
                  </div>
                  <small>{{ s.pct.toFixed(1) }}%</small>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
