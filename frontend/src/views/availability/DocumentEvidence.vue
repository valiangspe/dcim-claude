<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { evidenceDocumentsApi, type EvidenceDocument } from '../../services/api'

const documents = ref<EvidenceDocument[]>([])
const loading = ref(true)
const error = ref('')
const showModal = ref(false)
const editing = ref<EvidenceDocument | null>(null)
const saving = ref(false)
const filterType = ref('')
const filterStatus = ref('')
const search = ref('')

const form = ref({
  title: '',
  type: '',
  isoReference: '',
  description: '',
  fileName: '',
  fileSize: '',
  uploadedBy: '',
  uploadedDate: '',
  expiryDate: '',
  status: 'current',
  tags: '',
})

const docTypes = ['Policy', 'Procedure', 'Audit Report', 'Test Certificate', 'Inspection Record', 'Risk Assessment', 'Maintenance Record', 'Training Record', 'Other']

const statusColors: Record<string, string> = {
  current: 'success',
  'under-review': 'warning',
  expired: 'danger',
  archived: 'secondary',
  draft: 'info',
}

const typeIcons: Record<string, string> = {
  'Policy': '📋',
  'Procedure': '📄',
  'Audit Report': '🔍',
  'Test Certificate': '✅',
  'Inspection Record': '🔎',
  'Risk Assessment': '⚠️',
  'Maintenance Record': '🔧',
  'Training Record': '🎓',
  'Other': '📁',
}

const kpis = computed(() => {
  const total = documents.value.length
  const current = documents.value.filter(d => d.status === 'current').length
  const expired = documents.value.filter(d => d.status === 'expired').length
  const review = documents.value.filter(d => d.status === 'under-review').length
  const expiringSoon = documents.value.filter(d => {
    if (!d.expiryDate) return false
    const days = (new Date(d.expiryDate).getTime() - Date.now()) / 86400000
    return days > 0 && days <= 30
  }).length
  return [
    { label: 'Total Documents', value: String(total), color: 'primary', trend: 'Evidence library' },
    { label: 'Current', value: String(current), color: 'success', trend: 'Up to date' },
    { label: 'Under Review', value: String(review), color: 'warning', trend: 'Pending approval' },
    { label: 'Expired', value: String(expired), color: 'danger', trend: 'Renewal required' },
    { label: 'Expiring Soon', value: String(expiringSoon), color: 'warning', trend: 'Within 30 days' },
    { label: 'Coverage', value: total > 0 ? `${Math.round((current / total) * 100)}%` : '—', color: 'info', trend: 'Current / Total' },
  ]
})

const filtered = computed(() => documents.value.filter(d => {
  if (filterType.value && d.type !== filterType.value) return false
  if (filterStatus.value && d.status !== filterStatus.value) return false
  if (search.value && !d.title.toLowerCase().includes(search.value.toLowerCase()) && !d.isoReference.toLowerCase().includes(search.value.toLowerCase())) return false
  return true
}))

async function loadData() {
  documents.value = await evidenceDocumentsApi.getAll()
}

onMounted(async () => {
  try { await loadData() }
  catch (e: unknown) { error.value = e instanceof Error ? e.message : 'Failed to load documents' }
  finally { loading.value = false }
})

function openCreate() {
  editing.value = null
  form.value = { title: '', type: '', isoReference: '', description: '', fileName: '', fileSize: '', uploadedBy: '', uploadedDate: '', expiryDate: '', status: 'current', tags: '' }
  showModal.value = true
}

