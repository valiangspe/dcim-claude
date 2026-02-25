<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { serverPowerRecordsApi, type ServerPowerRecord } from '../../services/api'

const hours = ['00', '04', '08', '12', '16', '20']
const days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']

// Power values in kW (rows = days, cols = hours)
const data: number[][] = [
  [2.1, 1.9, 2.8, 3.4, 3.2, 2.6],
  [2.0, 1.8, 2.9, 3.5, 3.3, 2.5],
  [2.2, 2.0, 3.0, 3.6, 3.4, 2.7],
  [2.1, 1.9, 2.7, 3.3, 3.1, 2.5],
  [2.3, 2.1, 3.1, 3.7, 3.5, 2.8],
  [1.5, 1.3, 1.6, 1.8, 1.7, 1.4],
  [1.4, 1.2, 1.5, 1.7, 1.6, 1.3],
]

function cellColor(val: number): string {
  if (val >= 3.5) return 'bg-danger text-white'
  if (val >= 3.0) return 'bg-warning text-dark'
  if (val >= 2.0) return 'bg-info text-dark'
  return 'bg-success text-white'
}

const servers = ref<ServerPowerRecord[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ServerPowerRecord | null>(null)

const defaultForm = { name: '', rack: '', power: 0, cpu: 0, psu: '', status: 'online' }
const form = ref({ ...defaultForm })

async function loadData() {
  servers.value = await serverPowerRecordsApi.getAll()
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

function openEdit(item: ServerPowerRecord) {
  editing.value = item
  form.value = { name: item.name, rack: item.rack, power: item.power, cpu: item.cpu, psu: item.psu, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await serverPowerRecordsApi.update(editing.value.id, form.value)
    } else {
      await serverPowerRecordsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await serverPowerRecordsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Server Power Heatmap</h4>
    <p class="text-muted">Average power consumption (kW) by time of day and day of week.</p>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-body p-0">
          <table class="table table-bordered text-center mb-0">
            <thead class="table-light">
              <tr>
                <th></th>
                <th v-for="h in hours" :key="h">{{ h }}:00</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(row, di) in data" :key="days[di]">
                <td class="fw-semibold table-light">{{ days[di] }}</td>
                <td v-for="(val, hi) in row" :key="hi" :class="cellColor(val)" style="min-width: 70px;">
                  {{ val.toFixed(1) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="d-flex gap-3 small mb-4">
        <span><span class="badge bg-success">&nbsp;&nbsp;</span> &lt;2.0 kW</span>
        <span><span class="badge bg-info">&nbsp;&nbsp;</span> 2.0 - 3.0 kW</span>
        <span><span class="badge bg-warning">&nbsp;&nbsp;</span> 3.0 - 3.5 kW</span>
        <span><span class="badge bg-danger">&nbsp;&nbsp;</span> &ge;3.5 kW</span>
      </div>

      <!-- Server Power Records Table with CRUD -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Server Power Records</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Server</th><th>Rack</th><th>PSU</th><th class="text-end">Power (kW)</th><th class="text-end">CPU %</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="s in servers" :key="s.id">
                <td class="fw-semibold font-monospace">{{ s.name }}</td>
                <td>{{ s.rack }}</td>
                <td class="text-muted small">{{ s.psu }}</td>
                <td class="text-end">{{ s.power.toFixed(2) }}</td>
                <td class="text-end">{{ s.cpu }}%</td>
                <td><span class="badge" :class="s.status === 'online' ? 'bg-success' : 'bg-secondary'">{{ s.status }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(s)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(s.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Server Power Record</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Rack</label>
              <input v-model="form.rack" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Power (kW)</label>
              <input v-model.number="form.power" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">CPU (%)</label>
              <input v-model.number="form.cpu" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">PSU</label>
              <input v-model="form.psu" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>online</option>
                <option>offline</option>
                <option>standby</option>
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
