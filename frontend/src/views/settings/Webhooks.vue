<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Webhooks</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Webhook</button>
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
                <th>Name</th>
                <th>Endpoint URL</th>
                <th>Events Triggered</th>
                <th>Status</th>
                <th>Last Event</th>
                <th>Delivery Rate</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="webhook in webhooks" :key="webhook.id">
                <td class="fw-medium">{{ webhook.name }}</td>
                <td class="font-monospace small text-truncate" :title="webhook.url">{{ webhook.url }}</td>
                <td>
                  <span v-for="event in webhook.events.split(',')" :key="event" class="badge bg-secondary me-1">
                    {{ event.trim() }}
                  </span>
                </td>
                <td>
                  <span :class="['badge', webhook.status === 'Active' ? 'bg-success' : webhook.status === 'Failing' ? 'bg-danger' : 'bg-warning']">
                    {{ webhook.status }}
                  </span>
                </td>
                <td class="text-muted small">{{ webhook.lastEvent }}</td>
                <td>
                  <div class="progress" style="height: 20px; width: 80px;">
                    <div class="progress-bar" :class="webhook.deliveryRate >= 95 ? 'bg-success' : 'bg-warning'"
                         :style="{ width: webhook.deliveryRate + '%' }"></div>
                  </div>
                  <span class="small">{{ webhook.deliveryRate }}%</span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(webhook)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(webhook.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Webhook</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">URL</label><input v-model="form.url" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Events</label><input v-model="form.events" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Status</label><input v-model="form.status" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Last Event</label><input v-model="form.lastEvent" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Delivery Rate</label><input v-model.number="form.deliveryRate" type="number" class="form-control" /></div>
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
import { webhookConfigsApi, type WebhookConfig } from '../../services/api'

const webhooks = ref<WebhookConfig[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<WebhookConfig | null>(null)
const saving = ref(false)

const defaultForm = { name: '', url: '', events: '', status: '', lastEvent: '', deliveryRate: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  webhooks.value = await webhookConfigsApi.getAll()
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

function openEdit(item: WebhookConfig) {
  editing.value = item
  form.value = { name: item.name, url: item.url, events: item.events, status: item.status, lastEvent: item.lastEvent, deliveryRate: item.deliveryRate }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await webhookConfigsApi.update(editing.value.id, form.value)
    } else {
      await webhookConfigsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this webhook?')) return
  await webhookConfigsApi.remove(id)
  await loadData()
}
</script>
