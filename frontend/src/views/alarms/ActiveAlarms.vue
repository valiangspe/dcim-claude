<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { alarmsApi, type Alarm } from '../../services/api'

const activeAlarms = ref<Alarm[]>([])
const loading = ref(true)
const acknowledging = ref<number | null>(null)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Alarm | null>(null)

const defaultForm = { source: '', severity: 'Medium', message: '', time: '', acknowledged: false }
const form = ref({ ...defaultForm })

async function loadData() {
  activeAlarms.value = await alarmsApi.getAll()
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

function openEdit(alarm: Alarm) {
  editing.value = alarm
  form.value = { source: alarm.source, severity: alarm.severity, message: alarm.message, time: alarm.time, acknowledged: alarm.acknowledged }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await alarmsApi.update(editing.value.id, form.value)
    else await alarmsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await alarmsApi.remove(id)
  await loadData()
}

async function acknowledge(alarm: Alarm) {
  acknowledging.value = alarm.id
  try {
    await alarmsApi.update(alarm.id, { acknowledged: true })
    await loadData()
  } finally {
    acknowledging.value = null
  }
}

function formatTime(isoString: string): string {
  const date = new Date(isoString)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)

  if (diffMins < 60) return `${diffMins}m ago`
  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours}h ago`
  return date.toLocaleDateString()
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

<template>
  <div class="container-fluid">
    <h1 class="mb-4">Active Alarms</h1>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
    <!-- Severity Summary -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card border-danger">
          <div class="card-body">
            <h6 class="card-title text-danger">Critical</h6>
            <p class="h4 mb-0">{{ activeAlarms.filter(a => a.severity === 'Critical').length }}</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-warning">
          <div class="card-body">
            <h6 class="card-title text-warning">High</h6>
            <p class="h4 mb-0">{{ activeAlarms.filter(a => a.severity === 'High').length }}</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-info">
          <div class="card-body">
            <h6 class="card-title text-info">Medium</h6>
            <p class="h4 mb-0">{{ activeAlarms.filter(a => a.severity === 'Medium').length }}</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-secondary">
          <div class="card-body">
            <h6 class="card-title text-secondary">Low</h6>
            <p class="h4 mb-0">{{ activeAlarms.filter(a => a.severity === 'Low').length }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Active Alarms Table -->
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Active Alarms</h5>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Alarm</button>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Time</th>
              <th>Source</th>
              <th>Severity</th>
              <th>Message</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="alarm in activeAlarms" :key="alarm.id">
              <td class="text-nowrap">{{ formatTime(alarm.time) }}</td>
              <td>{{ alarm.source }}</td>
              <td>
                <span :class="getSeverityBadgeClass(alarm.severity)">
                  {{ alarm.severity }}
                </span>
              </td>
              <td>{{ alarm.message }}</td>
              <td>
                <button
                  v-if="!alarm.acknowledged"
                  class="btn btn-sm btn-outline-primary me-1"
                  :disabled="acknowledging === alarm.id"
                  @click="acknowledge(alarm)"
                >
                  <span v-if="acknowledging === alarm.id" class="spinner-border spinner-border-sm me-1"></span>
                  Ack
                </button>
                <span v-else class="badge bg-success me-1">Ack'd</span>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(alarm)">Edit</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Alarm</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Source</label>
              <input v-model="form.source" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Severity</label>
              <select v-model="form.severity" class="form-select">
                <option>Critical</option>
                <option>High</option>
                <option>Medium</option>
                <option>Low</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Message</label>
              <input v-model="form.message" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Time</label>
              <input v-model="form.time" type="datetime-local" class="form-control" />
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
