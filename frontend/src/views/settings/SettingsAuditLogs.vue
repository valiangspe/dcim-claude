<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1 class="h3 mb-0">System Audit Logs</h1>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Audit Log</button>
    </div>

    <div class="row mb-3">
      <div class="col-md-3">
        <input type="text" class="form-control form-control-sm" placeholder="Search logs...">
      </div>
      <div class="col-md-3">
        <select class="form-select form-select-sm">
          <option value="">All Events</option>
          <option value="login">Login</option>
          <option value="config">Configuration</option>
          <option value="user">User Management</option>
          <option value="security">Security</option>
        </select>
      </div>
      <div class="col-md-3">
        <select class="form-select form-select-sm">
          <option value="">All Users</option>
          <option value="admin">John Administrator</option>
          <option value="ops">Sarah Operations</option>
          <option value="system">System</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Timestamp</th>
                <th>User</th>
                <th>Event Type</th>
                <th>Action</th>
                <th>Resource</th>
                <th>Result</th>
                <th>IP Address</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="log in logs" :key="log.id">
                <td class="small">{{ log.timestamp }}</td>
                <td class="fw-medium">{{ log.user }}</td>
                <td>
                  <span class="badge" :class="getBadgeClass(log.eventType)">
                    {{ log.eventType }}
                  </span>
                </td>
                <td class="small">{{ log.action }}</td>
                <td class="small text-muted">{{ log.resource }}</td>
                <td>
                  <span :class="['badge', log.result === 'Success' ? 'bg-success' : 'bg-danger']">
                    {{ log.result }}
                  </span>
                </td>
                <td class="font-monospace small">{{ log.ipAddress }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(log)">Edit</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Audit Log</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Timestamp</label><input v-model="form.timestamp" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">User</label><input v-model="form.user" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Event Type</label><input v-model="form.eventType" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Action</label><input v-model="form.action" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Resource</label><input v-model="form.resource" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Result</label><input v-model="form.result" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">IP Address</label><input v-model="form.ipAddress" type="text" class="form-control" /></div>
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

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { settingsAuditsApi, type SettingsAudit } from '../../services/api'

const logs = ref<SettingsAudit[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<SettingsAudit | null>(null)
const saving = ref(false)

const defaultForm = { timestamp: '', user: '', eventType: '', action: '', resource: '', result: '', ipAddress: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  logs.value = await settingsAuditsApi.getAll()
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

function openEdit(item: SettingsAudit) {
  editing.value = item
  form.value = { timestamp: item.timestamp, user: item.user, eventType: item.eventType, action: item.action, resource: item.resource, result: item.result, ipAddress: item.ipAddress }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await settingsAuditsApi.update(editing.value.id, form.value)
    } else {
      await settingsAuditsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this audit log?')) return
  await settingsAuditsApi.remove(id)
  await loadData()
}

function getBadgeClass(eventType: string): string {
  const classMap: Record<string, string> = {
    'Login': 'bg-primary',
    'Configuration': 'bg-info',
    'User Management': 'bg-warning',
    'Security': 'bg-danger'
  };
  return classMap[eventType] || 'bg-secondary';
}
</script>
