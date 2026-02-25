<template>
  <div class="energy-storage">
    <h2 class="mb-4">Energy Storage Systems</h2>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <template v-else>
      <!-- Summary Cards -->
      <div class="row mb-4">
        <div v-for="m in metrics.slice(0, 3)" :key="m.id" class="col-md-4">
          <div class="card">
            <div class="card-body">
              <h5 class="card-title">{{ m.name }}</h5>
              <div class="display-5" :class="m.status === 'on-track' ? 'text-success' : m.status === 'warning' ? 'text-warning' : 'text-primary'">{{ m.value }} {{ m.unit }}</div>
              <p class="text-muted">Target: {{ m.target }}</p>
              <span class="badge" :class="m.status === 'on-track' ? 'bg-success' : m.status === 'warning' ? 'bg-warning text-dark' : 'bg-info'">{{ m.status }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- All Metrics Table -->
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Energy Storage Metrics</h5>
          <button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Metric</th>
                <th>Category</th>
                <th>Value</th>
                <th>Unit</th>
                <th>Target</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="m in metrics" :key="m.id">
                <td>{{ m.name }}</td>
                <td>{{ m.category }}</td>
                <td><strong>{{ m.value }}</strong></td>
                <td>{{ m.unit }}</td>
                <td>{{ m.target }}</td>
                <td>
                  <span class="badge" :class="m.status === 'on-track' ? 'bg-success' : m.status === 'warning' ? 'bg-warning text-dark' : 'bg-danger'">{{ m.status }}</span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(m)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(m.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'New' }} Metric</h5>
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
              <input v-model="form.value" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Unit</label>
              <input v-model="form.unit" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Target</label>
              <input v-model="form.target" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>on-track</option>
                <option>warning</option>
                <option>critical</option>
              </select>
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
import { sustainabilityMetricsApi, type SustainabilityMetric } from '../../services/api'

const allMetrics = ref<SustainabilityMetric[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<SustainabilityMetric | null>(null)

const PAGE = 'energy-storage'
const metrics = computed(() => allMetrics.value.filter(m => m.page === PAGE))

const defaultForm = { category: '', name: '', value: '', unit: '', target: '', status: 'on-track', page: PAGE }
const form = ref({ ...defaultForm })

async function loadData() {
  allMetrics.value = await sustainabilityMetricsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(m: SustainabilityMetric) {
  editing.value = m
  form.value = { category: m.category, name: m.name, value: m.value, unit: m.unit, target: m.target, status: m.status, page: PAGE }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await sustainabilityMetricsApi.update(editing.value.id, form.value)
    else await sustainabilityMetricsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await sustainabilityMetricsApi.remove(id)
  await loadData()
}
</script>

<style scoped>
.energy-storage {
  padding: 0;
}

.display-5 {
  font-weight: 700;
}
</style>
