<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { containerRecordsApi, type ContainerRecord } from '../../services/api'

const containers = ref<ContainerRecord[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<ContainerRecord | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', ip: '', pods: 0, cpu: 0, mem: 0, network: '', status: 'running' }
const form = ref({ ...defaultForm })

async function loadData() {
  containers.value = await containerRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ContainerRecord) {
  editing.value = item
  form.value = { name: item.name, ip: item.ip, pods: item.pods, cpu: item.cpu, mem: item.mem, network: item.network, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await containerRecordsApi.update(editing.value.id, form.value)
    else await containerRecordsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: ContainerRecord) {
  if (!confirm(`Delete container "${item.name}"?`)) return
  deleting.value = item.id
  try { await containerRecordsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Containers</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Container Hosts</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Host</th><th>IP</th><th class="text-end">Pods</th><th class="text-end">CPU %</th><th class="text-end">Mem %</th><th class="text-end">Network</th><th>Status</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="c in containers" :key="c.id">
        <td class="fw-semibold font-monospace">{{ c.name }}</td><td class="font-monospace">{{ c.ip }}</td>
        <td class="text-end">{{ c.pods }}</td>
        <td class="text-end" :class="c.cpu>80?'text-danger fw-semibold':''">{{ c.cpu || '---' }}%</td>
        <td class="text-end" :class="c.mem>75?'text-warning':''">{{ c.mem || '---' }}%</td>
        <td class="text-end">{{ c.network }}</td>
        <td><span class="badge" :class="c.status==='running'?'bg-success':'bg-secondary'">{{ c.status }}</span></td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(c)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(c)" :disabled="deleting === c.id">
            <span v-if="deleting === c.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Container</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">IP</label><input v-model="form.ip" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Pods</label><input v-model.number="form.pods" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">CPU %</label><input v-model.number="form.cpu" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Mem %</label><input v-model.number="form.mem" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Network</label><input v-model="form.network" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Status</label><select v-model="form.status" class="form-select"><option value="running">running</option><option value="stopped">stopped</option></select></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
