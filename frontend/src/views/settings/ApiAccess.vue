<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">API Access</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Generate Key</button>
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
                <th>Key Name</th>
                <th>Key ID</th>
                <th>Created</th>
                <th>Last Used</th>
                <th>Permissions</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="key in apiKeys" :key="key.id">
                <td class="fw-medium">{{ key.name }}</td>
                <td class="font-monospace small">{{ key.keyId }}</td>
                <td class="text-muted small">{{ key.created }}</td>
                <td class="text-muted small">{{ key.lastUsed }}</td>
                <td>
                  <span v-for="perm in key.permissions.split(',')" :key="perm" class="badge bg-secondary me-1">
                    {{ perm.trim() }}
                  </span>
                </td>
                <td>
                  <span :class="['badge', key.active ? 'bg-success' : 'bg-secondary']">
                    {{ key.active ? 'Active' : 'Disabled' }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(key)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(key.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} API Key</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Key ID</label><input v-model="form.keyId" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Created</label><input v-model="form.created" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Last Used</label><input v-model="form.lastUsed" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Permissions</label><input v-model="form.permissions" type="text" class="form-control" /></div>
            <div class="mb-3 form-check"><input v-model="form.active" type="checkbox" class="form-check-input" id="activeCheck" /><label class="form-check-label" for="activeCheck">Active</label></div>
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
import { apiKeysApi, type ApiKey } from '../../services/api'

const apiKeys = ref<ApiKey[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<ApiKey | null>(null)
const saving = ref(false)

const defaultForm = { name: '', keyId: '', created: '', lastUsed: '', permissions: '', active: true }
const form = ref({ ...defaultForm })

async function loadData() {
  apiKeys.value = await apiKeysApi.getAll()
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

function openEdit(item: ApiKey) {
  editing.value = item
  form.value = { name: item.name, keyId: item.keyId, created: item.created, lastUsed: item.lastUsed, permissions: item.permissions, active: item.active }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await apiKeysApi.update(editing.value.id, form.value)
    } else {
      await apiKeysApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this API key?')) return
  await apiKeysApi.remove(id)
  await loadData()
}
</script>
