<template>
  <div class="request-types">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else class="card">
      <div class="card-header bg-info text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Change Request Types</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">+ Add</button>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div v-for="type in requestTypes" :key="type.id" class="col-md-6">
            <div class="card border-start border-4" :style="{ borderColor: type.color }">
              <div class="card-body">
                <h6 class="card-title fw-bold">{{ type.name }}</h6>
                <p class="card-text text-muted small">{{ type.description }}</p>
                <div class="d-flex justify-content-between align-items-center">
                  <span class="badge" :style="{ backgroundColor: type.color }">
                    {{ type.count }} pending
                  </span>
                  <div>
                    <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(type)">Edit</button>
                    <button class="btn btn-sm btn-outline-danger" @click="remove(type.id)">Delete</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer d-flex justify-content-between">
        <span class="text-muted">Total request types: {{ requestTypes.length }}</span>
      </div>
    </div>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'New' }} Request Type</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <input v-model="form.description" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Color</label>
              <input v-model="form.color" type="color" class="form-control form-control-color" />
            </div>
            <div class="mb-3">
              <label class="form-label">Count</label>
              <input v-model.number="form.count" type="number" class="form-control" />
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
import { requestTypesApi, type RequestType } from '../../services/api'

const requestTypes = ref<RequestType[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<RequestType | null>(null)

const defaultForm = { name: '', description: '', color: '#0d6efd', count: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  requestTypes.value = await requestTypesApi.getAll()
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

function openEdit(type: RequestType) {
  editing.value = type
  form.value = { name: type.name, description: type.description, color: type.color, count: type.count }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await requestTypesApi.update(editing.value.id, form.value)
    else await requestTypesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await requestTypesApi.remove(id)
  await loadData()
}
</script>

<style scoped>
.request-types {
  padding: 1rem;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-2px);
}
</style>
