<template>
  <div class="capacity-reports">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="mb-3 text-end">
      <button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button>
    </div>
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
          <div class="card-footer bg-light d-flex justify-content-between align-items-center">
            <small class="text-muted">
              Forecast: {{ capacity.forecast }} ({{ capacity.forecastDays }} days)
            </small>
            <div>
              <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(capacity)">Edit</button>
              <button class="btn btn-sm btn-outline-danger" @click="remove(capacity.id)">Delete</button>
            </div>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Capacity Report</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Color</label>
              <input v-model="form.color" type="color" class="form-control form-control-color" />
            </div>
            <div class="mb-3">
              <label class="form-label">Current Usage (%)</label>
              <input v-model.number="form.currentUsage" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Available</label>
              <input v-model="form.available" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Total</label>
              <input v-model="form.total" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Forecast</label>
              <input v-model="form.forecast" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Forecast Days</label>
              <input v-model.number="form.forecastDays" type="number" class="form-control" />
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
import { ref, onMounted } from 'vue'
import { capacityReportsApi, type CapacityReport } from '../../services/api'

const capacities = ref<CapacityReport[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CapacityReport | null>(null)
const defaultForm = { name: '', color: '#0d6efd', currentUsage: 0, available: '', total: '', forecast: '', forecastDays: 0 }
const form = ref({ ...defaultForm })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: CapacityReport) {
  editing.value = item
  form.value = { name: item.name, color: item.color, currentUsage: item.currentUsage, available: item.available, total: item.total, forecast: item.forecast, forecastDays: item.forecastDays }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await capacityReportsApi.update(editing.value.id, form.value)
    else await capacityReportsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await capacityReportsApi.remove(id)
  await loadData()
}

async function loadData() {
  capacities.value = await capacityReportsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})
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
