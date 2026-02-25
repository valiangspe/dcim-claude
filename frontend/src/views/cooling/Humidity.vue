<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { cabinetHumiditiesApi, type CabinetHumidity } from '../../services/api'

const cabinets = ref<CabinetHumidity[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CabinetHumidity | null>(null)

const defaultForm = { cabinetId: '', location: '', rh: 0, dewpoint: 0, alarm: false }
const form = ref({ ...defaultForm })

function rhColor(rh: number): string {
  if (rh > 55 || rh < 40) return 'text-danger fw-semibold'
  if (rh > 50 || rh < 42) return 'text-warning'
  return ''
}

async function loadData() {
  cabinets.value = await cabinetHumiditiesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: CabinetHumidity) {
  editing.value = item
  form.value = { cabinetId: item.cabinetId, location: item.location, rh: item.rh, dewpoint: item.dewpoint, alarm: item.alarm }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await cabinetHumiditiesApi.update(editing.value.id, form.value)
    } else {
      await cabinetHumiditiesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await cabinetHumiditiesApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Humidity Monitoring</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
            <div class="card-body"><h3 class="text-primary mb-0">46.5% RH</h3><small class="text-muted">Avg Relative Humidity</small></div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
            <div class="card-body"><h3 class="text-info mb-0">11.1°C</h3><small class="text-muted">Avg Dew Point</small></div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-danger bg-opacity-10">
            <div class="card-body"><h3 class="text-danger mb-0">{{ cabinets.filter(c => c.alarm).length }}</h3><small class="text-muted">Humidity Alarms</small></div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Per-Cabinet Humidity</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Cabinet</th><th>Location</th><th class="text-end">RH (%)</th><th style="width:22%">Bar</th><th class="text-end">Dew Point (°C)</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="c in cabinets" :key="c.id" :class="c.alarm ? 'table-warning' : ''">
                <td class="fw-semibold">{{ c.cabinetId }}</td>
                <td class="text-muted">{{ c.location }}</td>
                <td class="text-end" :class="rhColor(c.rh)">{{ c.rh.toFixed(1) }}%</td>
                <td>
                  <div class="progress" style="height: 8px;">
                    <div class="progress-bar" :class="c.rh > 55 || c.rh < 40 ? 'bg-danger' : 'bg-primary'" :style="{ width: c.rh + '%' }"></div>
                  </div>
                </td>
                <td class="text-end">{{ c.dewpoint.toFixed(1) }}</td>
                <td>
                  <span v-if="c.alarm" class="badge bg-warning text-dark">ALARM</span>
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

      <div class="small text-muted mt-3">ASHRAE recommended RH range: 40% – 55%. Dew point max: 15°C.</div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Cabinet Humidity</h5>
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
              <label class="form-label">RH (%)</label>
              <input v-model.number="form.rh" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Dew Point (°C)</label>
              <input v-model.number="form.dewpoint" type="number" class="form-control" />
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
