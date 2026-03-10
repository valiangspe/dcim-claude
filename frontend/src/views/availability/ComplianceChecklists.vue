<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { complianceChecklistsApi, type ComplianceChecklist } from '../../services/api'

const checklists = ref<ComplianceChecklist[]>([])
const loading = ref(true)
const error = ref('')
const showModal = ref(false)
const editing = ref<ComplianceChecklist | null>(null)
const saving = ref(false)
const filterCategory = ref('')
const filterStatus = ref('')

const form = ref({
  category: '',
  requirement: '',
  isoReference: '',
  description: '',
  status: 'pending',
  assignedTo: '',
  dueDate: '',
  evidence: '',
  notes: '',
})

const categories = ['Power', 'Cooling', 'Physical Security', 'Network', 'Environmental', 'Space', 'General']

const statusColors: Record<string, string> = {
  compliant: 'success',
  'non-compliant': 'danger',
  'in-progress': 'warning',
  pending: 'secondary',
  'not-applicable': 'light',
}

const kpis = computed(() => {
  const total = checklists.value.length
  const compliant = checklists.value.filter(c => c.status === 'compliant').length
  const nonCompliant = checklists.value.filter(c => c.status === 'non-compliant').length
  const inProgress = checklists.value.filter(c => c.status === 'in-progress').length
  const pending = checklists.value.filter(c => c.status === 'pending').length
  const score = total > 0 ? Math.round((compliant / total) * 100) : 0
  return [
    { label: 'Compliance Score', value: `${score}%`, color: score >= 80 ? 'success' : score >= 60 ? 'warning' : 'danger', trend: `${compliant}/${total} requirements met` },
    { label: 'Compliant', value: String(compliant), color: 'success', trend: 'Requirements satisfied' },
    { label: 'Non-Compliant', value: String(nonCompliant), color: 'danger', trend: 'Action required' },
    { label: 'In Progress', value: String(inProgress), color: 'warning', trend: 'Being addressed' },
    { label: 'Pending', value: String(pending), color: 'secondary', trend: 'Not yet assessed' },
    { label: 'Total Requirements', value: String(total), color: 'primary', trend: 'ISO/IEC 22237' },
  ]
})

const filtered = computed(() => checklists.value.filter(c => {
  if (filterCategory.value && c.category !== filterCategory.value) return false
  if (filterStatus.value && c.status !== filterStatus.value) return false
  return true
}))

async function loadData() {
  checklists.value = await complianceChecklistsApi.getAll()
}

onMounted(async () => {
  try { await loadData() }
  catch (e: unknown) { error.value = e instanceof Error ? e.message : 'Failed to load checklists' }
  finally { loading.value = false }
})

function openCreate() {
  editing.value = null
  form.value = { category: '', requirement: '', isoReference: '', description: '', status: 'pending', assignedTo: '', dueDate: '', evidence: '', notes: '' }
  showModal.value = true
}

