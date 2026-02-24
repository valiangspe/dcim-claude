<template>
  <div class="carbon-footprint">
    <h2 class="mb-4">Carbon Footprint</h2>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>
    <template v-else>
    <!-- Total CO2e Card -->
    <div class="row mb-4">
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Total CO2e Emissions</h5>
            <div class="display-4 text-danger">142</div>
            <p class="text-muted">Tonnes per year</p>
            <small class="text-success">↓ 12% vs last year</small>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Monthly Average</h5>
            <div class="display-4 text-info">11.8</div>
            <p class="text-muted">Tonnes per month</p>
            <small class="text-warning">Trending stable</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Monthly Breakdown Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Monthly Breakdown</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Month</th>
              <th>CO2e (tonnes)</th>
              <th>Trend</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in monthlyBreakdown" :key="row.month">
              <td>{{ row.month }}</td>
              <td>{{ row.value }}</td>
              <td><span :class="row.trend.class">{{ row.trend.text }}</span></td>
              <td><span class="badge" :class="row.status.class">{{ row.status.label }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Reduction Trend -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Reduction Trend (YoY)</h5>
      </div>
      <div class="card-body">
        <div class="mb-3">
          <div class="d-flex justify-content-between mb-2">
            <span>2023</span>
            <span class="fw-bold">162 tonnes</span>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-danger" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
          </div>
        </div>
        <div class="mb-3">
          <div class="d-flex justify-content-between mb-2">
            <span>2024</span>
            <span class="fw-bold">152 tonnes</span>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-warning" role="progressbar" style="width: 94%" aria-valuenow="94" aria-valuemin="0" aria-valuemax="100"></div>
          </div>
        </div>
        <div>
          <div class="d-flex justify-content-between mb-2">
            <span>2025</span>
            <span class="fw-bold">142 tonnes</span>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-success" role="progressbar" style="width: 88%" aria-valuenow="88" aria-valuemin="0" aria-valuemax="100"></div>
          </div>
        </div>
      </div>
    </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { environmentalMetricsApi, type EnvironmentalMetric } from '../../services/api'

const metrics = ref<EnvironmentalMetric[]>([])
const loading = ref(true)
const error = ref('')

const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']

const monthlyBreakdown = computed(() => {
  const co2Metrics = metrics.value.filter(m => m.unit === 'tonnes' || m.category === 'carbon')
  return co2Metrics.map(m => {
    const trend = m.value <= 11.5 ? { text: '↓ good', class: 'text-success' } : { text: '↑', class: 'text-danger' }
    const status = m.value <= 11.0 ? { label: 'Good', class: 'bg-success' }
      : m.value <= 12.0 ? { label: 'On Track', class: 'bg-info' }
      : { label: 'Above Avg', class: 'bg-warning' }
    return {
      month: monthNames[m.month - 1] || `Month ${m.month}`,
      value: m.value,
      trend,
      status,
    }
  })
})

onMounted(async () => {
  try {
    metrics.value = await environmentalMetricsApi.getAll()
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'Failed to load metrics'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.carbon-footprint {
  padding: 0;
}

.display-4 {
  font-weight: 700;
}
</style>
