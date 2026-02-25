<template>
  <div class="completed-requests">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else class="card">
      <div class="card-header bg-success text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Completed Change Requests</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">+ Add</button>
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
                <th>Requested Date</th>
                <th>Completion Date</th>
                <th>Duration (Days)</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="req in completedRequests" :key="req.id">
                <td><strong>{{ req.requestId }}</strong></td>
                <td>{{ req.requester }}</td>
                <td>{{ req.type }}</td>
                <td>
                  <span :class="getPriorityBadgeClass(req.priority)">
                    {{ req.priority }}
                  </span>
                </td>
                <td>{{ req.requestedDate }}</td>
                <td>{{ req.completionDate }}</td>
                <td>
                  <span class="badge bg-info">{{ req.duration }}</span>
                </td>
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
        Total completed: {{ completedRequests.length }} requests
      </div>
    </div>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'New' }} Completed Request</h5>
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
              <input v-model="form.type" type="text" class="form-control" />
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
              <label class="form-label">Requested Date</label>
              <input v-model="form.requestedDate" type="date" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Completion Date</label>
              <input v-model="form.completionDate" type="date" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Duration (Days)</label>
              <input v-model.number="form.duration" type="number" class="form-control" />
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
import { completedRequestsApi, type CompletedRequest } from '../../services/api'

const completedRequests = ref<CompletedRequest[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CompletedRequest | null>(null)

const defaultForm = { requestId: '', requester: '', type: '', priority: 'Medium', requestedDate: '', completionDate: '', duration: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  completedRequests.value = await completedRequestsApi.getAll()
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

function openEdit(req: CompletedRequest) {
  editing.value = req
  form.value = { requestId: req.requestId, requester: req.requester, type: req.type, priority: req.priority, requestedDate: req.requestedDate, completionDate: req.completionDate, duration: req.duration }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await completedRequestsApi.update(editing.value.id, form.value)
    else await completedRequestsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await completedRequestsApi.remove(id)
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
</script>

<style scoped>
.completed-requests {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
