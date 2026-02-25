<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { coolingMetricsApi, type CoolingMetric } from '../../services/api'

const metrics = ref<CoolingMetric[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CoolingMetric | null>(null)

const defaultForm = { name: '', value: 0, unit: '', target: 0, description: '' }
const form = ref({ ...defaultForm })

const monthly = [
  { month: 'Sep 2025', cer: 0.82, puec: 1.19, eer: 3.6, cop: 3.9 },
  { month: 'Oct 2025', cer: 0.83, puec: 1.18, eer: 3.7, cop: 4.0 },
  { month: 'Nov 2025', cer: 0.84, puec: 1.18, eer: 3.7, cop: 4.1 },
  { month: 'Dec 2025', cer: 0.84, puec: 1.17, eer: 3.8, cop: 4.1 },
  { month: 'Jan 2026', cer: 0.85, puec: 1.17, eer: 3.8, cop: 4.2 },
  { month: 'Feb 2026', cer: 0.85, puec: 1.17, eer: 3.8, cop: 4.2 },
]

function vsTarget(val: number, target: number, higherBetter: boolean): string {
  return higherBetter ? (val >= target ? 'text-success' : 'text-danger') : (val <= target ? 'text-success' : 'text-danger')
}

async function loadData() {
  metrics.value = await coolingMetricsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: CoolingMetric) {
  editing.value = item
  form.value = { name: item.name, value: item.value, unit: item.unit, target: item.target, description: item.description }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await coolingMetricsApi.update(editing.value.id, form.value)
    } else {
      await coolingMetricsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await coolingMetricsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cooling Efficiency</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="m in metrics" :key="m.id" class="col-md-6 col-xl-3">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body text-center">
              <h2 class="text-primary mb-1">{{ m.value }}<small v-if="m.unit" class="text-muted fs-6 ms-1">{{ m.unit }}</small></h2>
              <div class="fw-semibold small">{{ m.name }}</div>
              <small class="text-muted">Target: {{ m.target }} &middot; {{ m.description }}</small>
              <div class="mt-2">
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(m)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(m.id)">Delete</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>6-Month Trend</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Metric</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Month</th><th class="text-end">CER</th><th class="text-end">PUEc</th><th class="text-end">EER</th><th class="text-end">COP</th></tr>
            </thead>
            <tbody>
              <tr v-for="r in monthly" :key="r.month">
                <td class="fw-semibold">{{ r.month }}</td>
                <td class="text-end" :class="vsTarget(r.cer, 0.90, true)">{{ r.cer.toFixed(2) }}</td>
                <td class="text-end" :class="vsTarget(r.puec, 1.15, false)">{{ r.puec.toFixed(2) }}</td>
                <td class="text-end" :class="vsTarget(r.eer, 4.0, true)">{{ r.eer.toFixed(1) }}</td>
                <td class="text-end" :class="vsTarget(r.cop, 4.5, true)">{{ r.cop.toFixed(1) }}</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Metric</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Value</label>
              <input v-model.number="form.value" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Unit</label>
              <input v-model="form.unit" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Target</label>
              <input v-model.number="form.target" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <input v-model="form.description" type="text" class="form-control" />
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
