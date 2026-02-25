<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { vmHostsApi, type VmHost } from '../../services/api'

const summary = [
  { label: 'Physical Servers', count: 124, color: 'primary' },
  { label: 'Virtual Machines', count: 486, color: 'info' },
  { label: 'Containers', count: 1247, color: 'success' },
  { label: 'VM-to-Physical Ratio', count: '3.9:1', color: 'dark' },
]
const hosts = ref<VmHost[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<VmHost | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', type: '', physical: true, vms: 0, vcpus: 0, vmem: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  hosts.value = await vmHostsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: VmHost) {
  editing.value = item
  form.value = { name: item.name, type: item.type, physical: item.physical, vms: item.vms, vcpus: item.vcpus, vmem: item.vmem }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await vmHostsApi.update(editing.value.id, form.value)
    else await vmHostsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: VmHost) {
  if (!confirm(`Delete host "${item.name}"?`)) return
  deleting.value = item.id
  try { await vmHostsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Physical / Virtual</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="row g-3 mb-4">
      <div v-for="s in summary" :key="s.label" class="col-sm-6 col-lg-3">
        <div class="card border-0 shadow-sm text-center h-100"><div class="card-body"><h3 :class="`text-${s.color} mb-0`">{{ s.count }}</h3><small class="text-muted">{{ s.label }}</small></div></div>
      </div>
    </div>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Hypervisor Hosts</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Host</th><th>Hypervisor</th><th class="text-end">VMs</th><th class="text-end">vCPUs</th><th class="text-end">vMem (GB)</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="h in hosts" :key="h.id"><td class="fw-semibold font-monospace">{{ h.name }}</td><td>{{ h.type }}</td><td class="text-end">{{ h.vms }}</td><td class="text-end">{{ h.vcpus }}</td><td class="text-end">{{ h.vmem }}</td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(h)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(h)" :disabled="deleting === h.id">
            <span v-if="deleting === h.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} VM Host</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Type / Hypervisor</label><input v-model="form.type" type="text" class="form-control" /></div>
          <div class="mb-3 form-check"><input v-model="form.physical" type="checkbox" class="form-check-input" id="physicalCheck" /><label class="form-check-label" for="physicalCheck">Physical</label></div>
          <div class="mb-3"><label class="form-label">VMs</label><input v-model.number="form.vms" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">vCPUs</label><input v-model.number="form.vcpus" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">vMem (GB)</label><input v-model.number="form.vmem" type="number" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
