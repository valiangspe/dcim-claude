<template>
  <div class="pue-trends">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-info" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-header bg-info text-white d-flex align-items-center justify-content-between">
        <h5 class="mb-0">PUE Trends - Monthly Analysis</h5>
        <button class="btn btn-sm btn-light" @click="openCreate">+ Add</button>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover">
            <thead>
              <tr class="table-light">
                <th>Month</th>
                <th>PUE Value</th>
                <th>Trend</th>
                <th>Progress</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="trend in pueTrends" :key="trend.id">
                <td><strong>{{ trend.month }}</strong></td>
                <td>{{ trend.pueValue }}</td>
                <td>
                  <span :class="getTrendBadgeClass(trend.trend)">
                    {{ trend.trend }}
                  </span>
                </td>
                <td>
                  <div class="progress" style="height: 20px">
                    <div
                      class="progress-bar"
                      :style="{ width: trend.progressPercent + '%', backgroundColor: getProgressColor(trend.pueValue) }"
                      role="progressbar"
                    >
                      <small class="text-white">{{ trend.progressPercent }}%</small>
                    </div>
                  </div>
                </td>
                <td>
                  <span :class="getStatusBadgeClass(trend.status)">
                    {{ trend.status }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(trend)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(trend.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Target PUE: 1.5 | Current: {{ pueTrends.length > 0 ? pueTrends[0].pueValue : 'N/A' }}
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} PUE Trend</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Month</label>
              <input v-model="form.month" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">PUE Value</label>
              <input v-model.number="form.pueValue" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Trend</label>
              <select v-model="form.trend" class="form-select">
                <option value="Down">Down</option>
                <option value="Up">Up</option>
                <option value="Stable">Stable</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Progress Percent</label>
              <input v-model.number="form.progressPercent" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="On Track">On Track</option>
                <option value="Good">Good</option>
                <option value="Fair">Fair</option>
                <option value="Poor">Poor</option>
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
import { ref, onMounted } from 'vue'
import { pueTrendsApi, type PueTrend } from '../../services/api'

const pueTrends = ref<PueTrend[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PueTrend | null>(null)
const defaultForm = { month: '', pueValue: 1.5, trend: 'Stable', progressPercent: 0, status: 'On Track' }
const form = ref({ ...defaultForm })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: PueTrend) {
  editing.value = item
  form.value = { month: item.month, pueValue: item.pueValue, trend: item.trend, progressPercent: item.progressPercent, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await pueTrendsApi.update(editing.value.id, form.value)
    else await pueTrendsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await pueTrendsApi.remove(id)
  await loadData()
}

async function loadData() {
  pueTrends.value = await pueTrendsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const getTrendBadgeClass = (trend: string): string => {
  const classes = 'badge'
  if (trend === 'Down') return `${classes} bg-success`
  if (trend === 'Up') return `${classes} bg-danger`
  return `${classes} bg-warning text-dark`
}

const getStatusBadgeClass = (status: string): string => {
  const classes = 'badge'
  switch (status) {
    case 'On Track':
      return `${classes} bg-success`
    case 'Good':
      return `${classes} bg-info`
    case 'Fair':
      return `${classes} bg-warning text-dark`
    case 'Poor':
      return `${classes} bg-danger`
    default:
      return `${classes} bg-secondary`
  }
}

const getProgressColor = (pueValue: number): string => {
  if (pueValue <= 1.5) return '#198754'
  if (pueValue <= 1.6) return '#0dcaf0'
  if (pueValue <= 1.75) return '#ffc107'
  return '#dc3545'
}
</script>

<style scoped>
.pue-trends {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
