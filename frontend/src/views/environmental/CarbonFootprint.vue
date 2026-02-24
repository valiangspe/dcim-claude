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
            <div class="display-4 text-danger">{{ totalEmissions.toFixed(0) }}</div>
            <p class="text-muted">Tonnes per year</p>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Monthly Average</h5>
            <div class="display-4 text-info">{{ monthlyAverage.toFixed(1) }}</div>
            <p class="text-muted">Tonnes per month</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Metrics Table with CRUD -->
    <div class="card mb-4">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Environmental Metrics</h5>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Metric</button>
      </div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Category</th>
              <th>Name</th>
              <th>Value</th>
              <th>Unit</th>
              <th>Month</th>
              <th>Year</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="m in metrics" :key="m.id">
              <td>{{ m.category }}</td>
              <td class="fw-semibold">{{ m.name }}</td>
              <td>{{ m.value }}</td>
              <td>{{ m.unit }}</td>
              <td>{{ monthNames[m.month - 1] || m.month }}</td>
              <td>{{ m.year }}</td>
              <td>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(m)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(m.id)">Delete</button>
              </td>
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
            <div class="progress-bar bg-danger" role="progressbar" style="width: 100%"></div>
          </div>
        </div>
        <div class="mb-3">
          <div class="d-flex justify-content-between mb-2">
            <span>2024</span>
            <span class="fw-bold">152 tonnes</span>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-warning" role="progressbar" style="width: 94%"></div>
          </div>
        </div>
        <div>
          <div class="d-flex justify-content-between mb-2">
            <span>2025</span>
            <span class="fw-bold">142 tonnes</span>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-success" role="progressbar" style="width: 88%"></div>
          </div>
        </div>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Metric</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Category</label>
              <input v-model="form.category" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Value</label>
              <input v-model.number="form.value" type="number" step="0.1" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Unit</label>
              <input v-model="form.unit" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Month</label>
              <input v-model.number="form.month" type="number" min="1" max="12" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Year</label>
              <input v-model.number="form.year" type="number" class="form-control" />
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { environmentalMetricsApi, type EnvironmentalMetric } from '../../services/api'

const metrics = ref<EnvironmentalMetric[]>([])
const loading = ref(true)
const error = ref('')
const showModal = ref(false)
const saving = ref(false)
const editing = ref<EnvironmentalMetric | null>(null)

const defaultForm = { category: 'carbon', name: '', value: 0, unit: 'tonnes', month: 1, year: 2025 }
const form = ref({ ...defaultForm })

const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']

const totalEmissions = computed(() => metrics.value.reduce((a, m) => a + m.value, 0))
const monthlyAverage = computed(() => {
  const count = metrics.value.length
  return count > 0 ? totalEmissions.value / count : 0
})

async function loadData() {
  metrics.value = await environmentalMetricsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'Failed to load metrics'
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(metric: EnvironmentalMetric) {
  editing.value = metric
  form.value = { category: metric.category, name: metric.name, value: metric.value, unit: metric.unit, month: metric.month, year: metric.year }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await environmentalMetricsApi.update(editing.value.id, form.value)
    else await environmentalMetricsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await environmentalMetricsApi.remove(id)
  await loadData()
}
</script>

<style scoped>
.carbon-footprint {
  padding: 0;
}

.display-4 {
  font-weight: 700;
}
</style>
