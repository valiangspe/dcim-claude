<template>
  <div class="container-fluid">
    <h1 class="mb-4">Alarm History</h1>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>

    <!-- Filters -->
    <div class="card mb-4">
      <div class="card-body">
        <div class="row">
          <div class="col-md-3">
            <label class="form-label">Severity</label>
            <select v-model="filterSeverity" class="form-select">
              <option value="">All</option>
              <option value="Critical">Critical</option>
              <option value="High">High</option>
              <option value="Medium">Medium</option>
              <option value="Low">Low</option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">Time Range</label>
            <select v-model="timeRange" class="form-select">
              <option value="24h">Last 24 Hours</option>
              <option value="7d">Last 7 Days</option>
              <option value="30d">Last 30 Days</option>
              <option value="all">All Time</option>
            </select>
          </div>
          <div class="col-md-6 d-flex align-items-end">
            <button class="btn btn-primary">Search</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Historical Alarms Table -->
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Resolved Alarms</h5>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Entry</button>
      </div>
      <div class="table-responsive">
        <table class="table table-sm mb-0">
          <thead class="table-light">
            <tr>
              <th>Triggered</th>
              <th>Resolved</th>
              <th>Duration</th>
              <th>Source</th>
              <th>Severity</th>
              <th>Message</th>
              <th style="width:120px">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="alarm in historicalAlarms" :key="alarm.id">
              <td class="text-nowrap">{{ formatDateTime(alarm.triggered) }}</td>
              <td class="text-nowrap">{{ formatDateTime(alarm.resolved) }}</td>
              <td class="text-nowrap">{{ calculateDuration(alarm.triggered, alarm.resolved) }}</td>
              <td>{{ alarm.source }}</td>
              <td>
                <span :class="getSeverityBadgeClass(alarm.severity)">
                  {{ alarm.severity }}
                </span>
              </td>
              <td>{{ alarm.message }}</td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(alarm)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(alarm.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Alarm History</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Triggered</label>
              <input v-model="form.triggered" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Resolved</label>
              <input v-model="form.resolved" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Source</label>
              <input v-model="form.source" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Severity</label>
              <select v-model="form.severity" class="form-select">
                <option value="Critical">Critical</option>
                <option value="High">High</option>
                <option value="Medium">Medium</option>
                <option value="Low">Low</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Message</label>
              <input v-model="form.message" type="text" class="form-control" />
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
import { alarmHistoriesApi, type AlarmHistory } from '../../services/api'

const filterSeverity = ref('')
const timeRange = ref('24h')

const historicalAlarms = ref<AlarmHistory[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<AlarmHistory | null>(null)
const defaultForm = { triggered: '', resolved: '', source: '', severity: 'Medium', message: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  historicalAlarms.value = await alarmHistoriesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(alarm: AlarmHistory) {
  editing.value = alarm
  form.value = { triggered: alarm.triggered, resolved: alarm.resolved, source: alarm.source, severity: alarm.severity, message: alarm.message }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await alarmHistoriesApi.update(editing.value.id, form.value)
    else await alarmHistoriesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await alarmHistoriesApi.remove(id)
  await loadData()
}

function formatDateTime(date: string | Date): string {
  const d = typeof date === 'string' ? new Date(date) : date
  return d.toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function calculateDuration(start: string | Date, end: string | Date): string {
  const s = typeof start === 'string' ? new Date(start) : start
  const e = typeof end === 'string' ? new Date(end) : end
  const diffMs = e.getTime() - s.getTime()
  const diffMins = Math.floor(diffMs / 60000)

  if (diffMins < 60) return `${diffMins}m`
  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours}h ${diffMins % 60}m`
  const diffDays = Math.floor(diffHours / 24)
  return `${diffDays}d ${diffHours % 24}h`
}

function getSeverityBadgeClass(severity: string): string {
  const classes: Record<string, string> = {
    Critical: 'badge bg-danger',
    High: 'badge bg-warning text-dark',
    Medium: 'badge bg-info',
    Low: 'badge bg-secondary',
  }
  return classes[severity] || 'badge bg-secondary'
}
</script>
