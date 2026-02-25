<template>
  <div class="export-pdf">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-danger" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-header bg-danger text-white d-flex align-items-center justify-content-between">
        <h5 class="mb-0">Export as PDF / Excel</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">+ Add Template</button>
      </div>
      <div class="card-body">
        <form @submit.prevent="handleExport">
          <div class="mb-4">
            <label class="form-label fw-bold">Export Format</label>
            <div class="btn-group w-100" role="group">
              <input v-model="exportFormat" type="radio" class="btn-check" id="pdf" value="pdf" />
              <label class="btn btn-outline-primary" for="pdf">PDF</label>

              <input v-model="exportFormat" type="radio" class="btn-check" id="excel" value="excel" />
              <label class="btn btn-outline-primary" for="excel">Excel</label>

              <input v-model="exportFormat" type="radio" class="btn-check" id="both" value="both" />
              <label class="btn btn-outline-primary" for="both">Both</label>
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-bold">Report Template</label>
            <select v-model="selectedTemplate" class="form-select">
              <option v-for="template in templates" :key="template.id" :value="template.id">
                {{ template.name }} - {{ template.pages }} pages
              </option>
            </select>
            <small class="text-muted d-block mt-2">
              {{ templates.find((t) => t.id === selectedTemplate)?.description }}
            </small>
            <div class="mt-2">
              <button v-if="templates.find((t) => t.id === selectedTemplate)" class="btn btn-sm btn-outline-primary me-1" @click="openEdit(templates.find((t) => t.id === selectedTemplate)!)">Edit Template</button>
              <button v-if="templates.find((t) => t.id === selectedTemplate)" class="btn btn-sm btn-outline-danger" @click="removeTemplate(selectedTemplate)">Delete Template</button>
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-bold">Report Options</label>
            <div class="form-check">
              <input v-model="includeCharts" type="checkbox" class="form-check-input" id="charts" />
              <label class="form-check-label" for="charts">Include charts and graphs</label>
            </div>
            <div class="form-check">
              <input v-model="includeSummary" type="checkbox" class="form-check-input" id="summary" />
              <label class="form-check-label" for="summary">Include executive summary</label>
            </div>
            <div class="form-check">
              <input v-model="includeFootnotes" type="checkbox" class="form-check-input" id="footnotes" />
              <label class="form-check-label" for="footnotes">Include footnotes and details</label>
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-bold">Recipient Email (optional)</label>
            <input v-model="recipientEmail" type="email" class="form-control" placeholder="user@company.com" />
          </div>

          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-danger">
              <i class="bi bi-file-pdf me-2"></i>Generate & Download
            </button>
            <button type="reset" class="btn btn-outline-secondary">Clear</button>
          </div>
        </form>
      </div>
      <div class="card-footer bg-light">
        <small class="text-muted">Processing time: 15-30 seconds</small>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Template</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="tplForm.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Pages</label>
              <input v-model.number="tplForm.pages" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <textarea v-model="tplForm.description" class="form-control" rows="3"></textarea>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="saveTemplate" :disabled="saving">
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
import { exportTemplatesApi, type ExportTemplate } from '../../services/api'

const exportFormat = ref<string>('pdf')
const selectedTemplate = ref<number>(0)
const includeCharts = ref<boolean>(true)
const includeSummary = ref<boolean>(true)
const includeFootnotes = ref<boolean>(false)
const recipientEmail = ref<string>('')
const loading = ref(true)

const templates = ref<ExportTemplate[]>([])
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ExportTemplate | null>(null)
const tplForm = ref({ name: '', pages: 1, description: '' })

function openCreate() {
  editing.value = null
  tplForm.value = { name: '', pages: 1, description: '' }
  showModal.value = true
}

function openEdit(item: ExportTemplate) {
  editing.value = item
  tplForm.value = { name: item.name, pages: item.pages, description: item.description }
  showModal.value = true
}

async function saveTemplate() {
  saving.value = true
  try {
    if (editing.value) await exportTemplatesApi.update(editing.value.id, tplForm.value)
    else await exportTemplatesApi.create(tplForm.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function removeTemplate(id: number) {
  if (!confirm('Are you sure?')) return
  await exportTemplatesApi.remove(id)
  await loadData()
}

async function loadData() {
  templates.value = await exportTemplatesApi.getAll()
  if (templates.value.length > 0) {
    selectedTemplate.value = templates.value[0].id
  }
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const handleExport = () => {
  alert(
    `Generating ${exportFormat.value.toUpperCase()} with template #${selectedTemplate.value}\n` +
      `Charts: ${includeCharts.value}\nSummary: ${includeSummary.value}\nFootnotes: ${includeFootnotes.value}\n` +
      (recipientEmail.value ? `Email to: ${recipientEmail.value}` : 'Download to local machine')
  )
}
</script>

<style scoped>
.export-pdf {
  padding: 1rem;
  max-width: 600px;
}

.btn-group {
  display: flex;
}

.btn {
  flex: 1;
}
</style>
