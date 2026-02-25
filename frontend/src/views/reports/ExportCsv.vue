<template>
  <div class="export-csv">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-success" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-header bg-success text-white d-flex align-items-center justify-content-between">
        <h5 class="mb-0">Export as CSV</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">+ Add Category</button>
      </div>
      <div class="card-body">
        <form @submit.prevent="handleExport">
          <div class="mb-4">
            <label class="form-label fw-bold">Select Data Category</label>
            <div class="list-group">
              <label v-for="category in categories" :key="category.id" class="list-group-item d-flex align-items-start">
                <input
                  v-model="selectedCategory"
                  type="radio"
                  class="form-check-input me-2 mt-1"
                  :value="category.id"
                />
                <div class="flex-grow-1">
                  <strong>{{ category.name }}</strong>
                  <small class="d-block text-muted">{{ category.description }}</small>
                </div>
                <div class="ms-2">
                  <button class="btn btn-sm btn-outline-primary me-1" @click.prevent="openEdit(category)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click.prevent="removeCategory(category.id)">Delete</button>
                </div>
              </label>
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-bold">Date Range</label>
            <div class="row g-2">
              <div class="col-md-6">
                <input v-model="startDate" type="date" class="form-control" />
              </div>
              <div class="col-md-6">
                <input v-model="endDate" type="date" class="form-control" />
              </div>
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-bold">Include Filters</label>
            <div class="form-check">
              <input v-model="includeMetadata" type="checkbox" class="form-check-input" id="metadata" />
              <label class="form-check-label" for="metadata">Include metadata</label>
            </div>
            <div class="form-check">
              <input v-model="includeTimestamps" type="checkbox" class="form-check-input" id="timestamps" />
              <label class="form-check-label" for="timestamps">Include timestamps</label>
            </div>
          </div>

          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-success">
              <i class="bi bi-download me-2"></i>Download CSV
            </button>
            <button type="reset" class="btn btn-outline-secondary">Clear</button>
          </div>
        </form>
      </div>
      <div class="card-footer bg-light">
        <small class="text-muted">Export file size estimate: {{ selectedCategory ? '2-5 MB' : 'N/A' }}</small>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Category</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="catForm.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <textarea v-model="catForm.description" class="form-control" rows="3"></textarea>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="saveCategory" :disabled="saving">
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
import { exportCategoriesApi, type ExportCategory } from '../../services/api'

const selectedCategory = ref<string>('power')
const startDate = ref<string>('2026-01-01')
const endDate = ref<string>('2026-02-20')
const includeMetadata = ref<boolean>(true)
const includeTimestamps = ref<boolean>(true)
const loading = ref(true)

const categories = ref<ExportCategory[]>([])
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ExportCategory | null>(null)
const catForm = ref({ name: '', description: '' })

function openCreate() {
  editing.value = null
  catForm.value = { name: '', description: '' }
  showModal.value = true
}

function openEdit(item: ExportCategory) {
  editing.value = item
  catForm.value = { name: item.name, description: item.description }
  showModal.value = true
}

async function saveCategory() {
  saving.value = true
  try {
    if (editing.value) await exportCategoriesApi.update(editing.value.id, catForm.value)
    else await exportCategoriesApi.create(catForm.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function removeCategory(id: number) {
  if (!confirm('Are you sure?')) return
  await exportCategoriesApi.remove(id)
  await loadData()
}

async function loadData() {
  categories.value = await exportCategoriesApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const handleExport = () => {
  const category = categories.value.find((c) => c.id === Number(selectedCategory.value))
  alert(
    `Exporting ${category?.name}\nFrom: ${startDate.value}\nTo: ${endDate.value}\nMetadata: ${includeMetadata.value}\nTimestamps: ${includeTimestamps.value}`
  )
}
</script>

<style scoped>
.export-csv {
  padding: 1rem;
  max-width: 600px;
}

.list-group-item {
  cursor: pointer;
  transition: background-color 0.2s;
}

.list-group-item:hover {
  background-color: #f8f9fa;
}

.list-group-item input[type='radio']:checked ~ strong {
  color: #198754;
}
</style>
