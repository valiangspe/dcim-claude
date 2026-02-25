<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { powerPortRecordsApi, type PowerPortRecord } from '../../services/api'

const pdus = ref<PowerPortRecord[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<PowerPortRecord | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', total: 0, used: 0, reserved: 0, type: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  pdus.value = await powerPortRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: PowerPortRecord) {
  editing.value = item
  form.value = { name: item.name, total: item.total, used: item.used, reserved: item.reserved, type: item.type }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await powerPortRecordsApi.update(editing.value.id, form.value)
    else await powerPortRecordsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: PowerPortRecord) {
  if (!confirm(`Delete "${item.name}"?`)) return
  deleting.value = item.id
  try { await powerPortRecordsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Power Ports</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Power Port Records</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>PDU</th><th>Connector</th><th class="text-end">Total</th><th class="text-end">Used</th><th class="text-end">Reserved</th><th class="text-end">Free</th><th style="width:20%">Utilization</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="p in pdus" :key="p.id">
        <td class="fw-semibold">{{ p.name }}</td><td><span class="badge bg-secondary">{{ p.type }}</span></td>
        <td class="text-end">{{ p.total }}</td><td class="text-end">{{ p.used }}</td><td class="text-end">{{ p.reserved }}</td><td class="text-end">{{ p.total-p.used-p.reserved }}</td>
        <td><div class="progress" style="height:8px"><div class="progress-bar bg-primary" :style="{width:(p.used/p.total*100)+'%'}"></div><div class="progress-bar bg-warning" :style="{width:(p.reserved/p.total*100)+'%'}"></div></div></td>
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
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Power Port</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Total</label><input v-model.number="form.total" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Used</label><input v-model.number="form.used" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Reserved</label><input v-model.number="form.reserved" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Type</label><input v-model="form.type" type="text" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
