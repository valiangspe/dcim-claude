<template>
  <div class="export-pdf">
    <div class="card">
      <div class="card-header bg-danger text-white">
        <h5 class="mb-0">Export as PDF / Excel</h5>
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
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface Template {
  id: string
  name: string
  pages: number
  description: string
}

const exportFormat = ref<string>('pdf')
const selectedTemplate = ref<string>('comprehensive')
const includeCharts = ref<boolean>(true)
const includeSummary = ref<boolean>(true)
const includeFootnotes = ref<boolean>(false)
const recipientEmail = ref<string>('')

const templates: Template[] = [
  {
    id: 'executive',
    name: 'Executive Summary',
    pages: 3,
    description: 'High-level overview with key metrics and recommendations'
  },
  {
    id: 'comprehensive',
    name: 'Comprehensive Report',
    pages: 15,
    description: 'Full detailed analysis with charts, tables, and recommendations'
  },
  {
    id: 'technical',
    name: 'Technical Analysis',
    pages: 20,
    description: 'In-depth technical metrics and performance data'
  },
  {
    id: 'compliance',
    name: 'Compliance Report',
    pages: 10,
    description: 'ISO 22237 compliance checklist and audit trail'
  },
  {
    id: 'capacity',
    name: 'Capacity Planning',
    pages: 8,
    description: 'Current utilization and future capacity forecasts'
  }
]

const handleExport = () => {
  alert(
    `Generating ${exportFormat.value.toUpperCase()} with ${selectedTemplate.value} template\n` +
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
