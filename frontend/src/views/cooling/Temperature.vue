<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { cabinetTemperaturesApi, type CabinetTemperature } from '../../services/api'

const cabinets = ref<CabinetTemperature[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CabinetTemperature | null>(null)

const defaultForm = { cabinetId: '', location: '', inlet: 0, outlet: 0, delta: 0, alarm: false }
const form = ref({ ...defaultForm })

function tempColor(temp: number): string {
  if (temp >= 25) return 'text-danger fw-semibold'
  if (temp >= 23) return 'text-warning'
  return ''
}

function outletColor(temp: number): string {
  if (temp >= 38) return 'text-danger fw-semibold'
  if (temp >= 35) return 'text-warning'
  return ''
}

async function loadData() {
  cabinets.value = await cabinetTemperaturesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: CabinetTemperature) {
  editing.value = item
  form.value = { cabinetId: item.cabinetId, location: item.location, inlet: item.inlet, outlet: item.outlet, delta: item.delta, alarm: item.alarm }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await cabinetTemperaturesApi.update(editing.value.id, form.value)
    } else {
      await cabinetTemperaturesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await cabinetTemperaturesApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Temperature Monitoring</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
            <div class="card-body"><h3 class="text-info mb-0">22.7°C</h3><small class="text-muted">Avg Inlet</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-danger bg-opacity-10">
            <div class="card-body"><h3 class="text-danger mb-0">34.3°C</h3><small class="text-muted">Avg Outlet</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
            <div class="card-body"><h3 class="text-warning mb-0">11.5°C</h3><small class="text-muted">Avg Delta-T</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center" :class="cabinets.filter(c => c.alarm).length > 0 ? 'bg-danger bg-opacity-10' : 'bg-success bg-opacity-10'">
            <div class="card-body">
              <h3 :class="cabinets.filter(c => c.alarm).length > 0 ? 'text-danger' : 'text-success'" class="mb-0">{{ cabinets.filter(c => c.alarm).length }}</h3>
              <small class="text-muted">Temp Alarms</small>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Per-Cabinet Temperature</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Cabinet</th><th>Location</th><th class="text-end">Inlet (°C)</th><th class="text-end">Outlet (°C)</th><th class="text-end">Delta-T (°C)</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="c in cabinets" :key="c.id" :class="c.alarm ? 'table-danger' : ''">
                <td class="fw-semibold">{{ c.cabinetId }}</td>
                <td class="text-muted">{{ c.location }}</td>
                <td class="text-end" :class="tempColor(c.inlet)">{{ c.inlet.toFixed(1) }}</td>
                <td class="text-end" :class="outletColor(c.outlet)">{{ c.outlet.toFixed(1) }}</td>
                <td class="text-end">{{ c.delta.toFixed(1) }}</td>
                <td>
                  <span v-if="c.alarm" class="badge bg-danger">ALARM</span>
                  <span v-else class="badge bg-success">OK</span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(c)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(c.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="small text-muted mt-3">ASHRAE recommended inlet range: 18°C – 27°C. Alarms triggered above 24.5°C inlet.</div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Cabinet Temperature</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Cabinet ID</label>
              <input v-model="form.cabinetId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Location</label>
              <input v-model="form.location" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Inlet (°C)</label>
              <input v-model.number="form.inlet" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Outlet (°C)</label>
              <input v-model.number="form.outlet" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Delta-T (°C)</label>
              <input v-model.number="form.delta" type="number" class="form-control" />
            </div>
            <div class="mb-3 form-check">
              <input v-model="form.alarm" type="checkbox" class="form-check-input" id="alarmCheck" />
              <label class="form-check-label" for="alarmCheck">Alarm</label>
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
