<template>
  <div class="container-fluid">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Email Alerts Configuration</h1>
      <button class="btn btn-primary" @click="openCreate">+ Add Recipient</button>
    </div>

    <!-- Email Recipients Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Email Recipients</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Email Address</th>
              <th>Frequency</th>
              <th>Alert Types</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="recipient in emailRecipients" :key="recipient.id">
              <td>{{ recipient.email }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ recipient.frequency }}</span>
              </td>
              <td>{{ recipient.alertTypes }}</td>
              <td>
                <span :class="recipient.active ? 'badge bg-success' : 'badge bg-danger'">
                  {{ recipient.active ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(recipient)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(recipient.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Email Frequency Settings -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Email Frequency Settings</h5>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">Critical Alerts</label>
            <select v-model="frequencySettings.critical" class="form-select">
              <option value="immediate">Immediate</option>
              <option value="5min">Every 5 Minutes</option>
              <option value="hourly">Hourly Digest</option>
              <option value="daily">Daily Digest</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">High Priority Alerts</label>
            <select v-model="frequencySettings.high" class="form-select">
              <option value="immediate">Immediate</option>
              <option value="5min">Every 5 Minutes</option>
              <option value="hourly">Hourly Digest</option>
              <option value="daily">Daily Digest</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">Medium Priority Alerts</label>
            <select v-model="frequencySettings.medium" class="form-select">
              <option value="5min">Every 5 Minutes</option>
              <option value="hourly">Hourly Digest</option>
              <option value="daily">Daily Digest</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">Low Priority Alerts</label>
            <select v-model="frequencySettings.low" class="form-select">
              <option value="hourly">Hourly Digest</option>
              <option value="daily">Daily Digest</option>
              <option value="disabled">Disabled</option>
            </select>
          </div>
        </div>
        <button class="btn btn-primary">Save Settings</button>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Email Recipient</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Email</label>
              <input v-model="form.email" type="email" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Frequency</label>
              <select v-model="form.frequency" class="form-select">
                <option value="Immediate">Immediate</option>
                <option value="Hourly">Hourly</option>
                <option value="Daily">Daily</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Alert Types</label>
              <input v-model="form.alertTypes" type="text" class="form-control" />
            </div>
            <div class="mb-3 form-check">
              <input v-model="form.active" type="checkbox" class="form-check-input" id="formActive" />
              <label class="form-check-label" for="formActive">Active</label>
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
import { emailRecipientsApi, type EmailRecipient } from '../../services/api'

const emailRecipients = ref<EmailRecipient[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<EmailRecipient | null>(null)
const defaultForm = { email: '', frequency: 'Immediate', alertTypes: '', active: true }
const form = ref({ ...defaultForm })

async function loadData() {
  emailRecipients.value = await emailRecipientsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(recipient: EmailRecipient) {
  editing.value = recipient
  form.value = { email: recipient.email, frequency: recipient.frequency, alertTypes: recipient.alertTypes, active: recipient.active }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await emailRecipientsApi.update(editing.value.id, form.value)
    else await emailRecipientsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await emailRecipientsApi.remove(id)
  await loadData()
}

const frequencySettings = reactive({
  critical: 'immediate',
  high: '5min',
  medium: 'hourly',
  low: 'daily',
})
</script>
