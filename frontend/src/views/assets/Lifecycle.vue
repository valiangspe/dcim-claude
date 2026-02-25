<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { lifecyclePhasesApi, type LifecyclePhase } from '../../services/api'

const phases = ref<LifecyclePhase[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<LifecyclePhase | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { phase: '', count: 0, items: '', eta: '' }
const form = ref({ ...defaultForm })

function phaseColor(p: string) { return p==='Production'?'bg-success':p==='Deployment'?'bg-primary':p==='Staging'?'bg-info text-dark':p==='Procurement'?'bg-warning text-dark':p==='End of Life'?'bg-danger':'bg-secondary' }

async function loadData() {
  phases.value = await lifecyclePhasesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: LifecyclePhase) {
  editing.value = item
  form.value = { phase: item.phase, count: item.count, items: item.items, eta: item.eta }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await lifecyclePhasesApi.update(editing.value.id, form.value)
    else await lifecyclePhasesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: LifecyclePhase) {
  if (!confirm(`Delete phase "${item.phase}"?`)) return
  deleting.value = item.id
  try { await lifecyclePhasesApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Asset Lifecycle</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Lifecycle Phases</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Phase</th><th class="text-end">Assets</th><th>Items</th><th>ETA / Notes</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="p in phases" :key="p.id">
        <td><span class="badge" :class="phaseColor(p.phase)">{{ p.phase }}</span></td>
        <td class="text-end fw-semibold">{{ p.count }}</td><td class="text-muted small">{{ p.items }}</td><td class="small">{{ p.eta }}</td>
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
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Lifecycle Phase</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Phase</label><input v-model="form.phase" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Count</label><input v-model.number="form.count" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Items</label><input v-model="form.items" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">ETA / Notes</label><input v-model="form.eta" type="text" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
