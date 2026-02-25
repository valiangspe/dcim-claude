<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { accessLogsApi, type AccessLog } from '../../services/api'

const accessLogs = ref<AccessLog[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<AccessLog | null>(null)
const defaultForm = { timestamp: '', door: '', person: '', method: '', result: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  accessLogs.value = await accessLogsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(log: AccessLog) {
  editing.value = log
  form.value = { timestamp: log.timestamp, door: log.door, person: log.person, method: log.method, result: log.result }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await accessLogsApi.update(editing.value.id, form.value)
    else await accessLogsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await accessLogsApi.remove(id)
  await loadData()
}

const resultBadge = {
  allowed: 'success',
  denied: 'danger',
} as const

const methodBadge = {
  'RFID': 'primary',
  'Fingerprint': 'info',
  'Card': 'secondary',
  'Biometric': 'primary',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">Physical Security</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>

    <!-- Stats Row -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-success mb-1">{{ accessLogs.filter(l => l.result === 'allowed').length }}</h3>
            <small class="text-muted">Authorized Access</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-danger mb-1">{{ accessLogs.filter(l => l.result === 'denied').length }}</h3>
            <small class="text-muted">Denied Access</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-primary mb-1">24</h3>
            <small class="text-muted">Active Doors</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-info mb-1">98%</h3>
            <small class="text-muted">System Uptime</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Access Logs Table -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
        <span>Access Control Logs (Last 8 Events)</span>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Log</button>
      </div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Timestamp</th>
              <th>Door / Location</th>
              <th>Person</th>
              <th>Method</th>
              <th>Result</th>
              <th style="width:120px">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in accessLogs" :key="log.id">
              <td class="text-muted">{{ log.timestamp }}</td>
              <td class="fw-semibold">{{ log.door }}</td>
              <td>{{ log.person }}</td>
              <td><span class="badge" :class="`bg-${methodBadge[log.method as keyof typeof methodBadge]}`">{{ log.method }}</span></td>
              <td><span class="badge" :class="`bg-${resultBadge[log.result as keyof typeof resultBadge]}`">{{ log.result }}</span></td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(log)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(log.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Access Log</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Timestamp</label>
              <input v-model="form.timestamp" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Door / Location</label>
              <input v-model="form.door" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Person</label>
              <input v-model="form.person" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Method</label>
              <select v-model="form.method" class="form-select">
                <option value="RFID">RFID</option>
                <option value="Fingerprint">Fingerprint</option>
                <option value="Card">Card</option>
                <option value="Biometric">Biometric</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Result</label>
              <select v-model="form.result" class="form-select">
                <option value="allowed">allowed</option>
                <option value="denied">denied</option>
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
