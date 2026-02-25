<template>
  <div class="container-fluid">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>SMS Notifications</h1>
      <button class="btn btn-primary" @click="openCreate">+ Add Phone Number</button>
    </div>

    <!-- SMS Recipients Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">SMS Recipients</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Contact Name</th>
              <th>Phone Number</th>
              <th>Alert Severity</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="recipient in smsRecipients" :key="recipient.id">
              <td>{{ recipient.name }}</td>
              <td>{{ recipient.phone }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ recipient.severity }}</span>
              </td>
              <td>
                <div class="form-check form-switch">
                  <input
                    type="checkbox"
                    class="form-check-input"
                    :checked="recipient.enabled"
                    :disabled="true"
                  />
                  <label class="form-check-label">
                    {{ recipient.enabled ? 'Enabled' : 'Disabled' }}
                  </label>
                </div>
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

    <!-- SMS Settings -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">SMS Settings</h5>
      </div>
      <div class="card-body">
        <div class="mb-3">
          <label class="form-label">SMS Alerts Enabled</label>
          <div class="form-check form-switch">
            <input
              v-model="smsSettings.enabled"
              type="checkbox"
              class="form-check-input"
              id="smsEnabled"
            />
            <label class="form-check-label" for="smsEnabled">
              {{ smsSettings.enabled ? 'Yes' : 'No' }}
            </label>
          </div>
        </div>
        <div class="mb-3">
          <label class="form-label">Minimum Alert Severity for SMS</label>
          <select v-model="smsSettings.minSeverity" class="form-select">
            <option value="critical">Critical</option>
            <option value="high">High</option>
            <option value="medium">Medium</option>
            <option value="low">Low</option>
          </select>
        </div>
        <div class="mb-3">
          <label class="form-label">Daily SMS Limit</label>
          <input
            v-model.number="smsSettings.dailyLimit"
            type="number"
            class="form-control"
            min="1"
            max="1000"
          />
        </div>
        <div class="mb-3">
          <label class="form-label">Quiet Hours (24-hour format)</label>
          <div class="row">
            <div class="col-md-4">
              <label class="form-label">Start Time</label>
              <input
                v-model="smsSettings.quietHourStart"
                type="time"
                class="form-control"
              />
            </div>
            <div class="col-md-4">
              <label class="form-label">End Time</label>
              <input v-model="smsSettings.quietHourEnd" type="time" class="form-control" />
            </div>
            <div class="col-md-4">
              <label class="form-label">Enabled</label>
              <div class="form-check form-switch pt-2">
                <input
                  v-model="smsSettings.quietHourEnabled"
                  type="checkbox"
                  class="form-check-input"
                  id="quietHoursEnabled"
                />
                <label class="form-check-label" for="quietHoursEnabled"> </label>
              </div>
            </div>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} SMS Recipient</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Phone</label>
              <input v-model="form.phone" type="text" class="form-control" />
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
            <div class="mb-3 form-check">
              <input v-model="form.enabled" type="checkbox" class="form-check-input" id="formEnabled" />
              <label class="form-check-label" for="formEnabled">Enabled</label>
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
import { smsRecipientsApi, type SmsRecipient } from '../../services/api'

const smsRecipients = ref<SmsRecipient[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<SmsRecipient | null>(null)
const defaultForm = { name: '', phone: '', severity: 'Critical', enabled: true }
const form = ref({ ...defaultForm })

async function loadData() {
  smsRecipients.value = await smsRecipientsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(recipient: SmsRecipient) {
  editing.value = recipient
  form.value = { name: recipient.name, phone: recipient.phone, severity: recipient.severity, enabled: recipient.enabled }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await smsRecipientsApi.update(editing.value.id, form.value)
    else await smsRecipientsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await smsRecipientsApi.remove(id)
  await loadData()
}

const smsSettings = reactive({
  enabled: true,
  minSeverity: 'critical',
  dailyLimit: 50,
  quietHourStart: '22:00',
  quietHourEnd: '06:00',
  quietHourEnabled: true,
})
</script>
