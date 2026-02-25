<template>
  <div class="container-fluid">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Push Notifications</h1>
      <button class="btn btn-primary" @click="openCreate">+ Add Channel</button>
    </div>

    <!-- Push Channels Configuration -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Configured Channels</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Channel Name</th>
              <th>Type</th>
              <th>Endpoint</th>
              <th>Alert Types</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="channel in pushChannels" :key="channel.id">
              <td>{{ channel.name }}</td>
              <td>
                <span class="badge bg-info">{{ channel.type }}</span>
              </td>
              <td class="text-truncate" style="max-width: 250px">{{ channel.endpoint }}</td>
              <td>{{ channel.alertTypes }}</td>
              <td>
                <span :class="channel.active ? 'badge bg-success' : 'badge bg-danger'">
                  {{ channel.active ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(channel)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(channel.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Push Notification Settings -->
    <div class="row">
      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Mobile App Settings</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Push Notifications Enabled</label>
              <div class="form-check form-switch">
                <input
                  v-model="pushSettings.mobileEnabled"
                  type="checkbox"
                  class="form-check-input"
                  id="mobileEnabled"
                />
                <label class="form-check-label" for="mobileEnabled">
                  {{ pushSettings.mobileEnabled ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Include Rich Media</label>
              <div class="form-check form-switch">
                <input
                  v-model="pushSettings.richMedia"
                  type="checkbox"
                  class="form-check-input"
                  id="richMedia"
                />
                <label class="form-check-label" for="richMedia">
                  {{ pushSettings.richMedia ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Delivery Timeout (seconds)</label>
              <input
                v-model.number="pushSettings.deliveryTimeout"
                type="number"
                class="form-control"
                min="10"
                max="300"
              />
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Webhook Settings</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Webhook Enabled</label>
              <div class="form-check form-switch">
                <input
                  v-model="pushSettings.webhookEnabled"
                  type="checkbox"
                  class="form-check-input"
                  id="webhookEnabled"
                />
                <label class="form-check-label" for="webhookEnabled">
                  {{ pushSettings.webhookEnabled ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Retry Policy</label>
              <select v-model="pushSettings.retryPolicy" class="form-select">
                <option value="none">No Retry</option>
                <option value="exponential">Exponential Backoff</option>
                <option value="fixed">Fixed Interval (5 min)</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Max Retries</label>
              <input
                v-model.number="pushSettings.maxRetries"
                type="number"
                class="form-control"
                min="0"
                max="10"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Save Button -->
    <div class="mt-4">
      <button class="btn btn-primary btn-lg">Save All Settings</button>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Push Channel</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Endpoint</label>
              <input v-model="form.endpoint" type="text" class="form-control" />
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
import { pushChannelsApi, type PushChannel } from '../../services/api'

const pushChannels = ref<PushChannel[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<PushChannel | null>(null)
const defaultForm = { name: '', type: '', endpoint: '', alertTypes: '', active: true }
const form = ref({ ...defaultForm })

async function loadData() {
  pushChannels.value = await pushChannelsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(channel: PushChannel) {
  editing.value = channel
  form.value = { name: channel.name, type: channel.type, endpoint: channel.endpoint, alertTypes: channel.alertTypes, active: channel.active }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await pushChannelsApi.update(editing.value.id, form.value)
    else await pushChannelsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await pushChannelsApi.remove(id)
  await loadData()
}

const pushSettings = reactive({
  mobileEnabled: true,
  richMedia: true,
  deliveryTimeout: 120,
  webhookEnabled: true,
  retryPolicy: 'exponential',
  maxRetries: 3,
})
</script>
