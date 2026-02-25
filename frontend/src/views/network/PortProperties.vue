<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { portRecordsApi, type PortRecord } from '../../services/api'

const ports = ref<PortRecord[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<PortRecord | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { portId: '', speed: '', vlan: 0, color: '', connected: '', status: 'up' }
const form = ref({ ...defaultForm })

function statusBadge(s: string) { return s === 'up' ? 'bg-success' : s === 'down' ? 'bg-danger' : 'bg-secondary' }
function colorBadge(c: string) { return c === 'Blue' ? 'bg-primary' : c === 'Green' ? 'bg-success' : c === 'Yellow' ? 'bg-warning text-dark' : 'bg-danger' }

async function loadData() {
  ports.value = await portRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: PortRecord) {
  editing.value = item
  form.value = { portId: item.portId, speed: item.speed, vlan: item.vlan, color: item.color, connected: item.connected, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await portRecordsApi.update(editing.value.id, form.value)
    else await portRecordsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: PortRecord) {
  if (!confirm(`Delete port "${item.portId}"?`)) return
  deleting.value = item.id
  try { await portRecordsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Port Properties</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Port Records</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><div class="table-responsive"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Port</th><th>Speed</th><th>VLAN</th><th>Color</th><th>Connected To</th><th>Status</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="p in ports" :key="p.id">
        <td class="fw-semibold font-monospace small">{{ p.portId }}</td>
        <td><span class="badge bg-secondary">{{ p.speed }}</span></td>
        <td>{{ p.vlan }}</td>
        <td><span class="badge" :class="colorBadge(p.color)">{{ p.color }}</span></td>
        <td class="font-monospace small">{{ p.connected }}</td>
        <td><span class="badge" :class="statusBadge(p.status)">{{ p.status }}</span></td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(p)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(p)" :disabled="deleting === p.id">
            <span v-if="deleting === p.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Port</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Port ID</label><input v-model="form.portId" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Speed</label><input v-model="form.speed" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">VLAN</label><input v-model.number="form.vlan" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Color</label><select v-model="form.color" class="form-select"><option value="Blue">Blue</option><option value="Green">Green</option><option value="Yellow">Yellow</option><option value="Red">Red</option></select></div>
          <div class="mb-3"><label class="form-label">Connected To</label><input v-model="form.connected" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Status</label><select v-model="form.status" class="form-select"><option value="up">up</option><option value="down">down</option><option value="disabled">disabled</option></select></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