function openEdit(c: ComplianceChecklist) {
  editing.value = c
  form.value = { category: c.category, requirement: c.requirement, isoReference: c.isoReference, description: c.description, status: c.status, assignedTo: c.assignedTo, dueDate: c.dueDate, evidence: c.evidence, notes: c.notes }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await complianceChecklistsApi.update(editing.value.id, form.value)
    else await complianceChecklistsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(id: number) {
  if (!confirm('Delete this checklist item?')) return
  await complianceChecklistsApi.remove(id)
  await loadData()
}

async function toggleStatus(item: ComplianceChecklist) {
  const next: Record<string, string> = { pending: 'in-progress', 'in-progress': 'compliant', compliant: 'non-compliant', 'non-compliant': 'pending' }
  await complianceChecklistsApi.update(item.id, { status: next[item.status] ?? 'pending' })
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-1">Structured Compliance Checklists</h4>
    <p class="text-muted small mb-4">ISO/IEC 22237 compliance requirements — track, assign, and evidence each obligation</p>

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

      <!-- Filters + Table -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent d-flex align-items-center justify-content-between flex-wrap gap-2">
          <span class="fw-semibold">Requirements</span>
          <div class="d-flex gap-2 align-items-center flex-wrap">
            <select v-model="filterCategory" class="form-select form-select-sm" style="width:160px">
              <option value="">All Categories</option>
              <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
            </select>
            <select v-model="filterStatus" class="form-select form-select-sm" style="width:160px">
              <option value="">All Statuses</option>
              <option value="compliant">Compliant</option>
              <option value="non-compliant">Non-Compliant</option>
              <option value="in-progress">In Progress</option>
              <option value="pending">Pending</option>
              <option value="not-applicable">N/A</option>
            </select>
            <button class="btn btn-sm btn-primary" @click="openCreate">Add Requirement</button>
          </div>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Category</th>
                <th>Requirement</th>
                <th>ISO Reference</th>
                <th>Status</th>
                <th>Assigned To</th>
                <th>Due Date</th>
                <th>Evidence</th>
                <th style="width:100px">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in filtered" :key="item.id">
                <td><span class="badge bg-light text-dark border">{{ item.category }}</span></td>
                <td>
                  <div class="fw-semibold">{{ item.requirement }}</div>
                  <small class="text-muted">{{ item.description }}</small>
                </td>
                <td class="text-muted small font-monospace">{{ item.isoReference }}</td>
                <td>
                  <span class="badge" :class="`bg-${statusColors[item.status] ?? 'secondary'}`"
                    role="button" @click="toggleStatus(item)" title="Click to advance status">
                    {{ item.status }}
                  </span>
                </td>
                <td>{{ item.assignedTo || '—' }}</td>
                <td class="text-muted small">{{ item.dueDate || '—' }}</td>
                <td>
                  <span v-if="item.evidence" class="badge bg-success-subtle text-success border border-success-subtle">
                    Evidence attached
                  </span>
                  <span v-else class="text-muted small">—</span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(item)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(item.id)">Delete</button>
                </td>
              </tr>
              <tr v-if="filtered.length === 0">
                <td colspan="8" class="text-center text-muted py-4">No checklist items match the current filters.</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Compliance Requirement</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="row g-3">
              <div class="col-6">
                <label class="form-label">Category</label>
                <select v-model="form.category" class="form-select">
                  <option value="">Select category</option>
                  <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                </select>
              </div>
              <div class="col-6">
                <label class="form-label">ISO Reference</label>
                <input v-model="form.isoReference" type="text" class="form-control" placeholder="e.g. ISO 22237-2:2018 §5.3" />
              </div>
              <div class="col-12">
                <label class="form-label">Requirement</label>
                <input v-model="form.requirement" type="text" class="form-control" placeholder="Short requirement title" />
              </div>
              <div class="col-12">
                <label class="form-label">Description</label>
                <textarea v-model="form.description" class="form-control" rows="2" placeholder="Detailed description of the requirement"></textarea>
              </div>
              <div class="col-4">
                <label class="form-label">Status</label>
                <select v-model="form.status" class="form-select">
                  <option value="pending">Pending</option>
                  <option value="in-progress">In Progress</option>
                  <option value="compliant">Compliant</option>
                  <option value="non-compliant">Non-Compliant</option>
                  <option value="not-applicable">Not Applicable</option>
                </select>
              </div>
              <div class="col-4">
                <label class="form-label">Assigned To</label>
                <input v-model="form.assignedTo" type="text" class="form-control" />
              </div>
              <div class="col-4">
                <label class="form-label">Due Date</label>
                <input v-model="form.dueDate" type="date" class="form-control" />
              </div>
              <div class="col-12">
                <label class="form-label">Evidence Reference</label>
                <input v-model="form.evidence" type="text" class="form-control" placeholder="Document ID or description of evidence" />
              </div>
              <div class="col-12">
                <label class="form-label">Notes</label>
                <textarea v-model="form.notes" class="form-control" rows="2"></textarea>
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
