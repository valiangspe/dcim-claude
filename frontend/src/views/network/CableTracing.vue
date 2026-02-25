<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { cablesApi, type Cable } from '../../services/api'

const traces = ref<Cable[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<Cable | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { cableId: '', fromEnd: '', toEnd: '', type: '', length: '', color: '', path: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  traces.value = await cablesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: Cable) {
  editing.value = item
  form.value = { cableId: item.cableId, fromEnd: item.fromEnd, toEnd: item.toEnd, type: item.type, length: item.length, color: item.color, path: item.path }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await cablesApi.update(editing.value.id, form.value)
    else await cablesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: Cable) {
  if (!confirm(`Delete cable "${item.cableId}"?`)) return
  deleting.value = item.id
  try { await cablesApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cable Tracing</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Cables</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><div class="table-responsive"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Cable ID</th><th>From</th><th>To</th><th>Type</th><th>Length</th><th>Color</th><th>Path</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="t in traces" :key="t.id">
        <td class="fw-semibold font-monospace">{{ t.cableId }}</td>
        <td class="font-monospace small">{{ t.fromEnd }}</td><td class="font-monospace small">{{ t.toEnd }}</td>
        <td><span class="badge bg-secondary">{{ t.type }}</span></td><td>{{ t.length }}</td>
        <td><span class="badge" :class="{'bg-primary':t.color==='Blue','bg-warning text-dark':t.color==='Yellow','bg-danger':t.color==='Red','bg-success':t.color==='Green','bg-orange':t.color==='Orange'}" :style="t.color==='Orange'?'background-color:orange':''">{{ t.color }}</span></td>
        <td class="small text-muted">{{ t.path }}</td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(t)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(t)" :disabled="deleting === t.id">
            <span v-if="deleting === t.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Cable</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Cable ID</label><input v-model="form.cableId" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">From</label><input v-model="form.fromEnd" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">To</label><input v-model="form.toEnd" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Type</label><input v-model="form.type" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Length</label><input v-model="form.length" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Color</label><select v-model="form.color" class="form-select"><option value="Blue">Blue</option><option value="Green">Green</option><option value="Yellow">Yellow</option><option value="Red">Red</option><option value="Orange">Orange</option></select></div>
          <div class="mb-3"><label class="form-label">Path</label><input v-model="form.path" type="text" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
