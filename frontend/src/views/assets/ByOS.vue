<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { osRecordsApi, type OsRecord } from '../../services/api'

const osList = ref<OsRecord[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<OsRecord | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { os: '', count: 0, pct: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  osList.value = await osRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: OsRecord) {
  editing.value = item
  form.value = { os: item.os, count: item.count, pct: item.pct }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await osRecordsApi.update(editing.value.id, form.value)
    else await osRecordsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: OsRecord) {
  if (!confirm(`Delete OS "${item.os}"?`)) return
  deleting.value = item.id
  try { await osRecordsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Hosts by OS</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>Operating Systems</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Operating System</th><th class="text-end">Count</th><th style="width:35%">Distribution</th><th class="text-end">%</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="o in osList" :key="o.id"><td class="fw-semibold">{{ o.os }}</td><td class="text-end">{{ o.count }}</td>
        <td><div class="progress" style="height:10px"><div class="progress-bar bg-primary" :style="{width:o.pct+'%'}"></div></div></td><td class="text-end">{{ o.pct }}%</td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(o)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(o)" :disabled="deleting === o.id">
            <span v-if="deleting === o.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} OS Record</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Operating System</label><input v-model="form.os" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Count</label><input v-model.number="form.count" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Percentage</label><input v-model.number="form.pct" type="number" step="0.1" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
