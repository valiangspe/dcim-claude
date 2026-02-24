<script setup lang="ts">
const chillers = [
  { name: 'Chiller-1', type: 'Centrifugal', capacity: 1200, load: 920, cop: 5.2, eer: 17.8, supplyW: 7.2, returnW: 12.8, flow: 280, status: 'running' },
  { name: 'Chiller-2', type: 'Screw', capacity: 800, load: 580, cop: 4.8, eer: 16.4, supplyW: 7.5, returnW: 12.5, flow: 210, status: 'running' },
]

const hourly = [
  { time: '00:00', ch1Cop: 5.4, ch2Cop: 5.0 },
  { time: '04:00', ch1Cop: 5.5, ch2Cop: 5.1 },
  { time: '08:00', ch1Cop: 5.3, ch2Cop: 4.9 },
  { time: '12:00', ch1Cop: 5.0, ch2Cop: 4.6 },
  { time: '16:00', ch1Cop: 5.1, ch2Cop: 4.7 },
  { time: '20:00', ch1Cop: 5.3, ch2Cop: 4.9 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Chiller Efficiency</h4>

    <div class="row g-3 mb-4">
      <div v-for="c in chillers" :key="c.name" class="col-lg-6">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
            <span class="fw-semibold">{{ c.name }} <span class="text-muted fw-normal">({{ c.type }})</span></span>
            <span class="badge bg-success">{{ c.status }}</span>
          </div>
          <div class="card-body">
            <div class="row text-center mb-3">
              <div class="col-4">
                <h3 class="text-primary mb-0">{{ c.cop.toFixed(1) }}</h3>
                <small class="text-muted">COP</small>
              </div>
              <div class="col-4">
                <h3 class="text-info mb-0">{{ c.eer.toFixed(1) }}</h3>
                <small class="text-muted">EER</small>
              </div>
              <div class="col-4">
                <h3 class="text-warning mb-0">{{ (c.load / c.capacity * 100).toFixed(0) }}%</h3>
                <small class="text-muted">Load</small>
              </div>
            </div>
            <div class="progress mb-3" style="height: 8px;">
              <div class="progress-bar bg-primary" :style="{ width: (c.load / c.capacity * 100) + '%' }"></div>
            </div>
            <div class="row small text-muted">
              <div class="col-6">Capacity: <strong>{{ c.capacity }} kW</strong></div>
              <div class="col-6">Load: <strong>{{ c.load }} kW</strong></div>
              <div class="col-6">Supply Water: <strong>{{ c.supplyW }}°C</strong></div>
              <div class="col-6">Return Water: <strong>{{ c.returnW }}°C</strong></div>
              <div class="col-6">Flow Rate: <strong>{{ c.flow }} L/min</strong></div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">COP Trend (24h)</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Time</th><th class="text-end">Chiller-1 COP</th><th class="text-end">Chiller-2 COP</th></tr>
          </thead>
          <tbody>
            <tr v-for="h in hourly" :key="h.time">
              <td>{{ h.time }}</td>
              <td class="text-end fw-semibold">{{ h.ch1Cop.toFixed(1) }}</td>
              <td class="text-end fw-semibold">{{ h.ch2Cop.toFixed(1) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
