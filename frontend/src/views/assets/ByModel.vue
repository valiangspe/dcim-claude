<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { hardwareModelsApi, type HardwareModel } from '../../services/api'

const models = ref<HardwareModel[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<HardwareModel | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { model: '', vendor: '', category: '', count: 0, avgAge: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  models.value = await hardwareModelsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: HardwareModel) {
  editing.value = item
  form.value = { model: item.model, vendor: item.vendor, category: item.category, count: item.count, avgAge: item.avgAge }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await hardwareModelsApi.update(editing.value.id, form.value)
    else await hardwareModelsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: HardwareModel) {
  if (!confirm(`Delete model "${item.model}"?`)) return
  deleting.value = item.id
  try { await hardwareModelsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Assets by Model</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Hardware Models</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Model</th><th>Vendor</th><th>Category</th><th class="text-end">Count</th><th class="text-end">Avg Age</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="m in models" :key="m.id">
        <td class="fw-semibold">{{ m.model }}</td><td>{{ m.vendor }}</td><td><span class="badge bg-secondary">{{ m.category }}</span></td><td class="text-end">{{ m.count }}</td><td class="text-end">{{ m.avgAge }}</td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(m)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(m)" :disabled="deleting === m.id">
            <span v-if="deleting === m.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Hardware Model</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Model</label><input v-model="form.model" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Vendor</label><input v-model="form.vendor" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Category</label><input v-model="form.category" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Count</label><input v-model.number="form.count" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Avg Age</label><input v-model="form.avgAge" type="text" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
