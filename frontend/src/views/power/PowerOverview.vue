<script setup lang="ts">
const summary = [
  { label: 'Total Capacity', value: '4,200', unit: 'kW', color: 'primary' },
  { label: 'Current Load', value: '2,847', unit: 'kW', color: 'info' },
  { label: 'Load %', value: '67.8', unit: '%', color: 'warning' },
  { label: 'PUE', value: '1.38', unit: '', color: 'success' },
  { label: 'Redundancy', value: 'N+1', unit: '', color: 'secondary' },
  { label: 'Cost / kWh', value: '$0.087', unit: '', color: 'dark' },
]

const feeds = [
  { name: 'Utility Feed A', capacity: 2100, load: 1480, status: 'active' },
  { name: 'Utility Feed B', capacity: 2100, load: 1367, status: 'active' },
  { name: 'Generator G1', capacity: 2000, load: 0, status: 'standby' },
  { name: 'Generator G2', capacity: 2000, load: 0, status: 'standby' },
]

const hourly = [
  { time: '00:00', load: 2650 }, { time: '02:00', load: 2610 },
  { time: '04:00', load: 2580 }, { time: '06:00', load: 2640 },
  { time: '08:00', load: 2720 }, { time: '10:00', load: 2790 },
  { time: '12:00', load: 2860 }, { time: '14:00', load: 2880 },
  { time: '16:00', load: 2850 }, { time: '18:00', load: 2810 },
  { time: '20:00', load: 2760 }, { time: '22:00', load: 2700 },
]

const maxLoad = 3000
</script>

<template>
  <div>
    <h4 class="mb-4">Power Overview</h4>

    <div class="row g-3 mb-4">
      <div v-for="s in summary" :key="s.label" class="col-sm-6 col-lg-4 col-xl-2">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body text-center">
            <h3 :class="`text-${s.color} mb-0`">{{ s.value }}<small class="text-muted fs-6">{{ s.unit }}</small></h3>
            <small class="text-muted">{{ s.label }}</small>
          </div>
        </div>
      </div>
    </div>

    <div class="row g-3 mb-4">
      <div class="col-lg-6">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">Power Feeds</div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Feed</th><th>Capacity</th><th>Load</th><th>Util</th><th>Status</th></tr>
              </thead>
              <tbody>
                <tr v-for="f in feeds" :key="f.name">
                  <td class="fw-semibold">{{ f.name }}</td>
                  <td>{{ f.capacity.toLocaleString() }} kW</td>
                  <td>{{ f.load > 0 ? f.load.toLocaleString() + ' kW' : '---' }}</td>
                  <td>
                    <div v-if="f.load > 0" class="progress" style="height: 8px; width: 80px;">
                      <div class="progress-bar" :class="f.load / f.capacity > 0.8 ? 'bg-danger' : 'bg-success'" :style="{ width: (f.load / f.capacity * 100) + '%' }"></div>
                    </div>
                    <span v-else class="text-muted">---</span>
                  </td>
                  <td><span class="badge" :class="f.status === 'active' ? 'bg-success' : 'bg-secondary'">{{ f.status }}</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">24h Load Profile</div>
          <div class="card-body p-0">
            <table class="table table-sm table-hover mb-0">
              <thead class="table-light">
                <tr><th>Time</th><th>Load (kW)</th><th style="width:50%">Bar</th></tr>
              </thead>
              <tbody>
                <tr v-for="h in hourly" :key="h.time">
                  <td>{{ h.time }}</td>
                  <td>{{ h.load.toLocaleString() }}</td>
                  <td>
                    <div class="progress" style="height: 10px;">
                      <div class="progress-bar bg-primary" :style="{ width: (h.load / maxLoad * 100) + '%' }"></div>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
