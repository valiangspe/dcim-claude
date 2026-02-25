<template>
  <div class="container-fluid">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Maintenance Scheduling</h1>
      <button class="btn btn-primary" @click="openCreate">+ Schedule Maintenance</button>
    </div>

    <!-- Maintenance Windows Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Scheduled Maintenance Windows</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>System/Component</th>
              <th>Start Time</th>
              <th>End Time</th>
              <th>Duration</th>
              <th>Impact</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="maintenance in maintenanceWindows" :key="maintenance.id">
              <td>{{ maintenance.component }}</td>
              <td>{{ formatDateTime(maintenance.startTime) }}</td>
              <td>{{ formatDateTime(maintenance.endTime) }}</td>
              <td>{{ calculateDuration(maintenance.startTime, maintenance.endTime) }}</td>
              <td>
                <span :class="getImpactBadgeClass(maintenance.impact)">
                  {{ maintenance.impact }}
                </span>
              </td>
              <td>
                <span :class="getStatusBadgeClass(maintenance.status)">
                  {{ maintenance.status }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(maintenance)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(maintenance.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Maintenance Policy Settings -->
    <div class="row">
      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Maintenance Windows Policy</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Suppress Alarms During Maintenance</label>
              <div class="form-check form-switch">
                <input
                  v-model="maintenancePolicy.suppressAlarms"
                  type="checkbox"
                  class="form-check-input"
                  id="suppressAlarms"
                />
                <label class="form-check-label" for="suppressAlarms">
                  {{ maintenancePolicy.suppressAlarms ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Minimum Notice Required (hours)</label>
              <input
                v-model.number="maintenancePolicy.minimumNotice"
                type="number"
                class="form-control"
                min="1"
                max="168"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Maximum Duration (hours)</label>
              <input
                v-model.number="maintenancePolicy.maxDuration"
                type="number"
                class="form-control"
                min="1"
                max="168"
              />
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Notification Settings</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Notify Before Maintenance Starts</label>
              <select v-model="maintenancePolicy.preNotification" class="form-select">
                <option value="1h">1 hour before</option>
                <option value="30min">30 minutes before</option>
                <option value="15min">15 minutes before</option>
                <option value="5min">5 minutes before</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Notify After Maintenance Completes</label>
              <div class="form-check form-switch">
                <input
                  v-model="maintenancePolicy.postNotification"
                  type="checkbox"
                  class="form-check-input"
                  id="postNotification"
                />
                <label class="form-check-label" for="postNotification">
                  {{ maintenancePolicy.postNotification ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Notification Recipients</label>
              <input
                v-model="maintenancePolicy.recipients"
                type="text"
                class="form-control"
                placeholder="ops-team@company.com, manager@company.com"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Save Button -->
    <div class="mt-4">
      <button class="btn btn-primary btn-lg">Save Policy Settings</button>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Maintenance Window</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Component</label>
              <input v-model="form.component" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Start Time</label>
              <input v-model="form.startTime" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">End Time</label>
              <input v-model="form.endTime" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Impact</label>
              <select v-model="form.impact" class="form-select">
                <option value="Critical">Critical</option>
                <option value="High">High</option>
                <option value="Medium">Medium</option>
                <option value="Low">Low</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="Scheduled">Scheduled</option>
                <option value="In Progress">In Progress</option>
                <option value="Completed">Completed</option>
                <option value="Cancelled">Cancelled</option>
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

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { maintenanceWindowsApi, type MaintenanceWindow } from '../../services/api'

const maintenanceWindows = ref<MaintenanceWindow[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<MaintenanceWindow | null>(null)
const defaultForm = { component: '', startTime: '', endTime: '', impact: 'Medium', status: 'Scheduled' }
const form = ref({ ...defaultForm })

async function loadData() {
  maintenanceWindows.value = await maintenanceWindowsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(maintenance: MaintenanceWindow) {
  editing.value = maintenance
  form.value = { component: maintenance.component, startTime: maintenance.startTime, endTime: maintenance.endTime, impact: maintenance.impact, status: maintenance.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await maintenanceWindowsApi.update(editing.value.id, form.value)
    else await maintenanceWindowsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await maintenanceWindowsApi.remove(id)
  await loadData()
}

const maintenancePolicy = reactive({
  suppressAlarms: true,
  minimumNotice: 24,
  maxDuration: 12,
  preNotification: '1h',
  postNotification: true,
  recipients: 'ops-team@company.com, manager@company.com',
})

function formatDateTime(date: string | Date): string {
  const d = typeof date === 'string' ? new Date(date) : date
  return d.toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function calculateDuration(start: string | Date, end: string | Date): string {
  const s = typeof start === 'string' ? new Date(start) : start
  const e = typeof end === 'string' ? new Date(end) : end
  const diffMs = e.getTime() - s.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)

  if (diffHours < 1) return `${diffMins}m`
  const remainingMins = diffMins % 60
  return `${diffHours}h ${remainingMins}m`
}

function getImpactBadgeClass(impact: string): string {
  const classes: Record<string, string> = {
    Critical: 'badge bg-danger',
    High: 'badge bg-warning text-dark',
    Medium: 'badge bg-info',
    Low: 'badge bg-secondary',
  }
  return classes[impact] || 'badge bg-secondary'
}

function getStatusBadgeClass(status: string): string {
  const classes: Record<string, string> = {
    Scheduled: 'badge bg-info',
    'In Progress': 'badge bg-warning text-dark',
    Completed: 'badge bg-success',
    Cancelled: 'badge bg-danger',
  }
  return classes[status] || 'badge bg-secondary'
}
</script>
