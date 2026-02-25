<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { cabinetAirflowsApi, type CabinetAirflow } from '../../services/api'

const cabinets = ref<CabinetAirflow[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CabinetAirflow | null>(null)

const defaultForm = { cabinetId: '', cfm: 0, kw: 0, cfmPerKw: 0, pressure: 0, leakage: 0 }
const form = ref({ ...defaultForm })

function leakageBadge(l: number): string {
  if (l === 0) return 'bg-success'
  if (l <= 2) return 'bg-info text-dark'
  return 'bg-warning text-dark'
}

async function loadData() {
  cabinets.value = await cabinetAirflowsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: CabinetAirflow) {
  editing.value = item
  form.value = { cabinetId: item.cabinetId, cfm: item.cfm, kw: item.kw, cfmPerKw: item.cfmPerKw, pressure: item.pressure, leakage: item.leakage }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await cabinetAirflowsApi.update(editing.value.id, form.value)
    } else {
      await cabinetAirflowsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await cabinetAirflowsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Airflow Monitoring</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
            <div class="card-body"><h3 class="text-primary mb-0">6,510</h3><small class="text-muted">Total CFM</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
            <div class="card-body"><h3 class="text-info mb-0">98.4</h3><small class="text-muted">Avg CFM/kW</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-success bg-opacity-10">
            <div class="card-body"><h3 class="text-success mb-0">0.05</h3><small class="text-muted">Avg Pressure (inH2O)</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
            <div class="card-body"><h3 class="text-warning mb-0">2</h3><small class="text-muted">Medium Leakage</small></div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Per-Cabinet Airflow</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Cabinet</th><th class="text-end">CFM</th><th class="text-end">Power (kW)</th><th class="text-end">CFM/kW</th><th class="text-end">Pressure (inH2O)</th><th>Leakage</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="c in cabinets" :key="c.id">
                <td class="fw-semibold">{{ c.cabinetId }}</td>
                <td class="text-end">{{ c.cfm.toLocaleString() }}</td>
                <td class="text-end">{{ c.kw.toFixed(1) }}</td>
                <td class="text-end">{{ c.cfmPerKw.toFixed(1) }}</td>
                <td class="text-end">{{ c.pressure.toFixed(2) }}</td>
                <td><span class="badge" :class="leakageBadge(c.leakage)">{{ c.leakage }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(c)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(c.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Cabinet Airflow</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Cabinet ID</label>
              <input v-model="form.cabinetId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">CFM</label>
              <input v-model.number="form.cfm" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Power (kW)</label>
              <input v-model.number="form.kw" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">CFM/kW</label>
              <input v-model.number="form.cfmPerKw" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Pressure (inH2O)</label>
              <input v-model.number="form.pressure" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Leakage</label>
              <input v-model.number="form.leakage" type="number" class="form-control" />
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
