<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { itemAgesApi, type ItemAge } from '../../services/api'

const items = ref<ItemAge[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<ItemAge | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', customer: '', purchased: '', age: '', warranty: '', status: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  items.value = await itemAgesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ItemAge) {
  editing.value = item
  form.value = { name: item.name, customer: item.customer, purchased: item.purchased, age: item.age, warranty: item.warranty, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await itemAgesApi.update(editing.value.id, form.value)
    else await itemAgesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: ItemAge) {
  if (!confirm(`Delete item "${item.name}"?`)) return
  deleting.value = item.id
  try { await itemAgesApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Item Age Tracking</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Items</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Item</th><th>Customer / Team</th><th>Purchased</th><th class="text-end">Age</th><th>Warranty</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="i in items" :key="i.id" :class="i.status==='warning'?'table-warning':''">
        <td class="fw-semibold">{{ i.name }}</td><td>{{ i.customer }}</td><td>{{ i.purchased }}</td><td class="text-end">{{ i.age }}</td>
        <td><span class="badge" :class="i.warranty==='Active'?'bg-success':'bg-danger'">{{ i.warranty }}</span></td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(i)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(i)" :disabled="deleting === i.id">
            <span v-if="deleting === i.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Item</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Customer / Team</label><input v-model="form.customer" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Purchased</label><input v-model="form.purchased" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Age</label><input v-model="form.age" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Warranty</label><select v-model="form.warranty" class="form-select"><option value="Active">Active</option><option value="Expired">Expired</option></select></div>
          <div class="mb-3"><label class="form-label">Status</label><select v-model="form.status" class="form-select"><option value="">normal</option><option value="warning">warning</option></select></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
