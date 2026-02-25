<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { serverMetricsApi, type ServerMetric } from '../../services/api'

const servers = ref<ServerMetric[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<ServerMetric | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', cpu: 0, gpu: 0, mem: 0, disk: 0, netIn: 0, netOut: 0, power: 0 }
const form = ref({ ...defaultForm })

function metricClass(v: number) { return v >= 85 ? 'text-danger fw-semibold' : v >= 70 ? 'text-warning' : '' }

async function loadData() {
  servers.value = await serverMetricsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ServerMetric) {
  editing.value = item
  form.value = { name: item.name, cpu: item.cpu, gpu: item.gpu, mem: item.mem, disk: item.disk, netIn: item.netIn, netOut: item.netOut, power: item.power }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await serverMetricsApi.update(editing.value.id, form.value)
    else await serverMetricsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: ServerMetric) {
  if (!confirm(`Delete server "${item.name}"?`)) return
  deleting.value = item.id
  try { await serverMetricsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Server Metrics</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Server Metrics</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><div class="table-responsive"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Server</th><th class="text-end">CPU %</th><th class="text-end">GPU %</th><th class="text-end">Mem %</th><th class="text-end">Disk %</th><th class="text-end">Net In (Gbps)</th><th class="text-end">Net Out (Gbps)</th><th class="text-end">Power (kW)</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="s in servers" :key="s.id">
        <td class="fw-semibold font-monospace">{{ s.name }}</td>
        <td class="text-end" :class="metricClass(s.cpu)">{{ s.cpu }}</td>
        <td class="text-end" :class="s.gpu?metricClass(s.gpu):''">{{ s.gpu||'---' }}</td>
        <td class="text-end" :class="metricClass(s.mem)">{{ s.mem }}</td>
        <td class="text-end" :class="metricClass(s.disk)">{{ s.disk }}</td>
        <td class="text-end">{{ s.netIn }}</td><td class="text-end">{{ s.netOut }}</td>
        <td class="text-end">{{ s.power.toFixed(2) }}</td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(s)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(s)" :disabled="deleting === s.id">
            <span v-if="deleting === s.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Server Metric</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">CPU %</label><input v-model.number="form.cpu" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">GPU %</label><input v-model.number="form.gpu" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Mem %</label><input v-model.number="form.mem" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Disk %</label><input v-model.number="form.disk" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Net In (Gbps)</label><input v-model.number="form.netIn" type="number" step="0.1" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Net Out (Gbps)</label><input v-model.number="form.netOut" type="number" step="0.1" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Power (kW)</label><input v-model.number="form.power" type="number" step="0.01" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
