<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { dataSwitchesApi, type DataSwitch } from '../../services/api'

const switches = ref<DataSwitch[]>([])
const loading = ref(true)

const totalPorts = computed(() => switches.value.reduce((a, s) => a + s.total, 0))
const usedPorts = computed(() => switches.value.reduce((a, s) => a + s.used, 0))

const showModal = ref(false)
const saving = ref(false)
const editing = ref<DataSwitch | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', total: 0, used: 0, speed: '', status: 'active' }
const form = ref({ ...defaultForm })

async function loadData() {
  switches.value = await dataSwitchesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: DataSwitch) {
  editing.value = item
  form.value = { name: item.name, total: item.total, used: item.used, speed: item.speed, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await dataSwitchesApi.update(editing.value.id, form.value)
    else await dataSwitchesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: DataSwitch) {
  if (!confirm(`Delete switch "${item.name}"?`)) return
  deleting.value = item.id
  try { await dataSwitchesApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Data Ports</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="row g-3 mb-4">
      <div class="col-md-3"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-primary mb-0">{{ totalPorts }}</h3><small class="text-muted">Total Ports</small></div></div></div>
      <div class="col-md-3"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-warning mb-0">{{ usedPorts }}</h3><small class="text-muted">Used</small></div></div></div>
      <div class="col-md-3"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-success mb-0">{{ totalPorts - usedPorts }}</h3><small class="text-muted">Free</small></div></div></div>
      <div class="col-md-3"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-info mb-0">{{ totalPorts > 0 ? (usedPorts/totalPorts*100).toFixed(1) : '0.0' }}%</h3><small class="text-muted">Utilization</small></div></div></div>
    </div>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Data Switches</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Switch</th><th>Speed</th><th class="text-end">Total</th><th class="text-end">Used</th><th class="text-end">Free</th><th style="width:20%">Utilization</th><th>Status</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="s in switches" :key="s.id">
        <td class="fw-semibold">{{ s.name }}</td><td><span class="badge bg-secondary">{{ s.speed }}</span></td>
        <td class="text-end">{{ s.total }}</td><td class="text-end">{{ s.used }}</td><td class="text-end">{{ s.total - s.used }}</td>
        <td><div class="d-flex align-items-center gap-2"><div class="progress flex-fill" style="height:8px"><div class="progress-bar" :class="s.used/s.total>0.9?'bg-danger':'bg-primary'" :style="{width:(s.used/s.total*100)+'%'}"></div></div><small>{{ (s.used/s.total*100).toFixed(0) }}%</small></div></td>
        <td><span class="badge" :class="s.status==='active'?'bg-success':'bg-warning text-dark'">{{ s.status }}</span></td>
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
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Data Switch</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Total Ports</label><input v-model.number="form.total" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Used Ports</label><input v-model.number="form.used" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Speed</label><input v-model="form.speed" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Status</label><select v-model="form.status" class="form-select"><option value="active">active</option><option value="inactive">inactive</option></select></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
