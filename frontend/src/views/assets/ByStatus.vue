<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { assetStatusesApi, type AssetStatus } from '../../services/api'

const statuses = ref<AssetStatus[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<AssetStatus | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { status: '', count: 0, pct: 0, color: 'primary' }
const form = ref({ ...defaultForm })

async function loadData() {
  statuses.value = await assetStatusesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: AssetStatus) {
  editing.value = item
  form.value = { status: item.status, count: item.count, pct: item.pct, color: item.color }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await assetStatusesApi.update(editing.value.id, form.value)
    else await assetStatusesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: AssetStatus) {
  if (!confirm(`Delete status "${item.status}"?`)) return
  deleting.value = item.id
  try { await assetStatusesApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Assets by Status</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-end mb-3"><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
    <div class="row g-3 mb-4">
      <div v-for="s in statuses" :key="s.id" class="col-sm-6 col-lg-3">
        <div class="card border-0 shadow-sm text-center h-100"><div class="card-body">
          <h2 :class="`text-${s.color} mb-0`">{{ s.count }}</h2>
          <div class="progress my-2" style="height:6px"><div :class="`progress-bar bg-${s.color}`" :style="{width:s.pct+'%'}"></div></div>
          <small class="text-muted">{{ s.status }} ({{ s.pct }}%)</small>
          <div class="mt-2">
            <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(s)">Edit</button>
            <button class="btn btn-sm btn-outline-danger" @click="remove(s)" :disabled="deleting === s.id">
              <span v-if="deleting === s.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
            </button>
          </div>
        </div></div>
      </div>
    </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Asset Status</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Status</label><input v-model="form.status" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Count</label><input v-model.number="form.count" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Percentage</label><input v-model.number="form.pct" type="number" step="0.1" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Color</label><select v-model="form.color" class="form-select"><option value="primary">primary</option><option value="success">success</option><option value="warning">warning</option><option value="danger">danger</option><option value="info">info</option><option value="secondary">secondary</option></select></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
