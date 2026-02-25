<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1 class="h3 mb-0">Multi-Language Support</h1>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Language</button>
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
              <th>Language</th>
              <th>Code</th>
              <th>Native Name</th>
              <th>Strings Translated</th>
              <th>Completion %</th>
              <th>Status</th>
              <th>Last Updated</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="language in languages" :key="language.id">
              <td class="fw-medium">{{ language.name }}</td>
              <td class="font-monospace">{{ language.code }}</td>
              <td>{{ language.native }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ language.translated }}/{{ language.total }}</span>
              </td>
              <td>
                <div class="progress" style="height: 20px; width: 120px;">
                  <div class="progress-bar" :class="getProgressClass(language.completion)"
                       :style="{ width: language.completion + '%' }">
                    {{ language.completion }}%
                  </div>
                </div>
              </td>
              <td>
                <span :class="['badge', getStatusBadge(language.completion)]">
                  {{ getStatusText(language.completion) }}
                </span>
              </td>
              <td class="text-muted small">{{ language.lastUpdated }}</td>
              <td>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(language)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(language.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Language</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Code</label><input v-model="form.code" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Native Name</label><input v-model="form.native" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Translated</label><input v-model.number="form.translated" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Total</label><input v-model.number="form.total" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Completion %</label><input v-model.number="form.completion" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Last Updated</label><input v-model="form.lastUpdated" type="text" class="form-control" /></div>
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
import { languageConfigsApi, type LanguageConfig } from '../../services/api'

const languages = ref<LanguageConfig[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<LanguageConfig | null>(null)
const saving = ref(false)

const defaultForm = { name: '', code: '', native: '', translated: 0, total: 0, completion: 0, lastUpdated: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  languages.value = await languageConfigsApi.getAll()
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

function openEdit(item: LanguageConfig) {
  editing.value = item
  form.value = { name: item.name, code: item.code, native: item.native, translated: item.translated, total: item.total, completion: item.completion, lastUpdated: item.lastUpdated }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await languageConfigsApi.update(editing.value.id, form.value)
    } else {
      await languageConfigsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this language?')) return
  await languageConfigsApi.remove(id)
  await loadData()
}

function getProgressClass(completion: number): string {
  if (completion === 100) return 'bg-success';
  if (completion >= 90) return 'bg-info';
  if (completion >= 70) return 'bg-warning';
  return 'bg-danger';
}

function getStatusBadge(completion: number): string {
  if (completion === 100) return 'bg-success';
  if (completion >= 90) return 'bg-info';
  if (completion >= 70) return 'bg-warning';
  return 'bg-danger';
}

function getStatusText(completion: number): string {
  if (completion === 100) return 'Complete';
  if (completion >= 90) return 'Nearly Complete';
  if (completion >= 70) return 'In Progress';
  return 'Not Started';
}
</script>
