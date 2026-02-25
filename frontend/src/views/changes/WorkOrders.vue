<template>
  <div class="work-orders">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else class="card">
      <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Work Orders</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">+ Add</button>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover">
            <thead>
              <tr class="table-light">
                <th>Work Order ID</th>
                <th>Change Request</th>
                <th>Assigned Technician</th>
                <th>Priority</th>
                <th>Status</th>
                <th>Due Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="wo in workOrders" :key="wo.id">
                <td><strong>{{ wo.workOrderId }}</strong></td>
                <td>{{ wo.changeRequest }}</td>
                <td>{{ wo.technician }}</td>
                <td>
                  <span :class="getPriorityBadgeClass(wo.priority)">
                    {{ wo.priority }}
                  </span>
                </td>
                <td>
                  <span :class="getStatusBadgeClass(wo.status)">
                    {{ wo.status }}
                  </span>
                </td>
                <td>{{ wo.dueDate }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(wo)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(wo.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Active work orders: {{ workOrders.length }}
      </div>
    </div>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'New' }} Work Order</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Work Order ID</label>
              <input v-model="form.workOrderId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Change Request</label>
              <input v-model="form.changeRequest" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Technician</label>
              <input v-model="form.technician" type="text" class="form-control" />
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
                <option>Not Started</option>
                <option>Scheduled</option>
                <option>In Progress</option>
                <option>Completed</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Due Date</label>
              <input v-model="form.dueDate" type="date" class="form-control" />
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
import { workOrdersApi, type WorkOrder } from '../../services/api'

const workOrders = ref<WorkOrder[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<WorkOrder | null>(null)

const defaultForm = { workOrderId: '', changeRequest: '', technician: '', priority: 'Medium', status: 'Not Started', dueDate: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  workOrders.value = await workOrdersApi.getAll()
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

function openEdit(wo: WorkOrder) {
  editing.value = wo
  form.value = { workOrderId: wo.workOrderId, changeRequest: wo.changeRequest, technician: wo.technician, priority: wo.priority, status: wo.status, dueDate: wo.dueDate }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await workOrdersApi.update(editing.value.id, form.value)
    else await workOrdersApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await workOrdersApi.remove(id)
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
    case 'In Progress':
      return `${classes} bg-info`
    case 'Scheduled':
      return `${classes} bg-warning text-dark`
    case 'Not Started':
      return `${classes} bg-secondary`
    case 'Completed':
      return `${classes} bg-success`
    default:
      return `${classes} bg-light text-dark`
  }
}
</script>

<style scoped>
.work-orders {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
