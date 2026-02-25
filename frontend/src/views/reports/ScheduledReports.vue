<template>
  <div class="scheduled-reports">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-header bg-primary text-white">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Scheduled Reports</h5>
          <button class="btn btn-sm btn-light" @click="openCreate">+ Add Schedule</button>
        </div>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover">
            <thead>
              <tr class="table-light">
                <th>Report Name</th>
                <th>Frequency</th>
                <th>Recipients</th>
                <th>Last Sent</th>
                <th>Next Run</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="report in scheduledReports" :key="report.id">
                <td><strong>{{ report.name }}</strong></td>
                <td>
                  <span class="badge bg-info">{{ report.frequency }}</span>
                </td>
                <td>
                  <small>{{ report.recipients }}</small>
                </td>
                <td>{{ report.lastSent }}</td>
                <td>{{ report.nextRun }}</td>
                <td>
                  <span :class="getStatusBadgeClass(report.status)">
                    {{ report.status }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(report)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(report.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Active schedules: {{ scheduledReports.length }}
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Scheduled Report</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Frequency</label>
              <select v-model="form.frequency" class="form-select">
                <option value="Daily">Daily</option>
                <option value="Weekly">Weekly</option>
                <option value="Monthly">Monthly</option>
                <option value="Quarterly">Quarterly</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Recipients</label>
              <input v-model="form.recipients" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Last Sent</label>
              <input v-model="form.lastSent" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Next Run</label>
              <input v-model="form.nextRun" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="Active">Active</option>
                <option value="Paused">Paused</option>
                <option value="Disabled">Disabled</option>
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
import { ref, onMounted } from 'vue'
import { scheduledReportsApi, type ScheduledReport } from '../../services/api'

const scheduledReports = ref<ScheduledReport[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ScheduledReport | null>(null)
const defaultForm = { name: '', frequency: 'Daily', recipients: '', lastSent: '', nextRun: '', status: 'Active' }
const form = ref({ ...defaultForm })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ScheduledReport) {
  editing.value = item
  form.value = { name: item.name, frequency: item.frequency, recipients: item.recipients, lastSent: item.lastSent, nextRun: item.nextRun, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await scheduledReportsApi.update(editing.value.id, form.value)
    else await scheduledReportsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await scheduledReportsApi.remove(id)
  await loadData()
}

async function loadData() {
  scheduledReports.value = await scheduledReportsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const getStatusBadgeClass = (status: string): string => {
  const classes = 'badge'
  switch (status) {
    case 'Active':
      return `${classes} bg-success`
    case 'Paused':
      return `${classes} bg-warning text-dark`
    case 'Disabled':
      return `${classes} bg-danger`
    default:
      return `${classes} bg-secondary`
  }
}
</script>

<style scoped>
.scheduled-reports {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
