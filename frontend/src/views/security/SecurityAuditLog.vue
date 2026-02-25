<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { securityAuditsApi, type SecurityAudit } from '../../services/api'

const auditLogs = ref<SecurityAudit[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<SecurityAudit | null>(null)
const defaultForm = { timestamp: '', user: '', action: '', resource: '', result: '', details: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  auditLogs.value = await securityAuditsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(log: SecurityAudit) {
  editing.value = log
  form.value = { timestamp: log.timestamp, user: log.user, action: log.action, resource: log.resource, result: log.result, details: log.details }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await securityAuditsApi.update(editing.value.id, form.value)
    else await securityAuditsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await securityAuditsApi.remove(id)
  await loadData()
}

const resultColor = {
  success: 'success',
  failed: 'danger',
} as const

const actionColor = {
  'Login': 'primary',
  'Failed Login Attempt': 'danger',
  'IP Whitelist Added': 'info',
  'VPN Session Started': 'success',
  'Report Generated': 'primary',
  'Role Modified': 'warning',
  'Policy Created': 'info',
  'Audit Settings Changed': 'secondary',
  'Access Granted': 'success',
  'Alert Acknowledged': 'info',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">Security Audit Log</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>

    <!-- Filter and Stats -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-success mb-1">{{ auditLogs.filter(l => l.result === 'success').length }}</h3>
            <small class="text-muted">Successful Actions</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-danger mb-1">{{ auditLogs.filter(l => l.result === 'failed').length }}</h3>
            <small class="text-muted">Failed Actions</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-info mb-1">{{ new Set(auditLogs.map(l => l.user)).size }}</h3>
            <small class="text-muted">Active Users</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-primary mb-1">90 days</h3>
            <small class="text-muted">Log Retention</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Audit Log Table -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
        <div class="d-flex align-items-center">
          <span>Audit Log Events</span>
          <span class="badge bg-primary ms-2">{{ auditLogs.length }}</span>
        </div>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Entry</button>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Timestamp</th>
                <th>User</th>
                <th>Action</th>
                <th>Resource</th>
                <th>Result</th>
                <th>Details</th>
                <th style="width:120px">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="log in auditLogs" :key="log.id">
                <td class="text-muted" style="font-size: 0.9rem;">{{ log.timestamp }}</td>
                <td><code class="fw-semibold">{{ log.user }}</code></td>
                <td>
                  <span class="badge" :class="`bg-${actionColor[log.action as keyof typeof actionColor] || 'secondary'}`">
                    {{ log.action }}
                  </span>
                </td>
                <td class="text-muted">{{ log.resource }}</td>
                <td><span class="badge" :class="`bg-${resultColor[log.result as keyof typeof resultColor]}`">{{ log.result }}</span></td>
                <td style="max-width: 250px;">
                  <small class="text-muted" title="log.details">{{ log.details }}</small>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(log)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(log.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Audit Entry</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Timestamp</label>
              <input v-model="form.timestamp" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">User</label>
              <input v-model="form.user" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Action</label>
              <input v-model="form.action" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Resource</label>
              <input v-model="form.resource" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Result</label>
              <select v-model="form.result" class="form-select">
                <option value="success">success</option>
                <option value="failed">failed</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Details</label>
              <input v-model="form.details" type="text" class="form-control" />
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
