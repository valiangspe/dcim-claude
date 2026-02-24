<template>
  <div class="standard-reports">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>
    <div v-else class="card">
      <div class="card-header bg-primary text-white d-flex align-items-center justify-content-between">
        <h5 class="mb-0">Standard Reports</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">Add Report</button>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div v-for="report in reports" :key="report.id" class="col-md-6">
            <div class="card border-start border-4" :style="{ borderColor: report.color }">
              <div class="card-body">
                <h6 class="card-title fw-bold">{{ report.name }}</h6>
                <p class="card-text text-muted small">{{ report.description }}</p>
                <div class="mb-3">
                  <small class="d-block text-muted">
                    <span class="badge" :style="{ backgroundColor: report.color }">
                      Last Run: {{ report.lastRun }}
                    </span>
                  </small>
                </div>
                <div class="d-flex gap-2">
                  <button class="btn btn-sm btn-primary flex-grow-1">Generate Now</button>
                  <button class="btn btn-sm btn-outline-secondary">View History</button>
                  <button class="btn btn-sm btn-outline-primary" @click="openEdit(report)">Edit</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer text-muted">
        Available reports: {{ reports.length }}
      </div>
    </div>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Report</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <textarea v-model="form.description" class="form-control" rows="3"></textarea>
            </div>
            <div class="mb-3">
              <label class="form-label">Color</label>
              <input v-model="form.color" type="color" class="form-control form-control-color" />
            </div>
            <div class="mb-3">
              <label class="form-label">Last Run</label>
              <input v-model="form.lastRun" type="date" class="form-control" />
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
import { reportsApi, type Report } from '../../services/api'

const reports = ref<Report[]>([])
const loading = ref(true)
const error = ref('')

const showModal = ref(false)
const editing = ref<Report | null>(null)
const saving = ref(false)
const form = ref({ name: '', description: '', color: '#0d6efd', lastRun: '' })

async function loadData() {
  reports.value = await reportsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'Failed to load reports'
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editing.value = null
  form.value = { name: '', description: '', color: '#0d6efd', lastRun: '' }
  showModal.value = true
}

function openEdit(report: Report) {
  editing.value = report
  form.value = { name: report.name, description: report.description, color: report.color, lastRun: report.lastRun }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await reportsApi.update(editing.value.id, form.value)
    else await reportsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}
</script>

<style scoped>
.standard-reports {
  padding: 1rem;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-2px);
}
</style>
