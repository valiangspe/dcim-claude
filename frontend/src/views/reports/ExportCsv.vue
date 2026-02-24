<template>
  <div class="export-csv">
    <div class="card">
      <div class="card-header bg-success text-white">
        <h5 class="mb-0">Export as CSV</h5>
      </div>
      <div class="card-body">
        <form @submit.prevent="handleExport">
          <div class="mb-4">
            <label class="form-label fw-bold">Select Data Category</label>
            <div class="list-group">
              <label v-for="category in categories" :key="category.id" class="list-group-item">
                <input
                  v-model="selectedCategory"
                  type="radio"
                  class="form-check-input me-2"
                  :value="category.id"
                />
                <strong>{{ category.name }}</strong>
                <small class="d-block text-muted ms-4">{{ category.description }}</small>
              </label>
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-bold">Date Range</label>
            <div class="row g-2">
              <div class="col-md-6">
                <input v-model="startDate" type="date" class="form-control" />
              </div>
              <div class="col-md-6">
                <input v-model="endDate" type="date" class="form-control" />
              </div>
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-bold">Include Filters</label>
            <div class="form-check">
              <input v-model="includeMetadata" type="checkbox" class="form-check-input" id="metadata" />
              <label class="form-check-label" for="metadata">Include metadata</label>
            </div>
            <div class="form-check">
              <input v-model="includeTimestamps" type="checkbox" class="form-check-input" id="timestamps" />
              <label class="form-check-label" for="timestamps">Include timestamps</label>
            </div>
          </div>

          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-success">
              <i class="bi bi-download me-2"></i>Download CSV
            </button>
            <button type="reset" class="btn btn-outline-secondary">Clear</button>
          </div>
        </form>
      </div>
      <div class="card-footer bg-light">
        <small class="text-muted">Export file size estimate: {{ selectedCategory ? '2-5 MB' : 'N/A' }}</small>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface Category {
  id: string
  name: string
  description: string
}

const selectedCategory = ref<string>('power')
const startDate = ref<string>('2026-01-01')
const endDate = ref<string>('2026-02-20')
const includeMetadata = ref<boolean>(true)
const includeTimestamps = ref<boolean>(true)

const categories: Category[] = [
  {
    id: 'power',
    name: 'Power Consumption Data',
    description: 'Historical power usage, costs, and efficiency metrics'
  },
  {
    id: 'cooling',
    name: 'Cooling System Data',
    description: 'Temperature readings and cooling efficiency data'
  },
  {
    id: 'space',
    name: 'Space Utilization Data',
    description: 'Rack and floor space occupancy information'
  },
  {
    id: 'network',
    name: 'Network Performance Data',
    description: 'Bandwidth usage and network metrics'
  },
  {
    id: 'assets',
    name: 'Asset Inventory Data',
    description: 'Equipment listing and depreciation tracking'
  },
  {
    id: 'alarms',
    name: 'Alarms & Events Data',
    description: 'System alerts and logged events'
  }
]

const handleExport = () => {
  const category = categories.find((c) => c.id === selectedCategory.value)
  alert(
    `Exporting ${category?.name}\nFrom: ${startDate.value}\nTo: ${endDate.value}\nMetadata: ${includeMetadata.value}\nTimestamps: ${includeTimestamps.value}`
  )
}
</script>

<style scoped>
.export-csv {
  padding: 1rem;
  max-width: 600px;
}

.list-group-item {
  cursor: pointer;
  transition: background-color 0.2s;
}

.list-group-item:hover {
  background-color: #f8f9fa;
}

.list-group-item input[type='radio']:checked ~ strong {
  color: #198754;
}
</style>
