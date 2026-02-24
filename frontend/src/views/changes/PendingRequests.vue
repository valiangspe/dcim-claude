<template>
  <div class="pending-requests">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
    <div class="card">
      <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Pending Change Requests</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">+ New Request</button>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover">
            <thead>
              <tr class="table-light">
                <th>Request ID</th>
                <th>Requester</th>
                <th>Type</th>
                <th>Priority</th>
                <th>Status</th>
                <th>Requested Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="req in pendingRequests" :key="req.id">
                <td><strong>{{ req.requestId }}</strong></td>
                <td>{{ req.requester }}</td>
                <td>{{ req.type }}</td>
                <td>
                  <span :class="getPriorityBadgeClass(req.priority)">
                    {{ req.priority }}
                  </span>
                </td>
                <td>
                  <span :class="getStatusBadgeClass(req.status)">
                    {{ req.status }}
                  </span>
                </td>
                <td>{{ req.requestedDate }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(req)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(req.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Total pending: {{ pendingRequests.length }} requests
      </div>
    </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'New' }} Request</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Request ID</label>
              <input v-model="form.requestId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Requester</label>
              <input v-model="form.requester" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <select v-model="form.type" class="form-select">
                <option>Hardware</option>
                <option>Network</option>
                <option>Software</option>
                <option>Access</option>
                <option>Emergency</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Priority</label>
              <select v-model="form.priority" class="form-select">
                <option>Critical</option>
                <option>High</option>
                <option>Medium</option>
                <option>Low</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>Pending Review</option>
                <option>Waiting Approval</option>
                <option>In Progress</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Requested Date</label>
              <input v-model="form.requestedDate" type="date" class="form-control" />
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
import { changeRequestsApi, type ChangeRequest } from '../../services/api'

const pendingRequests = ref<ChangeRequest[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ChangeRequest | null>(null)

const defaultForm = { requestId: '', requester: '', type: 'Hardware', priority: 'Medium', status: 'Pending Review', requestedDate: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  pendingRequests.value = await changeRequestsApi.getAll()
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

function openEdit(req: ChangeRequest) {
  editing.value = req
  form.value = { requestId: req.requestId, requester: req.requester, type: req.type, priority: req.priority, status: req.status, requestedDate: req.requestedDate }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await changeRequestsApi.update(editing.value.id, form.value)
    else await changeRequestsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await changeRequestsApi.remove(id)
  await loadData()
}

const getPriorityBadgeClass = (priority: string): string => {
  const classes = 'badge'
  switch (priority) {
    case 'Critical':
      return `${classes} bg-danger`
    case 'High':
      return `${classes} bg-warning text-dark`
    case 'Medium':
      return `${classes} bg-info`
    case 'Low':
      return `${classes} bg-success`
    default:
      return `${classes} bg-secondary`
  }
}

const getStatusBadgeClass = (status: string): string => {
  const classes = 'badge'
  switch (status) {
    case 'Pending Review':
      return `${classes} bg-secondary`
    case 'Waiting Approval':
      return `${classes} bg-warning text-dark`
    case 'In Progress':
      return `${classes} bg-info`
    case 'Completed':
      return `${classes} bg-success`
    default:
      return `${classes} bg-light text-dark`
  }
}
</script>

<style scoped>
.pending-requests {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
