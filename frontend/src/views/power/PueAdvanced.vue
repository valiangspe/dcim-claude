<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { pueMetricsApi, type PueMetric } from '../../services/api'

const metrics = ref<PueMetric[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PueMetric | null>(null)

const defaultForm = { name: '', value: 0, target: 0, stdDev: 0 }
const form = ref({ ...defaultForm })

const monthly = [
  { month: 'Sep 2025', pue1: 1.42, pue2: 1.26, pue3: 1.10 },
  { month: 'Oct 2025', pue1: 1.41, pue2: 1.25, pue3: 1.09 },
  { month: 'Nov 2025', pue1: 1.40, pue2: 1.24, pue3: 1.09 },
  { month: 'Dec 2025', pue1: 1.39, pue2: 1.23, pue3: 1.08 },
  { month: 'Jan 2026', pue1: 1.39, pue2: 1.23, pue3: 1.08 },
  { month: 'Feb 2026', pue1: 1.38, pue2: 1.22, pue3: 1.08 },
]

function pueColor(pue: number, target: number): string {
  return pue <= target ? 'success' : pue <= target + 0.05 ? 'warning' : 'danger'
}

async function loadData() {
  metrics.value = await pueMetricsApi.getAll()
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

function openEdit(item: PueMetric) {
  editing.value = item
  form.value = { name: item.name, value: item.value, target: item.target, stdDev: item.stdDev }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await pueMetricsApi.update(editing.value.id, form.value)
    } else {
      await pueMetricsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await pueMetricsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">PUE Advanced</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="m in metrics" :key="m.name" class="col-md-4">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body text-center">
              <h2 :class="`text-${pueColor(m.value, m.target)} mb-1`">{{ m.value.toFixed(2) }}</h2>
              <div class="fw-semibold">{{ m.name }}</div>
              <small class="text-muted">Target: {{ m.target.toFixed(2) }} &middot; &sigma; {{ m.stdDev.toFixed(2) }}</small>
            </div>
          </div>
        </div>
      </div>

      <!-- PUE Metrics Table with CRUD -->
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>PUE Metrics</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Name</th><th class="text-end">Value</th><th class="text-end">Target</th><th class="text-end">Std Dev</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="m in metrics" :key="m.id">
                <td class="fw-semibold">{{ m.name }}</td>
                <td class="text-end">{{ m.value.toFixed(2) }}</td>
                <td class="text-end">{{ m.target.toFixed(2) }}</td>
                <td class="text-end">{{ m.stdDev.toFixed(2) }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(m)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(m.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">6-Month Trend</div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Month</th><th class="text-end">PUE1</th><th class="text-end">PUE2</th><th class="text-end">PUE3</th></tr>
            </thead>
            <tbody>
              <tr v-for="r in monthly" :key="r.month">
                <td class="fw-semibold">{{ r.month }}</td>
                <td class="text-end">{{ r.pue1.toFixed(2) }}</td>
                <td class="text-end">{{ r.pue2.toFixed(2) }}</td>
                <td class="text-end">{{ r.pue3.toFixed(2) }}</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} PUE Metric</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Value</label>
              <input v-model.number="form.value" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Target</label>
              <input v-model.number="form.target" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Std Dev</label>
              <input v-model.number="form.stdDev" type="number" step="0.01" class="form-control" />
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
