<script setup lang="ts">
const cabinets = [
  { id: 'CAB-A01', cfm: 820, kw: 8.4, cfmPerKw: 97.6, pressure: 0.05, leakage: 'low' },
  { id: 'CAB-A02', cfm: 1050, kw: 11.2, cfmPerKw: 93.8, pressure: 0.06, leakage: 'low' },
  { id: 'CAB-A03', cfm: 620, kw: 6.1, cfmPerKw: 101.6, pressure: 0.04, leakage: 'none' },
  { id: 'CAB-B01', cfm: 980, kw: 9.8, cfmPerKw: 100.0, pressure: 0.07, leakage: 'medium' },
  { id: 'CAB-B02', cfm: 1380, kw: 14.3, cfmPerKw: 96.5, pressure: 0.08, leakage: 'medium' },
  { id: 'CAB-B03', cfm: 760, kw: 7.5, cfmPerKw: 101.3, pressure: 0.05, leakage: 'low' },
  { id: 'CAB-C01', cfm: 510, kw: 5.2, cfmPerKw: 98.1, pressure: 0.03, leakage: 'none' },
  { id: 'CAB-C02', cfm: 390, kw: 3.8, cfmPerKw: 102.6, pressure: 0.03, leakage: 'none' },
]

function leakageBadge(l: string): string {
  if (l === 'none') return 'bg-success'
  if (l === 'low') return 'bg-info text-dark'
  return 'bg-warning text-dark'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Airflow Monitoring</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
          <div class="card-body"><h3 class="text-primary mb-0">6,510</h3><small class="text-muted">Total CFM</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
          <div class="card-body"><h3 class="text-info mb-0">98.4</h3><small class="text-muted">Avg CFM/kW</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-success bg-opacity-10">
          <div class="card-body"><h3 class="text-success mb-0">0.05</h3><small class="text-muted">Avg Pressure (inH2O)</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
          <div class="card-body"><h3 class="text-warning mb-0">2</h3><small class="text-muted">Medium Leakage</small></div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Per-Cabinet Airflow</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Cabinet</th><th class="text-end">CFM</th><th class="text-end">Power (kW)</th><th class="text-end">CFM/kW</th><th class="text-end">Pressure (inH2O)</th><th>Leakage</th></tr>
          </thead>
          <tbody>
            <tr v-for="c in cabinets" :key="c.id">
              <td class="fw-semibold">{{ c.id }}</td>
              <td class="text-end">{{ c.cfm.toLocaleString() }}</td>
              <td class="text-end">{{ c.kw.toFixed(1) }}</td>
              <td class="text-end">{{ c.cfmPerKw.toFixed(1) }}</td>
              <td class="text-end">{{ c.pressure.toFixed(2) }}</td>
              <td><span class="badge" :class="leakageBadge(c.leakage)">{{ c.leakage }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
