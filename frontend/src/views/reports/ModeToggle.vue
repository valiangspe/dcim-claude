<template>
  <div class="mode-toggle">
    <div class="card">
      <div class="card-header bg-info text-white">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Report Display Mode</h5>
          <div class="btn-group btn-group-sm" role="group">
            <input v-model="viewMode" type="radio" class="btn-check" id="concise" value="concise" />
            <label class="btn btn-light" for="concise">Concise</label>

            <input v-model="viewMode" type="radio" class="btn-check" id="detailed" value="detailed" />
            <label class="btn btn-light" for="detailed">Detailed</label>
          </div>
        </div>
      </div>
      <div class="card-body">
        <!-- CONCISE VIEW -->
        <div v-if="viewMode === 'concise'">
          <div class="row g-3">
            <div v-for="metric in conciseMetrics" :key="metric.id" class="col-md-4">
              <div class="text-center">
                <h6 class="text-muted">{{ metric.name }}</h6>
                <h3 :style="{ color: metric.color }">{{ metric.value }}</h3>
                <small class="text-muted">{{ metric.unit }}</small>
              </div>
            </div>
          </div>
          <hr class="my-4" />
          <div class="row g-3">
            <div v-for="status in conciseStatus" :key="status.id" class="col-md-4 text-center">
              <span :class="getStatusBadgeClass(status.status)" class="badge fs-6">
                {{ status.label }}: {{ status.value }}
              </span>
            </div>
          </div>
        </div>

        <!-- DETAILED VIEW -->
        <div v-else>
          <div class="table-responsive">
            <table class="table table-sm">
              <thead>
                <tr class="table-light">
                  <th>Metric</th>
                  <th>Current</th>
                  <th>Average</th>
                  <th>Max</th>
                  <th>Min</th>
                  <th>Trend</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="metric in detailedMetrics" :key="metric.id">
                  <td><strong>{{ metric.name }}</strong></td>
                  <td>
                    <span class="badge" :style="{ backgroundColor: metric.color }">
                      {{ metric.current }}
                    </span>
                  </td>
                  <td>{{ metric.average }}</td>
                  <td>{{ metric.max }}</td>
                  <td>{{ metric.min }}</td>
                  <td>
                    <span :class="getTrendBadgeClass(metric.trend)">
                      {{ metric.trend }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <hr class="my-4" />

          <h6 class="fw-bold mb-3">Performance Summary</h6>
          <div v-for="summary in detailedSummary" :key="summary.id" class="mb-3">
            <div class="d-flex justify-content-between align-items-center mb-1">
              <small class="fw-semibold">{{ summary.name }}</small>
              <small class="text-muted">{{ summary.percentage }}%</small>
            </div>
            <div class="progress" style="height: 20px">
              <div
                class="progress-bar"
                :style="{ width: summary.percentage + '%', backgroundColor: summary.color }"
                role="progressbar"
              ></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface ConciseMetric {
  id: string
  name: string
  value: string
  unit: string
  color: string
}

interface ConciseStatus {
  id: string
  label: string
  value: string
  status: string
}

interface DetailedMetric {
  id: string
  name: string
  current: string
  average: string
  max: string
  min: string
  trend: string
  color: string
}

interface SummaryMetric {
  id: string
  name: string
  percentage: number
  color: string
}

const viewMode = ref<string>('concise')

const conciseMetrics: ConciseMetric[] = [
  { id: '1', name: 'Power Usage', value: '720', unit: 'kW', color: '#ffc107' },
  { id: '2', name: 'Avg Temperature', value: '24.5', unit: '°C', color: '#0dcaf0' },
  { id: '3', name: 'Space Utilization', value: '82%', unit: 'capacity', color: '#198754' }
]

const conciseStatus: ConciseStatus[] = [
  { id: '1', label: 'Status', value: 'Healthy', status: 'success' },
  { id: '2', label: 'Alerts', value: '2 Minor', status: 'warning' },
  { id: '3', label: 'Capacity', value: 'Good', status: 'info' }
]

const detailedMetrics: DetailedMetric[] = [
  {
    id: '1',
    name: 'Power Consumption',
    current: '720 kW',
    average: '685 kW',
    max: '825 kW',
    min: '580 kW',
    trend: 'Stable',
    color: '#ffc107'
  },
  {
    id: '2',
    name: 'Temperature',
    current: '24.5°C',
    average: '23.8°C',
    max: '28.2°C',
    min: '19.5°C',
    trend: 'Down',
    color: '#0dcaf0'
  },
  {
    id: '3',
    name: 'Humidity',
    current: '45%',
    average: '42%',
    max: '52%',
    min: '38%',
    trend: 'Up',
    color: '#198754'
  },
  {
    id: '4',
    name: 'Network Bandwidth',
    current: '5.2 Tbps',
    average: '4.8 Tbps',
    max: '6.1 Tbps',
    min: '3.9 Tbps',
    trend: 'Stable',
    color: '#0d6efd'
  },
  {
    id: '5',
    name: 'Rack Utilization',
    current: '82%',
    average: '78%',
    max: '91%',
    min: '72%',
    trend: 'Up',
    color: '#6c757d'
  }
]

const detailedSummary: SummaryMetric[] = [
  { id: '1', name: 'Infrastructure Health', percentage: 92, color: '#198754' },
  { id: '2', name: 'Capacity Headroom', percentage: 68, color: '#0dcaf0' },
  { id: '3', name: 'Compliance Score', percentage: 95, color: '#0d6efd' }
]

const getStatusBadgeClass = (status: string): string => {
  const classes = 'badge'
  switch (status) {
    case 'success':
      return `${classes} bg-success`
    case 'warning':
      return `${classes} bg-warning text-dark`
    case 'info':
      return `${classes} bg-info`
    default:
      return `${classes} bg-secondary`
  }
}

const getTrendBadgeClass = (trend: string): string => {
  const classes = 'badge'
  switch (trend) {
    case 'Up':
      return `${classes} bg-danger`
    case 'Down':
      return `${classes} bg-success`
    case 'Stable':
      return `${classes} bg-info`
    default:
      return `${classes} bg-secondary`
  }
}
</script>

<style scoped>
.mode-toggle {
  padding: 1rem;
}

.table-responsive {
  max-height: 500px;
  overflow-y: auto;
}
</style>
