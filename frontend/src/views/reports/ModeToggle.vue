<template>
  <div class="mode-toggle">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-info" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-header bg-info text-white">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Report Display Mode</h5>
          <div class="d-flex gap-2 align-items-center">
          <button class="btn btn-sm btn-light" @click="openCreate">+ Add Metric</button>
          <div class="btn-group btn-group-sm" role="group">
            <input v-model="viewMode" type="radio" class="btn-check" id="concise" value="concise" />
            <label class="btn btn-light" for="concise">Concise</label>

            <input v-model="viewMode" type="radio" class="btn-check" id="detailed" value="detailed" />
            <label class="btn btn-light" for="detailed">Detailed</label>
          </div>
          </div>
        </div>
      </div>
      <div class="card-body">
        <!-- CONCISE VIEW -->
        <div v-if="viewMode === 'concise'">
          <div class="row g-3">
            <div v-for="metric in conciseMetrics" :key="metric.id" class="col-md-4">
              <div class="text-center">
                <h6 class="text-muted">{{ metric.name }}</h6>
                <h3 :style="{ color: metric.color }">{{ metric.value }}</h3>
                <small class="text-muted">{{ metric.unit }}</small>
              </div>
            </div>
          </div>
          <hr class="my-4" />
          <div class="row g-3">
            <div v-for="status in conciseStatus" :key="status.id" class="col-md-4 text-center">
              <span :class="getStatusBadgeClass(status.color)" class="badge fs-6">
                {{ status.name }}: {{ status.value }}
              </span>
            </div>
          </div>
        </div>

        <!-- DETAILED VIEW -->
        <div v-else>
          <div class="table-responsive">
            <table class="table table-sm">
              <thead>
                <tr class="table-light">
                  <th>Metric</th>
                  <th>Value</th>
                  <th>Unit</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="metric in detailedMetrics" :key="metric.id">
                  <td><strong>{{ metric.name }}</strong></td>
                  <td>
                    <span class="badge" :style="{ backgroundColor: metric.color }">
                      {{ metric.value }}
                    </span>
                  </td>
                  <td>{{ metric.unit }}</td>
                  <td>
                    <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(metric)">Edit</button>
                    <button class="btn btn-sm btn-outline-danger" @click="remove(metric.id)">Delete</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <hr class="my-4" />

          <h6 class="fw-bold mb-3">Performance Summary</h6>
          <div v-for="metric in summaryMetrics" :key="metric.id" class="mb-3">
            <div class="d-flex justify-content-between align-items-center mb-1">
              <small class="fw-semibold">{{ metric.name }}</small>
              <small class="text-muted">{{ metric.value }}{{ metric.unit }}</small>
            </div>
            <div class="progress" style="height: 20px">
              <div
                class="progress-bar"
                :style="{ width: metric.value + '%', backgroundColor: metric.color }"
                role="progressbar"
              ></div>
            </div>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Metric</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Value</label>
              <input v-model="form.value" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Unit</label>
              <input v-model="form.unit" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Color</label>
              <input v-model="form.color" type="color" class="form-control form-control-color" />
            </div>
            <div class="mb-3">
              <label class="form-label">Category</label>
              <select v-model="form.category" class="form-select">
                <option value="concise">Concise</option>
                <option value="concise-status">Concise Status</option>
                <option value="detailed">Detailed</option>
                <option value="summary">Summary</option>
              </select>
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
import { ref, computed, onMounted } from 'vue'
import { reportMetricsApi, type ReportMetric } from '../../services/api'

const viewMode = ref<string>('concise')
const allMetrics = ref<ReportMetric[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ReportMetric | null>(null)
const defaultForm = { name: '', value: '', unit: '', color: '#0d6efd', category: 'concise' }
const form = ref({ ...defaultForm })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ReportMetric) {
  editing.value = item
  form.value = { name: item.name, value: item.value, unit: item.unit, color: item.color, category: item.category }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await reportMetricsApi.update(editing.value.id, form.value)
    else await reportMetricsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await reportMetricsApi.remove(id)
  await loadData()
}

async function loadData() {
  allMetrics.value = await reportMetricsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const conciseMetrics = computed(() => allMetrics.value.filter(m => m.category === 'concise'))
const conciseStatus = computed(() => allMetrics.value.filter(m => m.category === 'concise-status'))
const detailedMetrics = computed(() => allMetrics.value.filter(m => m.category === 'detailed'))
const summaryMetrics = computed(() => allMetrics.value.filter(m => m.category === 'summary'))

const getStatusBadgeClass = (color: string): string => {
  const classes = 'badge'
  switch (color) {
    case 'success':
    case '#198754':
      return `${classes} bg-success`
    case 'warning':
    case '#ffc107':
      return `${classes} bg-warning text-dark`
    case 'info':
    case '#0dcaf0':
      return `${classes} bg-info`
    default:
      return `${classes} bg-secondary`
  }
}
</script>

<style scoped>
.mode-toggle {
  padding: 1rem;
}

.table-responsive {
  max-height: 500px;
  overflow-y: auto;
}
</style>
