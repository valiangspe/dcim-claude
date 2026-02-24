<script setup lang="ts">
const cabinets = [
  { id: 'CAB-A01', location: 'Hall A, Row 1', inlet: 22.4, outlet: 34.8, delta: 12.4, alarm: false },
  { id: 'CAB-A02', location: 'Hall A, Row 1', inlet: 23.1, outlet: 36.2, delta: 13.1, alarm: false },
  { id: 'CAB-A03', location: 'Hall A, Row 2', inlet: 21.8, outlet: 32.5, delta: 10.7, alarm: false },
  { id: 'CAB-B01', location: 'Hall B, Row 1', inlet: 24.6, outlet: 38.1, delta: 13.5, alarm: true },
  { id: 'CAB-B02', location: 'Hall B, Row 1', inlet: 25.2, outlet: 39.4, delta: 14.2, alarm: true },
  { id: 'CAB-B03', location: 'Hall B, Row 2', inlet: 22.0, outlet: 33.1, delta: 11.1, alarm: false },
  { id: 'CAB-C01', location: 'Hall C, Row 1', inlet: 21.5, outlet: 30.2, delta: 8.7, alarm: false },
  { id: 'CAB-C02', location: 'Hall C, Row 1', inlet: 21.2, outlet: 29.8, delta: 8.6, alarm: false },
]

function tempColor(temp: number): string {
  if (temp >= 25) return 'text-danger fw-semibold'
  if (temp >= 23) return 'text-warning'
  return ''
}

function outletColor(temp: number): string {
  if (temp >= 38) return 'text-danger fw-semibold'
  if (temp >= 35) return 'text-warning'
  return ''
}
</script>

<template>
  <div>
    <h4 class="mb-4">Temperature Monitoring</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
          <div class="card-body"><h3 class="text-info mb-0">22.7°C</h3><small class="text-muted">Avg Inlet</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-danger bg-opacity-10">
          <div class="card-body"><h3 class="text-danger mb-0">34.3°C</h3><small class="text-muted">Avg Outlet</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
          <div class="card-body"><h3 class="text-warning mb-0">11.5°C</h3><small class="text-muted">Avg Delta-T</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center" :class="cabinets.filter(c => c.alarm).length > 0 ? 'bg-danger bg-opacity-10' : 'bg-success bg-opacity-10'">
          <div class="card-body">
            <h3 :class="cabinets.filter(c => c.alarm).length > 0 ? 'text-danger' : 'text-success'" class="mb-0">{{ cabinets.filter(c => c.alarm).length }}</h3>
            <small class="text-muted">Temp Alarms</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Per-Cabinet Temperature</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Cabinet</th><th>Location</th><th class="text-end">Inlet (°C)</th><th class="text-end">Outlet (°C)</th><th class="text-end">Delta-T (°C)</th><th>Status</th></tr>
          </thead>
          <tbody>
            <tr v-for="c in cabinets" :key="c.id" :class="c.alarm ? 'table-danger' : ''">
              <td class="fw-semibold">{{ c.id }}</td>
              <td class="text-muted">{{ c.location }}</td>
              <td class="text-end" :class="tempColor(c.inlet)">{{ c.inlet.toFixed(1) }}</td>
              <td class="text-end" :class="outletColor(c.outlet)">{{ c.outlet.toFixed(1) }}</td>
              <td class="text-end">{{ c.delta.toFixed(1) }}</td>
              <td>
                <span v-if="c.alarm" class="badge bg-danger">ALARM</span>
                <span v-else class="badge bg-success">OK</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="small text-muted mt-3">ASHRAE recommended inlet range: 18°C – 27°C. Alarms triggered above 24.5°C inlet.</div>
  </div>
</template>
