<script setup lang="ts">
const sensors = [
  { id: 'TEMP-A01-IN', cabinet: 'CAB-A01', type: 'Inlet Temp' },
  { id: 'TEMP-B01-IN', cabinet: 'CAB-B01', type: 'Inlet Temp' },
  { id: 'TEMP-B02-IN', cabinet: 'CAB-B02', type: 'Inlet Temp' },
]

const trend: Record<string, { time: string; value: number }[]> = {
  'TEMP-A01-IN': [
    { time: '00:00', value: 21.8 }, { time: '04:00', value: 21.5 }, { time: '08:00', value: 22.1 },
    { time: '12:00', value: 22.6 }, { time: '16:00', value: 22.4 }, { time: '20:00', value: 22.0 }, { time: 'Now', value: 22.4 },
  ],
  'TEMP-B01-IN': [
    { time: '00:00', value: 23.4 }, { time: '04:00', value: 23.0 }, { time: '08:00', value: 24.0 },
    { time: '12:00', value: 24.8 }, { time: '16:00', value: 24.6 }, { time: '20:00', value: 24.2 }, { time: 'Now', value: 24.6 },
  ],
  'TEMP-B02-IN': [
    { time: '00:00', value: 24.0 }, { time: '04:00', value: 23.6 }, { time: '08:00', value: 24.5 },
    { time: '12:00', value: 25.4 }, { time: '16:00', value: 25.2 }, { time: '20:00', value: 24.8 }, { time: 'Now', value: 25.2 },
  ],
}

function tempColor(val: number): string {
  if (val >= 25) return 'bg-danger'
  if (val >= 23) return 'bg-warning'
  return 'bg-success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Sensors Trend</h4>
    <p class="text-muted">Historical temperature sensor readings (last 24h).</p>

    <div v-for="sensor in sensors" :key="sensor.id" class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent d-flex justify-content-between">
        <span class="fw-semibold">{{ sensor.id }} <span class="text-muted fw-normal">— {{ sensor.cabinet }} ({{ sensor.type }})</span></span>
      </div>
      <div class="card-body p-0">
        <table class="table table-sm table-hover mb-0">
          <thead class="table-light">
            <tr><th>Time</th><th class="text-end">Temp (°C)</th><th style="width:55%">Bar</th></tr>
          </thead>
          <tbody>
            <tr v-for="t in trend[sensor.id]" :key="t.time">
              <td>{{ t.time }}</td>
              <td class="text-end fw-semibold">{{ t.value.toFixed(1) }}</td>
              <td>
                <div class="progress" style="height: 10px;">
                  <div :class="`progress-bar ${tempColor(t.value)}`" :style="{ width: ((t.value - 18) / 12 * 100) + '%' }"></div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
