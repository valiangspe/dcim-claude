<template>
  <div class="audit-log">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else class="card">
      <div class="card-header bg-secondary text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Change Audit Log</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">+ Add</button>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-sm table-hover">
            <thead>
              <tr class="table-light">
                <th>Log ID</th>
                <th>Change Request</th>
                <th>Action</th>
                <th>User</th>
                <th>Timestamp</th>
                <th>Details</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="log in auditLogs" :key="log.id">
                <td><small>{{ log.auditId }}</small></td>
                <td><strong>{{ log.changeRequest }}</strong></td>
                <td>
                  <span :class="getActionBadgeClass(log.action)">
                    {{ log.action }}
                  </span>
                </td>
                <td>{{ log.user }}</td>
                <td><small>{{ log.timestamp }}</small></td>
                <td>{{ log.details }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(log)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(log.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Total audit entries: {{ auditLogs.length }}
      </div>
    </div>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'New' }} Audit Entry</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Audit ID</label>
              <input v-model="form.auditId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Change Request</label>
              <input v-model="form.changeRequest" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Action</label>
              <select v-model="form.action" class="form-select">
                <option>Created</option>
                <option>Updated</option>
                <option>Approved</option>
                <option>Rejected</option>
                <option>Completed</option>
                <option>Verified</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">User</label>
              <input v-model="form.user" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Timestamp</label>
              <input v-model="form.timestamp" type="text" class="form-control" />
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

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { changeAuditsApi, type ChangeAudit } from '../../services/api'

const auditLogs = ref<ChangeAudit[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ChangeAudit | null>(null)

const defaultForm = { auditId: '', changeRequest: '', action: 'Created', user: '', timestamp: '', details: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  auditLogs.value = await changeAuditsApi.getAll()
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

function openEdit(log: ChangeAudit) {
  editing.value = log
  form.value = { auditId: log.auditId, changeRequest: log.changeRequest, action: log.action, user: log.user, timestamp: log.timestamp, details: log.details }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await changeAuditsApi.update(editing.value.id, form.value)
    else await changeAuditsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await changeAuditsApi.remove(id)
  await loadData()
}

const getActionBadgeClass = (action: string): string => {
  const classes = 'badge'
  switch (action) {
    case 'Created':
      return `${classes} bg-info`
    case 'Updated':
      return `${classes} bg-primary`
    case 'Approved':
      return `${classes} bg-success`
    case 'Rejected':
      return `${classes} bg-danger`
    case 'Completed':
      return `${classes} bg-success`
    case 'Verified':
      return `${classes} bg-primary`
    default:
      return `${classes} bg-secondary`
  }
}
</script>

<style scoped>
.audit-log {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
