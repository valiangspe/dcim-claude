<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { dcIncidentsApi, type DcIncident } from '../../services/api'

const incidents = ref<DcIncident[]>([])
const loading = ref(true)
const error = ref('')
const showModal = ref(false)
const editing = ref<DcIncident | null>(null)
const saving = ref(false)
const filterSeverity = ref('')
const filterStatus = ref('')

const form = ref({
  title: '',
  category: '',
  severity: 'medium',
  status: 'open',
  affectedSystem: '',
  site: '',
  detectedAt: '',
  resolvedAt: '',
  reportedBy: '',
  assignedTo: '',
  rootCause: '',
  impact: '',
  resolution: '',
  downtimeMinutes: 0,
  isoClass: '',
})

const categories = ['Power Failure', 'Cooling Failure', 'Network Outage', 'Physical Security', 'Environmental', 'Hardware Failure', 'Software Failure', 'Human Error', 'Other']

const severityColors: Record<string, string> = {
  critical: 'danger',
  high: 'warning',
  medium: 'info',
  low: 'secondary',
}

const statusColors: Record<string, string> = {
  open: 'danger',
  investigating: 'warning',
  'in-progress': 'primary',
  resolved: 'success',
  closed: 'secondary',
}

const kpis = computed(() => {
  const total = incidents.value.length
  const open = incidents.value.filter(i => i.status === 'open' || i.status === 'investigating' || i.status === 'in-progress').length
  const critical = incidents.value.filter(i => i.severity === 'critical').length
  const resolved = incidents.value.filter(i => i.status === 'resolved' || i.status === 'closed').length
  const totalDowntime = incidents.value.reduce((s, i) => s + (i.downtimeMinutes || 0), 0)
  const resolvedWithTime = incidents.value.filter(i => i.detectedAt && i.resolvedAt)
  const mttr = resolvedWithTime.length > 0
    ? resolvedWithTime.reduce((s, i) => {
        const diff = new Date(i.resolvedAt).getTime() - new Date(i.detectedAt).getTime()
        return s + diff / 60000
      }, 0) / resolvedWithTime.length
    : null
  return [
    { label: 'Open Incidents', value: String(open), color: open > 0 ? 'danger' : 'success', trend: 'Require action' },
    { label: 'Critical', value: String(critical), color: critical > 0 ? 'danger' : 'success', trend: 'Highest severity' },
    { label: 'Resolved', value: String(resolved), color: 'success', trend: 'Closed out' },
    { label: 'Total Incidents', value: String(total), color: 'primary', trend: 'All time' },
    { label: 'Total Downtime', value: totalDowntime > 0 ? `${Math.floor(totalDowntime / 60)}h ${totalDowntime % 60}m` : '0m', color: 'warning', trend: 'Infrastructure impact' },
    { label: 'Avg MTTR', value: mttr !== null ? `${Math.round(mttr)}m` : '—', color: 'info', trend: 'Mean time to resolve' },
  ]
})

const filtered = computed(() => incidents.value.filter(i => {
  if (filterSeverity.value && i.severity !== filterSeverity.value) return false
  if (filterStatus.value && i.status !== filterStatus.value) return false
  return true
}))

async function loadData() {
  incidents.value = await dcIncidentsApi.getAll()
}

onMounted(async () => {
  try { await loadData() }
  catch (e: unknown) { error.value = e instanceof Error ? e.message : 'Failed to load incidents' }
  finally { loading.value = false }
})

function openCreate() {
  editing.value = null
  form.value = { title: '', category: '', severity: 'medium', status: 'open', affectedSystem: '', site: '', detectedAt: '', resolvedAt: '', reportedBy: '', assignedTo: '', rootCause: '', impact: '', resolution: '', downtimeMinutes: 0, isoClass: '' }
  showModal.value = true
}

