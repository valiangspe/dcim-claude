<script setup lang="ts">
const cabinets = [
  { id: 'CAB-A01', location: 'Hall A, Row 1', rh: 45.2, dewpoint: 10.8, alarm: false },
  { id: 'CAB-A02', location: 'Hall A, Row 1', rh: 47.1, dewpoint: 11.4, alarm: false },
  { id: 'CAB-A03', location: 'Hall A, Row 2', rh: 43.8, dewpoint: 10.2, alarm: false },
  { id: 'CAB-B01', location: 'Hall B, Row 1', rh: 52.6, dewpoint: 13.1, alarm: false },
  { id: 'CAB-B02', location: 'Hall B, Row 1', rh: 58.3, dewpoint: 14.8, alarm: true },
  { id: 'CAB-B03', location: 'Hall B, Row 2', rh: 44.5, dewpoint: 10.5, alarm: false },
  { id: 'CAB-C01', location: 'Hall C, Row 1', rh: 42.1, dewpoint: 9.6, alarm: false },
  { id: 'CAB-C02', location: 'Hall C, Row 1', rh: 38.4, dewpoint: 8.2, alarm: true },
]

function rhColor(rh: number): string {
  if (rh > 55 || rh < 40) return 'text-danger fw-semibold'
  if (rh > 50 || rh < 42) return 'text-warning'
  return ''
}
</script>

<template>
  <div>
    <h4 class="mb-4">Humidity Monitoring</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
          <div class="card-body"><h3 class="text-primary mb-0">46.5% RH</h3><small class="text-muted">Avg Relative Humidity</small></div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
          <div class="card-body"><h3 class="text-info mb-0">11.1°C</h3><small class="text-muted">Avg Dew Point</small></div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center bg-danger bg-opacity-10">
          <div class="card-body"><h3 class="text-danger mb-0">{{ cabinets.filter(c => c.alarm).length }}</h3><small class="text-muted">Humidity Alarms</small></div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Per-Cabinet Humidity</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Cabinet</th><th>Location</th><th class="text-end">RH (%)</th><th style="width:22%">Bar</th><th class="text-end">Dew Point (°C)</th><th>Status</th></tr>
          </thead>
          <tbody>
            <tr v-for="c in cabinets" :key="c.id" :class="c.alarm ? 'table-warning' : ''">
              <td class="fw-semibold">{{ c.id }}</td>
              <td class="text-muted">{{ c.location }}</td>
              <td class="text-end" :class="rhColor(c.rh)">{{ c.rh.toFixed(1) }}%</td>
              <td>
                <div class="progress" style="height: 8px;">
                  <div class="progress-bar" :class="c.rh > 55 || c.rh < 40 ? 'bg-danger' : 'bg-primary'" :style="{ width: c.rh + '%' }"></div>
                </div>
              </td>
              <td class="text-end">{{ c.dewpoint.toFixed(1) }}</td>
              <td>
                <span v-if="c.alarm" class="badge bg-warning text-dark">ALARM</span>
                <span v-else class="badge bg-success">OK</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="small text-muted mt-3">ASHRAE recommended RH range: 40% – 55%. Dew point max: 15°C.</div>
  </div>
</template>
