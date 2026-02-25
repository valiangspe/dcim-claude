<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { serverRecordsApi, type ServerRecord } from '../../services/api'

const servers = ref<ServerRecord[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<ServerRecord | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', rack: '', os: '', uptime: '', status: 'online' }
const form = ref({ ...defaultForm })

const counts = computed(() => ({
  online: servers.value.filter(s => s.status === 'online').length,
  offline: servers.value.filter(s => s.status === 'offline').length,
  maintenance: servers.value.filter(s => s.status === 'maintenance').length,
}))

async function loadData() {
  servers.value = await serverRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ServerRecord) {
  editing.value = item
  form.value = { name: item.name, rack: item.rack, os: item.os, uptime: item.uptime, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await serverRecordsApi.update(editing.value.id, form.value)
    else await serverRecordsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: ServerRecord) {
  if (!confirm(`Delete server "${item.name}"?`)) return
  deleting.value = item.id
  try { await serverRecordsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Server Status</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="row g-3 mb-4">
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-success bg-opacity-10"><div class="card-body"><h3 class="text-success mb-0">{{ counts.online }}</h3><small class="text-muted">Online</small></div></div></div>
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-danger bg-opacity-10"><div class="card-body"><h3 class="text-danger mb-0">{{ counts.offline }}</h3><small class="text-muted">Offline</small></div></div></div>
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10"><div class="card-body"><h3 class="text-warning mb-0">{{ counts.maintenance }}</h3><small class="text-muted">Maintenance</small></div></div></div>
    </div>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Servers</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Server</th><th>Rack</th><th>OS</th><th>Uptime</th><th>Status</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="s in servers" :key="s.id">
        <td class="fw-semibold font-monospace">{{ s.name }}</td><td>{{ s.rack }}</td><td>{{ s.os }}</td><td>{{ s.uptime }}</td>
        <td><span class="badge" :class="{'bg-success':s.status==='online','bg-danger':s.status==='offline','bg-warning text-dark':s.status==='maintenance'}">{{ s.status }}</span></td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(s)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(s)" :disabled="deleting === s.id">
            <span v-if="deleting === s.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Server</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Rack</label><input v-model="form.rack" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">OS</label><input v-model="form.os" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Uptime</label><input v-model="form.uptime" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Status</label><select v-model="form.status" class="form-select"><option value="online">online</option><option value="offline">offline</option><option value="maintenance">maintenance</option></select></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