function openEdit(i: DcIncident) {
  editing.value = i
  form.value = { title: i.title, category: i.category, severity: i.severity, status: i.status, affectedSystem: i.affectedSystem, site: i.site, detectedAt: i.detectedAt, resolvedAt: i.resolvedAt, reportedBy: i.reportedBy, assignedTo: i.assignedTo, rootCause: i.rootCause, impact: i.impact, resolution: i.resolution, downtimeMinutes: i.downtimeMinutes, isoClass: i.isoClass }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await dcIncidentsApi.update(editing.value.id, form.value)
    else await dcIncidentsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(id: number) {
  if (!confirm('Delete this incident?')) return
  await dcIncidentsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-1">Incident Management</h4>
    <p class="text-muted small mb-4">Log, track and resolve data centre incidents — with root cause analysis and ISO availability impact</p>

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

      <!-- Table -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent d-flex align-items-center justify-content-between flex-wrap gap-2">
          <span class="fw-semibold">Incidents</span>
          <div class="d-flex gap-2 align-items-center flex-wrap">
            <select v-model="filterSeverity" class="form-select form-select-sm" style="width:140px">
              <option value="">All Severities</option>
              <option value="critical">Critical</option>
              <option value="high">High</option>
              <option value="medium">Medium</option>
              <option value="low">Low</option>
            </select>
            <select v-model="filterStatus" class="form-select form-select-sm" style="width:150px">
              <option value="">All Statuses</option>
              <option value="open">Open</option>
              <option value="investigating">Investigating</option>
              <option value="in-progress">In Progress</option>
              <option value="resolved">Resolved</option>
              <option value="closed">Closed</option>
            </select>
            <button class="btn btn-sm btn-primary" @click="openCreate">Log Incident</button>
          </div>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Title</th>
                <th>Category</th>
                <th>Site / System</th>
                <th>Severity</th>
                <th>Status</th>
                <th>Detected</th>
                <th>Downtime</th>
                <th>ISO Class</th>
                <th>Assigned To</th>
                <th style="width:80px">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="inc in filtered" :key="inc.id">
                <td>
                  <div class="fw-semibold">{{ inc.title }}</div>
                  <small class="text-muted">{{ inc.impact }}</small>
                </td>
                <td><span class="badge bg-light text-dark border">{{ inc.category }}</span></td>
                <td>
                  <div>{{ inc.site }}</div>
                  <small class="text-muted">{{ inc.affectedSystem }}</small>
                </td>
                <td><span class="badge" :class="`bg-${severityColors[inc.severity]}`">{{ inc.severity }}</span></td>
                <td><span class="badge" :class="`bg-${statusColors[inc.status]}`">{{ inc.status }}</span></td>
                <td class="text-muted small">{{ inc.detectedAt || '—' }}</td>
                <td :class="inc.downtimeMinutes > 0 ? 'text-danger' : 'text-muted'">
                  {{ inc.downtimeMinutes > 0 ? `${Math.floor(inc.downtimeMinutes / 60)}h ${inc.downtimeMinutes % 60}m` : '—' }}
                </td>
                <td>
                  <span v-if="inc.isoClass" class="badge bg-primary">Class {{ inc.isoClass }}</span>
                  <span v-else class="text-muted">—</span>
                </td>
                <td>{{ inc.assignedTo || '—' }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(inc)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(inc.id)">Delete</button>
                </td>
              </tr>
              <tr v-if="filtered.length === 0">
                <td colspan="10" class="text-center text-muted py-4">No incidents recorded. Log an incident to begin tracking.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Log' }} Incident</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="row g-3">
              <div class="col-12">
                <label class="form-label">Title</label>
                <input v-model="form.title" type="text" class="form-control" placeholder="Brief incident title" />
              </div>
              <div class="col-4">
                <label class="form-label">Category</label>
                <select v-model="form.category" class="form-select">
                  <option value="">Select</option>
                  <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                </select>
              </div>
              <div class="col-4">
                <label class="form-label">Severity</label>
                <select v-model="form.severity" class="form-select">
                  <option value="critical">Critical</option>
                  <option value="high">High</option>
                  <option value="medium">Medium</option>
                  <option value="low">Low</option>
                </select>
              </div>
              <div class="col-4">
                <label class="form-label">Status</label>
                <select v-model="form.status" class="form-select">
                  <option value="open">Open</option>
                  <option value="investigating">Investigating</option>
                  <option value="in-progress">In Progress</option>
                  <option value="resolved">Resolved</option>
                  <option value="closed">Closed</option>
                </select>
              </div>
              <div class="col-6">
                <label class="form-label">Site</label>
                <input v-model="form.site" type="text" class="form-control" />
              </div>
              <div class="col-6">
                <label class="form-label">Affected System</label>
                <input v-model="form.affectedSystem" type="text" class="form-control" />
              </div>
              <div class="col-6">
                <label class="form-label">Detected At</label>
                <input v-model="form.detectedAt" type="datetime-local" class="form-control" />
              </div>
              <div class="col-6">
                <label class="form-label">Resolved At</label>
                <input v-model="form.resolvedAt" type="datetime-local" class="form-control" />
              </div>
              <div class="col-4">
                <label class="form-label">Reported By</label>
                <input v-model="form.reportedBy" type="text" class="form-control" />
              </div>
              <div class="col-4">
                <label class="form-label">Assigned To</label>
                <input v-model="form.assignedTo" type="text" class="form-control" />
              </div>
              <div class="col-4">
                <label class="form-label">Downtime (minutes)</label>
                <input v-model.number="form.downtimeMinutes" type="number" class="form-control" min="0" />
              </div>
              <div class="col-4">
                <label class="form-label">ISO Class Impact</label>
                <select v-model="form.isoClass" class="form-select">
                  <option value="">None</option>
                  <option value="1">Class 1</option>
                  <option value="2">Class 2</option>
                  <option value="3">Class 3</option>
                  <option value="4">Class 4</option>
                </select>
              </div>
              <div class="col-8">
                <label class="form-label">Impact Description</label>
                <input v-model="form.impact" type="text" class="form-control" />
              </div>
              <div class="col-12">
                <label class="form-label">Root Cause</label>
                <textarea v-model="form.rootCause" class="form-control" rows="2"></textarea>
              </div>
              <div class="col-12">
                <label class="form-label">Resolution</label>
                <textarea v-model="form.resolution" class="form-control" rows="2"></textarea>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