function openEdit(d: EvidenceDocument) {
  editing.value = d
  form.value = { title: d.title, type: d.type, isoReference: d.isoReference, description: d.description, fileName: d.fileName, fileSize: d.fileSize, uploadedBy: d.uploadedBy, uploadedDate: d.uploadedDate, expiryDate: d.expiryDate, status: d.status, tags: d.tags }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await evidenceDocumentsApi.update(editing.value.id, form.value)
    else await evidenceDocumentsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(id: number) {
  if (!confirm('Delete this document?')) return
  await evidenceDocumentsApi.remove(id)
  await loadData()
}

function isExpiringSoon(d: EvidenceDocument) {
  if (!d.expiryDate) return false
  const days = (new Date(d.expiryDate).getTime() - Date.now()) / 86400000
  return days > 0 && days <= 30
}
</script>

<template>
  <div>
    <h4 class="mb-1">Document &amp; Evidence Management</h4>
    <p class="text-muted small mb-4">Manage compliance documents, certifications, audit reports and evidence required for ISO/IEC 22237</p>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>
    <template v-else>

      <!-- KPI Cards -->
      <div class="row g-3 mb-4">
        <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg-4 col-xl-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <h3 :class="`text-${kpi.color} mb-1`">{{ kpi.value }}</h3>
              <small class="text-muted d-block">{{ kpi.label }}</small>
              <small class="text-secondary">{{ kpi.trend }}</small>
            </div>
          </div>
        </div>
      </div>

      <!-- Documents Table -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent d-flex align-items-center justify-content-between flex-wrap gap-2">
          <span class="fw-semibold">Documents</span>
          <div class="d-flex gap-2 align-items-center flex-wrap">
            <input v-model="search" type="text" class="form-control form-control-sm" placeholder="Search..." style="width:180px" />
            <select v-model="filterType" class="form-select form-select-sm" style="width:160px">
              <option value="">All Types</option>
              <option v-for="t in docTypes" :key="t" :value="t">{{ t }}</option>
            </select>
            <select v-model="filterStatus" class="form-select form-select-sm" style="width:150px">
              <option value="">All Statuses</option>
              <option value="current">Current</option>
              <option value="under-review">Under Review</option>
              <option value="expired">Expired</option>
              <option value="archived">Archived</option>
              <option value="draft">Draft</option>
            </select>
            <button class="btn btn-sm btn-primary" @click="openCreate">Add Document</button>
          </div>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Title</th>
                <th>Type</th>
                <th>ISO Reference</th>
                <th>File</th>
                <th>Uploaded By</th>
                <th>Uploaded</th>
                <th>Expiry</th>
                <th>Status</th>
                <th style="width:80px">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="doc in filtered" :key="doc.id" :class="{ 'table-warning': isExpiringSoon(doc) }">
                <td>
                  <div class="fw-semibold">{{ typeIcons[doc.type] || '📁' }} {{ doc.title }}</div>
                  <small class="text-muted">{{ doc.description }}</small>
                </td>
                <td><span class="badge bg-light text-dark border">{{ doc.type }}</span></td>
                <td class="text-muted small font-monospace">{{ doc.isoReference || '—' }}</td>
                <td class="text-muted small">
                  <span v-if="doc.fileName">{{ doc.fileName }}<br><span class="text-secondary">{{ doc.fileSize }}</span></span>
                  <span v-else>—</span>
                </td>
                <td>{{ doc.uploadedBy || '—' }}</td>
                <td class="text-muted small">{{ doc.uploadedDate || '—' }}</td>
                <td class="small">
                  <span v-if="doc.expiryDate" :class="isExpiringSoon(doc) ? 'text-warning fw-semibold' : 'text-muted'">
                    {{ doc.expiryDate }}
                    <span v-if="isExpiringSoon(doc)" class="badge bg-warning ms-1">Soon</span>
                  </span>
                  <span v-else class="text-muted">—</span>
                </td>
                <td><span class="badge" :class="`bg-${statusColors[doc.status] ?? 'secondary'}`">{{ doc.status }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(doc)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(doc.id)">Delete</button>
                </td>
              </tr>
              <tr v-if="filtered.length === 0">
                <td colspan="9" class="text-center text-muted py-4">No documents found. Add evidence documents to build your compliance library.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Document</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="row g-3">
              <div class="col-8">
                <label class="form-label">Title</label>
                <input v-model="form.title" type="text" class="form-control" />
              </div>
              <div class="col-4">
                <label class="form-label">Type</label>
                <select v-model="form.type" class="form-select">
                  <option value="">Select type</option>
                  <option v-for="t in docTypes" :key="t" :value="t">{{ t }}</option>
                </select>
              </div>
              <div class="col-6">
                <label class="form-label">ISO Reference</label>
                <input v-model="form.isoReference" type="text" class="form-control" placeholder="e.g. ISO 22237-2 §6" />
              </div>
              <div class="col-6">
                <label class="form-label">Status</label>
                <select v-model="form.status" class="form-select">
                  <option value="draft">Draft</option>
                  <option value="current">Current</option>
                  <option value="under-review">Under Review</option>
                  <option value="expired">Expired</option>
                  <option value="archived">Archived</option>
                </select>
              </div>
              <div class="col-12">
                <label class="form-label">Description</label>
                <textarea v-model="form.description" class="form-control" rows="2"></textarea>
              </div>
              <div class="col-6">
                <label class="form-label">File Name</label>
                <input v-model="form.fileName" type="text" class="form-control" placeholder="e.g. power-audit-2025.pdf" />
              </div>
              <div class="col-6">
                <label class="form-label">File Size</label>
                <input v-model="form.fileSize" type="text" class="form-control" placeholder="e.g. 2.4 MB" />
              </div>
              <div class="col-4">
                <label class="form-label">Uploaded By</label>
                <input v-model="form.uploadedBy" type="text" class="form-control" />
              </div>
              <div class="col-4">
                <label class="form-label">Upload Date</label>
                <input v-model="form.uploadedDate" type="date" class="form-control" />
              </div>
              <div class="col-4">
                <label class="form-label">Expiry Date</label>
                <input v-model="form.expiryDate" type="date" class="form-control" />
              </div>
              <div class="col-12">
                <label class="form-label">Tags</label>
                <input v-model="form.tags" type="text" class="form-control" placeholder="Comma-separated tags" />
              </div>
            </div>
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
