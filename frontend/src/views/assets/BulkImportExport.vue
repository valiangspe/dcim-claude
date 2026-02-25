<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { importExportRecordsApi, type ImportExportRecord } from '../../services/api'

const history = ref<ImportExportRecord[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<ImportExportRecord | null>(null)
const saving = ref(false)

const defaultForm = { date: '', type: '', file: '', records: 0, status: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  history.value = await importExportRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ImportExportRecord) {
  editing.value = item
  form.value = { date: item.date, type: item.type, file: item.file, records: item.records, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await importExportRecordsApi.update(editing.value.id, form.value)
    } else {
      await importExportRecordsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this record?')) return
  await importExportRecordsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Bulk Import / Export</h4>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Record</button>
    </div>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="row g-3 mb-4">
      <div class="col-md-6"><div class="card border-0 shadow-sm h-100"><div class="card-body text-center">
        <h5>Import CSV</h5><p class="text-muted small">Upload a CSV file to bulk-import assets, parts, or configuration.</p>
        <button class="btn btn-primary">Choose File & Import</button>
      </div></div></div>
      <div class="col-md-6"><div class="card border-0 shadow-sm h-100"><div class="card-body text-center">
        <h5>Export CSV</h5><p class="text-muted small">Export current inventory, assets, or parts data to CSV.</p>
        <button class="btn btn-outline-primary">Export All Assets</button>
      </div></div></div>
    </div>
    <div class="card border-0 shadow-sm"><div class="card-header bg-transparent fw-semibold">Recent Operations</div><div class="card-body p-0"><table class="table table-hover mb-0">
      <thead class="table-light"><tr><th>Date</th><th>Type</th><th>File</th><th class="text-end">Records</th><th>Status</th><th>Actions</th></tr></thead>
      <tbody><tr v-for="h in history" :key="h.id"><td>{{ h.date }}</td><td><span class="badge" :class="h.type==='Import'?'bg-primary':'bg-info text-dark'">{{ h.type }}</span></td>
        <td class="font-monospace small">{{ h.file }}</td><td class="text-end">{{ h.records }}</td>
        <td><span class="badge" :class="h.status==='success'?'bg-success':'bg-warning text-dark'">{{ h.status }}</span></td>
        <td>
          <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(h)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(h.id)">Delete</button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Record</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Date</label><input v-model="form.date" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Type</label><input v-model="form.type" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">File</label><input v-model="form.file" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Records</label><input v-model.number="form.records" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Status</label><input v-model="form.status" type="text" class="form-control" /></div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
