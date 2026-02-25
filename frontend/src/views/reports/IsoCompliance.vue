<template>
  <div class="iso-compliance">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-warning" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-header bg-warning text-dark d-flex align-items-center justify-content-between">
        <h5 class="mb-0">ISO 22237 Compliance Checklist</h5>
        <button class="btn btn-sm btn-dark" @click="openCreate">+ Add Section</button>
      </div>
      <div class="card-body">
        <div class="row g-4">
          <div v-for="section in complianceSections" :key="section.id" class="col-lg-6">
            <div class="card border-light">
              <div class="card-header bg-light">
                <h6 class="mb-0">{{ section.name }}</h6>
                <small class="text-muted">{{ section.description }}</small>
              </div>
              <div class="card-body">
                <div v-for="item in parseItems(section.items)" :key="item.id" class="mb-3">
                  <div class="d-flex justify-content-between align-items-start">
                    <div class="flex-grow-1">
                      <small class="fw-semibold">{{ item.name }}</small>
                      <p class="text-muted small mb-0">{{ item.detail }}</p>
                    </div>
                    <span :class="getComplianceBadgeClass(item.status)">
                      {{ item.status }}
                    </span>
                  </div>
                </div>
              </div>
              <div class="card-footer bg-light d-flex justify-content-between align-items-center">
                <small class="text-muted">
                  {{ getSectionStats(section) }}
                </small>
                <div>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(section)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(section.id)">Delete</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer">
        <div class="row text-center">
          <div class="col-4">
            <small class="d-block text-muted">Total Checks</small>
            <strong>{{ getTotalChecks() }}</strong>
          </div>
          <div class="col-4">
            <small class="d-block text-muted">Compliant</small>
            <strong class="text-success">{{ getCompliantCount() }}</strong>
          </div>
          <div class="col-4">
            <small class="d-block text-muted">Non-Compliant</small>
            <strong class="text-danger">{{ getNonCompliantCount() }}</strong>
          </div>
        </div>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Compliance Section</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Section ID</label>
              <input v-model="form.sectionId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <textarea v-model="form.description" class="form-control" rows="2"></textarea>
            </div>
            <div class="mb-3">
              <label class="form-label">Items (JSON)</label>
              <textarea v-model="form.items" class="form-control" rows="4"></textarea>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { complianceSectionsApi, type ComplianceSection } from '../../services/api'

interface ComplianceItem {
  id: string
  name: string
  detail: string
  status: string
}

const complianceSections = ref<ComplianceSection[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ComplianceSection | null>(null)
const defaultForm = { sectionId: '', name: '', description: '', items: '[]' }
const form = ref({ ...defaultForm })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ComplianceSection) {
  editing.value = item
  form.value = { sectionId: item.sectionId, name: item.name, description: item.description, items: item.items }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await complianceSectionsApi.update(editing.value.id, form.value)
    else await complianceSectionsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await complianceSectionsApi.remove(id)
  await loadData()
}

async function loadData() {
  complianceSections.value = await complianceSectionsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

function parseItems(items: string): ComplianceItem[] {
  try {
    return JSON.parse(items)
  } catch {
    return []
  }
}

const getComplianceBadgeClass = (status: string): string => {
  const classes = 'badge'
  if (status === 'Pass') return `${classes} bg-success`
  if (status === 'Fail') return `${classes} bg-danger`
  return `${classes} bg-warning text-dark`
}

const getSectionStats = (section: ComplianceSection): string => {
  const parsed = parseItems(section.items)
  const pass = parsed.filter((item) => item.status === 'Pass').length
  const total = parsed.length
  return `${pass}/${total} compliant`
}

const getTotalChecks = (): number => {
  return complianceSections.value.reduce((sum, section) => sum + parseItems(section.items).length, 0)
}

const getCompliantCount = (): number => {
  return complianceSections.value.reduce(
    (sum, section) => sum + parseItems(section.items).filter((item) => item.status === 'Pass').length,
    0
  )
}

const getNonCompliantCount = (): number => {
  return complianceSections.value.reduce(
    (sum, section) => sum + parseItems(section.items).filter((item) => item.status === 'Fail').length,
    0
  )
}
</script>

<style scoped>
.iso-compliance {
  padding: 1rem;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-2px);
}
</style>
