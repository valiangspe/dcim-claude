<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { partsApi, type Part } from '../../services/api'

const parts = ref<Part[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<Part | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', category: '', inStock: 0, reserved: 0, reorder: 0, unit: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  parts.value = await partsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: Part) {
  editing.value = item
  form.value = { name: item.name, category: item.category, inStock: item.inStock, reserved: item.reserved, reorder: item.reorder, unit: item.unit }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await partsApi.update(editing.value.id, form.value)
    else await partsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: Part) {
  if (!confirm(`Delete part "${item.name}"?`)) return
  deleting.value = item.id
  try { await partsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Parts Inventory</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Parts</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Part</th><th>Category</th><th class="text-end">In Stock</th><th class="text-end">Reserved</th><th class="text-end">Available</th><th class="text-end">Reorder Level</th><th class="text-end">Unit Cost</th><th>Status</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="p in parts" :key="p.id">
        <td class="fw-semibold">{{ p.name }}</td><td><span class="badge bg-secondary">{{ p.category }}</span></td>
        <td class="text-end">{{ p.inStock }}</td><td class="text-end">{{ p.reserved }}</td><td class="text-end">{{ p.inStock-p.reserved }}</td><td class="text-end">{{ p.reorder }}</td><td class="text-end">{{ p.unit }}</td>
        <td><span class="badge" :class="(p.inStock-p.reserved)<=p.reorder?'bg-danger':'bg-success'">{{ (p.inStock-p.reserved)<=p.reorder?'LOW':'OK' }}</span></td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(p)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(p)" :disabled="deleting === p.id">
            <span v-if="deleting === p.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Part</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Category</label><input v-model="form.category" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">In Stock</label><input v-model.number="form.inStock" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Reserved</label><input v-model.number="form.reserved" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Reorder Level</label><input v-model.number="form.reorder" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Unit Cost</label><input v-model="form.unit" type="text" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
