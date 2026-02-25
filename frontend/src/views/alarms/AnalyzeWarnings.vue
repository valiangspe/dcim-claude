<template>
  <div class="container-fluid">
    <h1 class="mb-4">Warning Analysis & Patterns</h1>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>

    <!-- Analysis Summary Cards -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Total Warnings (7d)</h6>
            <p class="h4 mb-0">234</p>
            <small class="text-muted">Up 12% from previous week</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Critical Issues</h6>
            <p class="h4 mb-0">5</p>
            <small class="text-muted">Require immediate action</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Recurring Patterns</h6>
            <p class="h4 mb-0">8</p>
            <small class="text-muted">Patterns detected</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Avg Resolution Time</h6>
            <p class="h4 mb-0">2.3h</p>
            <small class="text-muted">Down 8% improvement</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Warning Patterns -->
    <div class="row mb-4">
      <div class="col-lg-8">
        <div class="card">
          <div class="card-header d-flex justify-content-between align-items-center">
            <h5 class="mb-0">Top Warning Patterns (7 days)</h5>
            <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Pattern</button>
          </div>
          <div class="table-responsive">
            <table class="table table-sm mb-0">
              <thead class="table-light">
                <tr>
                  <th>Pattern</th>
                  <th>Frequency</th>
                  <th>Last Occurrence</th>
                  <th>Trend</th>
                  <th style="width:120px">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="pattern in warningPatterns" :key="pattern.id">
                  <td>{{ pattern.name }}</td>
                  <td>
                    <span class="badge bg-light text-dark">{{ pattern.frequency }}</span>
                  </td>
                  <td class="text-nowrap">{{ formatTime(pattern.lastOccurrence) }}</td>
                  <td>
                    <span :class="getTrendBadgeClass(pattern.trend)">
                      {{ pattern.trend }}
                    </span>
                  </td>
                  <td>
                    <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(pattern)">Edit</button>
                    <button class="btn btn-sm btn-outline-danger" @click="remove(pattern.id)">Delete</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Warning Distribution</h5>
          </div>
          <div class="card-body">
            <div v-for="dist in warningDistribution" :key="dist.type" class="mb-3">
              <div class="d-flex justify-content-between mb-1">
                <small class="fw-500">{{ dist.type }}</small>
                <small class="text-muted">{{ dist.count }}</small>
              </div>
              <div class="progress" style="height: 20px">
                <div
                  :class="['progress-bar', getDistributionBarClass(dist.type)]"
                  :style="{ width: dist.percentage + '%' }"
                ></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Recommendations -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">System Recommendations</h5>
      </div>
      <div class="card-body">
        <div v-for="(recommendation, index) in recommendations" :key="index" class="mb-3">
          <div class="d-flex align-items-start">
            <div
              :class="['badge', getRecommendationBadgeClass(recommendation.priority), 'flex-shrink-0']"
              style="margin-top: 2px"
            >
              {{ recommendation.priority }}
            </div>
            <div class="ms-3 flex-grow-1">
              <p class="mb-1 fw-500">{{ recommendation.title }}</p>
              <p class="mb-0 text-muted">{{ recommendation.description }}</p>
              <small v-if="recommendation.impact" class="text-muted">
                Potential impact: {{ recommendation.impact }}
              </small>
            </div>
            <button class="btn btn-sm btn-outline-primary ms-2 flex-shrink-0">Details</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Warning Pattern</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Frequency</label>
              <input v-model="form.frequency" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Last Occurrence</label>
              <input v-model="form.lastOccurrence" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Trend</label>
              <select v-model="form.trend" class="form-select">
                <option value="Increasing">Increasing</option>
                <option value="Stable">Stable</option>
                <option value="Decreasing">Decreasing</option>
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
import { warningPatternsApi, type WarningPattern } from '../../services/api'

interface WarningDistribution {
  type: string
  count: number
  percentage: number
}

interface Recommendation {
  title: string
  description: string
  priority: 'Critical' | 'High' | 'Medium'
  impact?: string
}

const warningPatterns = ref<WarningPattern[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<WarningPattern | null>(null)
const defaultForm = { name: '', frequency: '', lastOccurrence: '', trend: 'Stable' }
const form = ref({ ...defaultForm })

async function loadData() {
  warningPatterns.value = await warningPatternsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(pattern: WarningPattern) {
  editing.value = pattern
  form.value = { name: pattern.name, frequency: pattern.frequency, lastOccurrence: pattern.lastOccurrence, trend: pattern.trend }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await warningPatternsApi.update(editing.value.id, form.value)
    else await warningPatternsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await warningPatternsApi.remove(id)
  await loadData()
}

const warningDistribution: WarningDistribution[] = [
  { type: 'Temperature', count: 85, percentage: 36 },
  { type: 'Power', count: 65, percentage: 28 },
  { type: 'Network', count: 45, percentage: 19 },
  { type: 'Cooling', count: 25, percentage: 11 },
  { type: 'Other', count: 14, percentage: 6 },
]

const recommendations: Recommendation[] = [
  {
    title: 'Upgrade Cooling Capacity in Rack A',
    description:
      'Temperature spikes have increased 35% this month. Current cooling capacity insufficient for current workload.',
    priority: 'Critical',
    impact: 'Potential equipment failure and data loss',
  },
  {
    title: 'Perform Power Distribution Audit',
    description:
      'PDU anomalies suggest unbalanced power loads across circuits. Recommend full audit and rebalancing.',
    priority: 'High',
    impact: 'Reduced equipment lifespan and increased failure risk',
  },
  {
    title: 'Schedule Network Port Upgrades',
    description:
      'Bandwidth utilization approaching 85% on core switches. Plan for upgrade within 6 months.',
    priority: 'Medium',
    impact: 'Potential service degradation during peak hours',
  },
  {
    title: 'Plan Preventive Cooling Maintenance',
    description:
      'Pressure warnings indicate potential refrigerant leak. Schedule preventive maintenance within 2 weeks.',
    priority: 'High',
    impact: 'System failure if leak develops',
  },
  {
    title: 'Replace UPS Battery Units',
    description:
      'Battery health degradation detected. Units approaching end of warranty period. Budget for replacement.',
    priority: 'Medium',
    impact: 'Reduced backup power capacity during outages',
  },
]

function formatTime(date: string | Date): string {
  const d = typeof date === 'string' ? new Date(date) : date
  const now = new Date()
  const diffMs = now.getTime() - d.getTime()
  const diffHours = Math.floor(diffMs / (60 * 60 * 1000))

  if (diffHours < 1) return 'Now'
  if (diffHours < 24) return `${diffHours}h ago`
  return d.toLocaleDateString()
}

function getTrendBadgeClass(trend: string): string {
  const classes: Record<string, string> = {
    Increasing: 'badge bg-danger',
    Stable: 'badge bg-info',
    Decreasing: 'badge bg-success',
  }
  return classes[trend] || 'badge bg-secondary'
}

function getDistributionBarClass(type: string): string {
  const classes: Record<string, string> = {
    Temperature: 'bg-danger',
    Power: 'bg-warning',
    Network: 'bg-info',
    Cooling: 'bg-primary',
    Other: 'bg-secondary',
  }
  return classes[type] || 'bg-secondary'
}

function getRecommendationBadgeClass(priority: string): string {
  const classes: Record<string, string> = {
    Critical: 'bg-danger',
    High: 'bg-warning text-dark',
    Medium: 'bg-info',
  }
  return classes[priority] || 'bg-secondary'
}
</script>
