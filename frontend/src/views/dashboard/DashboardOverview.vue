<script setup lang="ts">
import { computed } from 'vue'
import { useTier } from '../../composables/useTier'

const { hasAccess, currentTier } = useTier()
const isPlusOrAbove = computed(() => hasAccess('plus'))

const refreshInterval = computed(() => {
  if (currentTier.value === 'platinum') return '10 sec'
  if (currentTier.value === 'plus') return '1 min'
  return '5 min'
})

const kpis = [
  { label: 'Total Power', value: '2,847', unit: 'kW', color: 'primary', icon: 'bi-lightning-charge-fill' },
  { label: 'PUE', value: '1.38', unit: '', color: 'success', icon: 'bi-speedometer2' },
  { label: 'Cooling Capacity', value: '76', unit: '%', color: 'info', icon: 'bi-thermometer-half' },
  { label: 'Rack Utilization', value: '68', unit: '%', color: 'warning', icon: 'bi-hdd-rack' },
  { label: 'Active Alarms', value: '12', unit: '', color: 'danger', icon: 'bi-exclamation-triangle-fill' },
  { label: 'Network Uptime', value: '99.97', unit: '%', color: 'success', icon: 'bi-wifi' },
]

const alerts = [
  { severity: 'critical', count: 3, last: '2 min ago' },
  { severity: 'warning', count: 9, last: '14 min ago' },
  { severity: 'info', count: 24, last: '1 min ago' },
]

const quickStats = [
  { label: 'Total Servers', value: '1,247' },
  { label: 'Total Racks', value: '186' },
  { label: 'Occupied U Slots', value: '5,892' },
  { label: 'Free U Slots', value: '2,388' },
]

const sparklineData = [
  { time: '00:00', power: 2710, pue: 1.41, cooling: 72 },
  { time: '04:00', power: 2680, pue: 1.39, cooling: 70 },
  { time: '08:00', power: 2790, pue: 1.40, cooling: 74 },
  { time: '12:00', power: 2860, pue: 1.38, cooling: 77 },
  { time: '16:00', power: 2850, pue: 1.37, cooling: 76 },
  { time: '20:00', power: 2820, pue: 1.38, cooling: 75 },
  { time: 'Now', power: 2847, pue: 1.38, cooling: 76 },
]
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Dashboard Overview</h4>
      <span class="badge bg-dark">
        Auto-refresh: {{ refreshInterval }}
      </span>
    </div>

    <!-- KPI Cards -->
    <div class="row g-3 mb-4">
      <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg-4 col-xl-2">
        <div class="card h-100 border-0 shadow-sm">
          <div class="card-body text-center">
            <div :class="`text-${kpi.color} mb-2`">
              <i :class="kpi.icon" style="font-size: 1.5rem;"></i>
            </div>
            <h3 class="mb-0">{{ kpi.value }}<small class="text-muted fs-6">{{ kpi.unit }}</small></h3>
            <small class="text-muted">{{ kpi.label }}</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Alerts Summary -->
    <div class="row g-3 mb-4">
      <div class="col-lg-5">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">Alerts Summary</div>
          <div class="card-body p-0">
            <table class="table table-hover mb-0">
              <thead class="table-light">
                <tr>
                  <th>Severity</th>
                  <th class="text-end">Count</th>
                  <th class="text-end">Last</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="a in alerts" :key="a.severity">
                  <td>
                    <span
                      class="badge"
                      :class="{
                        'bg-danger': a.severity === 'critical',
                        'bg-warning text-dark': a.severity === 'warning',
                        'bg-info text-dark': a.severity === 'info',
                      }"
                    >{{ a.severity }}</span>
                  </td>
                  <td class="text-end fw-semibold">{{ a.count }}</td>
                  <td class="text-end text-muted">{{ a.last }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Quick Stats -->
      <div class="col-lg-7">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">Quick Stats</div>
          <div class="card-body">
            <div class="row g-3">
              <div v-for="stat in quickStats" :key="stat.label" class="col-6">
                <div class="border rounded p-3 text-center">
                  <h4 class="mb-0">{{ stat.value }}</h4>
                  <small class="text-muted">{{ stat.label }}</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Plus+ Sparkline Area -->
    <div v-if="isPlusOrAbove" class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Trend (Last 24h)</div>
      <div class="card-body p-0">
        <table class="table table-sm table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Time</th>
              <th>Power (kW)</th>
              <th style="width: 30%;">Power Bar</th>
              <th>PUE</th>
              <th>Cooling %</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in sparklineData" :key="row.time">
              <td class="fw-semibold">{{ row.time }}</td>
              <td>{{ row.power.toLocaleString() }}</td>
              <td>
                <div class="progress" style="height: 12px;">
                  <div
                    class="progress-bar bg-primary"
                    :style="{ width: ((row.power - 2600) / 300 * 100) + '%' }"
                  ></div>
                </div>
              </td>
              <td>{{ row.pue }}</td>
              <td>{{ row.cooling }}%</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
