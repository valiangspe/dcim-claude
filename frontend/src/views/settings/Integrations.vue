<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">BMS/SCADA Integrations</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Integration</button>
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
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>System Name</th>
              <th>Type</th>
              <th>Endpoint</th>
              <th>Status</th>
              <th>Last Sync</th>
              <th>Metrics</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="integration in integrations" :key="integration.id">
              <td class="fw-medium">{{ integration.name }}</td>
              <td>
                <span class="badge bg-info">{{ integration.type }}</span>
              </td>
              <td class="text-muted small">{{ integration.endpoint }}</td>
              <td>
                <span :class="['badge', integration.status === 'Connected' ? 'bg-success' : integration.status === 'Syncing' ? 'bg-primary' : 'bg-danger']">
                  {{ integration.status }}
                </span>
              </td>
              <td class="text-muted">{{ integration.lastSync }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ integration.metrics }}</span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(integration)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(integration.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Integration</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Type</label><input v-model="form.type" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Endpoint</label><input v-model="form.endpoint" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Status</label><input v-model="form.status" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Last Sync</label><input v-model="form.lastSync" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Metrics</label><input v-model.number="form.metrics" type="number" class="form-control" /></div>
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
import { integrationsApi, type Integration } from '../../services/api'

const integrations = ref<Integration[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<Integration | null>(null)
const saving = ref(false)

const defaultForm = { name: '', type: '', endpoint: '', status: '', lastSync: '', metrics: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  integrations.value = await integrationsApi.getAll()
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

function openEdit(item: Integration) {
  editing.value = item
  form.value = { name: item.name, type: item.type, endpoint: item.endpoint, status: item.status, lastSync: item.lastSync, metrics: item.metrics }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await integrationsApi.update(editing.value.id, form.value)
    } else {
      await integrationsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this integration?')) return
  await integrationsApi.remove(id)
  await loadData()
}
</script>
