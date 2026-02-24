<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { securityIncidentsApi, type SecurityIncident } from '../../services/api'

const incidents = ref<SecurityIncident[]>([])
const loading = ref(true)
const error = ref('')

const showModal = ref(false)
const editing = ref<SecurityIncident | null>(null)
const saving = ref(false)
const form = ref({ type: '', timestamp: '', location: '', severity: 'medium', status: 'active' })

const kpis = [
  { label: 'Security Incidents', value: '12', color: 'danger', trend: '+3 this month' },
  { label: 'Data Breaches', value: '0', color: 'success', trend: 'No incidents' },
  { label: 'Access Events', value: '8,247', color: 'primary', trend: '+324 today' },
  { label: 'Failed Auth', value: '34', color: 'warning', trend: '+8 today' },
  { label: 'Active Policies', value: '47', color: 'info', trend: '5 pending review' },
  { label: 'Compliance Score', value: '94%', color: 'success', trend: '+2% last week' },
]

const severityColor = {
  critical: 'danger',
  high: 'warning',
  medium: 'info',
  low: 'secondary',
} as const

const statusBadge = {
  active: 'danger',
  investigating: 'warning',
  resolved: 'success',
} as const

async function loadData() {
  incidents.value = await securityIncidentsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'Failed to load incidents'
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editing.value = null
  form.value = { type: '', timestamp: '', location: '', severity: 'medium', status: 'active' }
  showModal.value = true
}

function openEdit(incident: SecurityIncident) {
  editing.value = incident
  form.value = { type: incident.type, timestamp: incident.timestamp, location: incident.location, severity: incident.severity, status: incident.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await securityIncidentsApi.update(editing.value.id, form.value)
    else await securityIncidentsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await securityIncidentsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Security Overview</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>
    <template v-else>
    <!-- KPI Cards -->
    <div class="row g-3 mb-4">
      <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg-4 col-xl-2">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body">
            <h3 :class="`text-${kpi.color} mb-1`">{{ kpi.value }}</h3>
            <small class="text-muted d-block">{{ kpi.label }}</small>
            <small class="text-secondary">{{ kpi.trend }}</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Incidents -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
        <div class="d-flex align-items-center">
          <span>Recent Security Incidents</span>
          <span class="badge bg-danger ms-2">{{ incidents.length }}</span>
        </div>
        <button class="btn btn-sm btn-primary" @click="openCreate">Add Incident</button>
      </div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Type</th>
              <th>Location</th>
              <th>Timestamp</th>
              <th>Severity</th>
              <th>Status</th>
              <th style="width:80px">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="incident in incidents" :key="incident.id">
              <td class="fw-semibold">{{ incident.type }}</td>
              <td>{{ incident.location }}</td>
              <td class="text-muted">{{ incident.timestamp }}</td>
              <td><span class="badge" :class="`bg-${severityColor[incident.severity as keyof typeof severityColor]}`">{{ incident.severity }}</span></td>
              <td><span class="badge" :class="`bg-${statusBadge[incident.status as keyof typeof statusBadge]}`">{{ incident.status }}</span></td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(incident)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(incident.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Incident</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Timestamp</label>
              <input v-model="form.timestamp" type="datetime-local" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Location</label>
              <input v-model="form.location" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Severity</label>
              <select v-model="form.severity" class="form-select">
                <option value="critical">critical</option>
                <option value="high">high</option>
                <option value="medium">medium</option>
                <option value="low">low</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="active">active</option>
                <option value="investigating">investigating</option>
                <option value="resolved">resolved</option>
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
