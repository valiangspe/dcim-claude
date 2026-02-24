<template>
  <div class="capacity-reports">
    <div class="row g-3">
      <div v-for="capacity in capacities" :key="capacity.id" class="col-md-4">
        <div class="card border-start border-4" :style="{ borderColor: capacity.color }">
          <div class="card-body">
            <h6 class="card-title fw-bold">{{ capacity.name }}</h6>
            <div class="mb-3">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <small class="text-muted">Current Usage</small>
                <strong>{{ capacity.currentUsage }}%</strong>
              </div>
              <div class="progress" style="height: 20px">
                <div
                  class="progress-bar"
                  :style="{ width: capacity.currentUsage + '%', backgroundColor: capacity.color }"
                  role="progressbar"
                ></div>
              </div>
            </div>
            <div class="row text-center">
              <div class="col-6">
                <small class="d-block text-muted">Available</small>
                <strong>{{ capacity.available }}</strong>
              </div>
              <div class="col-6">
                <small class="d-block text-muted">Total</small>
                <strong>{{ capacity.total }}</strong>
              </div>
            </div>
          </div>
          <div class="card-footer bg-light">
            <small class="text-muted">
              Forecast: {{ capacity.forecast }} ({{ capacity.forecastDays }} days)
            </small>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface CapacityMetric {
  id: string
  name: string
  color: string
  currentUsage: number
  available: string
  total: string
  forecast: string
  forecastDays: number
}

const capacities: CapacityMetric[] = [
  {
    id: '1',
    name: 'Power Capacity',
    color: '#ffc107',
    currentUsage: 72,
    available: '280 kW',
    total: '1000 kW',
    forecast: '90% usage',
    forecastDays: 45
  },
  {
    id: '2',
    name: 'Cooling Capacity',
    color: '#0dcaf0',
    currentUsage: 68,
    available: '320 kW',
    total: '1000 kW',
    forecast: '85% usage',
    forecastDays: 60
  },
  {
    id: '3',
    name: 'Space Capacity',
    color: '#198754',
    currentUsage: 82,
    available: '36 Racks',
    total: '200 Racks',
    forecast: '95% usage',
    forecastDays: 30
  },
  {
    id: '4',
    name: 'Network Bandwidth',
    color: '#0d6efd',
    currentUsage: 55,
    available: '4.5 Tbps',
    total: '10 Tbps',
    forecast: '70% usage',
    forecastDays: 90
  },
  {
    id: '5',
    name: 'Storage Capacity',
    color: '#6c757d',
    currentUsage: 78,
    available: '220 TB',
    total: '1000 TB',
    forecast: '92% usage',
    forecastDays: 40
  },
  {
    id: '6',
    name: 'Memory Resources',
    color: '#dc3545',
    currentUsage: 65,
    available: '350 GB',
    total: '1000 GB',
    forecast: '80% usage',
    forecastDays: 75
  }
]
</script>

<style scoped>
.capacity-reports {
  padding: 1rem;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-2px);
}
</style>
