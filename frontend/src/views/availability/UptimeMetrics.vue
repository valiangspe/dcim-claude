<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { uptimeRecordsApi, type UptimeRecord } from '../../services/api'

const records = ref<UptimeRecord[]>([])
const loading = ref(true)
const error = ref('')
const showModal = ref(false)
const editing = ref<UptimeRecord | null>(null)
const saving = ref(false)
const form = ref({ site: '', system: '', uptimeMinutes: 0, downtimeMinutes: 0, periodStart: '', periodEnd: '', slaTarget: 99.9, incidents: 0, notes: '' })

const kpis = computed(() => {
  if (!records.value.length) return [
    { label: 'Overall Availability', value: '—', color: 'primary', trend: 'No data' },
    { label: 'Total Downtime', value: '—', color: 'danger', trend: 'No data' },
    { label: 'SLA Compliance', value: '—', color: 'success', trend: 'No data' },
    { label: 'Systems Monitored', value: '0', color: 'info', trend: 'No records' },
    { label: 'SLA Breaches', value: '0', color: 'warning', trend: 'No data' },
    { label: 'MTTR (avg)', value: '—', color: 'secondary', trend: 'No data' },
  ]
  const totalUp = records.value.reduce((s, r) => s + r.uptimeMinutes, 0)
  const totalDown = records.value.reduce((s, r) => s + r.downtimeMinutes, 0)
  const total = totalUp + totalDown
  const availability = total > 0 ? ((totalUp / total) * 100).toFixed(3) : '—'
  const downtimeHrs = (totalDown / 60).toFixed(1)
  const slaBreach = records.value.filter(r => {
    const t = r.uptimeMinutes + r.downtimeMinutes
    return t > 0 && ((r.uptimeMinutes / t) * 100) < r.slaTarget
  }).length
  const mttr = records.value.filter(r => r.incidents > 0)
    .reduce((s, r) => s + r.downtimeMinutes / r.incidents, 0)
  const mttrAvg = records.value.some(r => r.incidents > 0) ? (mttr / records.value.filter(r => r.incidents > 0).length).toFixed(0) + ' min' : '—'
  return [
    { label: 'Overall Availability', value: `${availability}%`, color: 'primary', trend: 'All systems combined' },
    { label: 'Total Downtime', value: `${downtimeHrs}h`, color: 'danger', trend: 'Across all periods' },
    { label: 'SLA Breaches', value: String(slaBreach), color: slaBreach > 0 ? 'danger' : 'success', trend: slaBreach > 0 ? 'Review required' : 'All within SLA' },
    { label: 'Systems Monitored', value: String(records.value.length), color: 'info', trend: 'Active records' },
    { label: 'Total Incidents', value: String(records.value.reduce((s, r) => s + r.incidents, 0)), color: 'warning', trend: 'All periods' },
    { label: 'Avg MTTR', value: mttrAvg, color: 'secondary', trend: 'Mean time to recover' },
  ]
})

function availabilityPct(r: UptimeRecord) {
  const total = r.uptimeMinutes + r.downtimeMinutes
  if (!total) return null
  return ((r.uptimeMinutes / total) * 100).toFixed(3)
}

function slaColor(r: UptimeRecord) {
  const pct = availabilityPct(r)
  if (!pct) return 'secondary'
  return parseFloat(pct) >= r.slaTarget ? 'success' : 'danger'
}

async function loadData() {
  records.value = await uptimeRecordsApi.getAll()
}

onMounted(async () => {
  try { await loadData() }
  catch (e: unknown) { error.value = e instanceof Error ? e.message : 'Failed to load uptime records' }
  finally { loading.value = false }
})

function openCreate() {
  editing.value = null
  form.value = { site: '', system: '', uptimeMinutes: 0, downtimeMinutes: 0, periodStart: '', periodEnd: '', slaTarget: 99.9, incidents: 0, notes: '' }
  showModal.value = true
}

function openEdit(r: UptimeRecord) {
  editing.value = r
  form.value = { site: r.site, system: r.system, uptimeMinutes: r.uptimeMinutes, downtimeMinutes: r.downtimeMinutes, periodStart: r.periodStart, periodEnd: r.periodEnd, slaTarget: r.slaTarget, incidents: r.incidents, notes: r.notes }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await uptimeRecordsApi.update(editing.value.id, form.value)
    else await uptimeRecordsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(id: number) {
  if (!confirm('Delete this record?')) return
  await uptimeRecordsApi.remove(id)
  await loadData()
}

function formatMinutes(mins: number) {
  if (mins < 60) return `${mins}m`
  const h = Math.floor(mins / 60)
  const m = mins % 60
  return m > 0 ? `${h}h ${m}m` : `${h}h`
}
</script>

<template>
  <div>
    <h4 class="mb-1">Uptime &amp; Availability Metrics</h4>
    <p class="text-muted small mb-4">Track system availability, SLA compliance, and mean time to recover across all data centre infrastructure</p>

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

      <!-- Uptime Table -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
          <span>Availability Records</span>
          <button class="btn btn-sm btn-primary" @click="openCreate">Add Record</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Site</th>
                <th>System</th>
                <th>Period</th>
                <th>Uptime</th>
                <th>Downtime</th>
                <th>Availability</th>
                <th>SLA Target</th>
                <th>SLA</th>
                <th>Incidents</th>
                <th style="width:80px">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in records" :key="r.id">
                <td class="fw-semibold">{{ r.site }}</td>
                <td>{{ r.system }}</td>
                <td class="text-muted small">{{ r.periodStart }} – {{ r.periodEnd }}</td>
                <td class="text-success">{{ formatMinutes(r.uptimeMinutes) }}</td>
                <td :class="r.downtimeMinutes > 0 ? 'text-danger' : 'text-muted'">{{ formatMinutes(r.downtimeMinutes) }}</td>
                <td>
                  <span class="fw-semibold" :class="`text-${slaColor(r)}`">
                    {{ availabilityPct(r) ? availabilityPct(r) + '%' : '—' }}
                  </span>
                  <div class="progress mt-1" style="height:4px;width:80px">
                    <div class="progress-bar" :class="`bg-${slaColor(r)}`"
                      :style="`width:${Math.min(parseFloat(availabilityPct(r) ?? '0'), 100)}%`"></div>
                  </div>
                </td>
                <td class="text-muted">{{ r.slaTarget }}%</td>
                <td><span class="badge" :class="`bg-${slaColor(r)}`">{{ slaColor(r) === 'success' ? 'Met' : 'Breached' }}</span></td>
                <td>{{ r.incidents }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(r)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(r.id)">Delete</button>
                </td>
              </tr>
              <tr v-if="records.length === 0">
                <td colspan="10" class="text-center text-muted py-4">No uptime records yet. Add a record to begin availability tracking.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Availability Record</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="row g-3">
              <div class="col-6">
                <label class="form-label">Site</label>
                <input v-model="form.site" type="text" class="form-control" />
              </div>
              <div class="col-6">
                <label class="form-label">System</label>
                <input v-model="form.system" type="text" class="form-control" />
              </div>
              <div class="col-6">
                <label class="form-label">Period Start</label>
                <input v-model="form.periodStart" type="date" class="form-control" />
              </div>
              <div class="col-6">
                <label class="form-label">Period End</label>
                <input v-model="form.periodEnd" type="date" class="form-control" />
              </div>
              <div class="col-6">
                <label class="form-label">Uptime (minutes)</label>
                <input v-model.number="form.uptimeMinutes" type="number" class="form-control" min="0" />
              </div>
              <div class="col-6">
                <label class="form-label">Downtime (minutes)</label>
                <input v-model.number="form.downtimeMinutes" type="number" class="form-control" min="0" />
              </div>
              <div class="col-6">
                <label class="form-label">SLA Target (%)</label>
                <input v-model.number="form.slaTarget" type="number" class="form-control" step="0.001" min="0" max="100" />
              </div>
              <div class="col-6">
                <label class="form-label">Incidents</label>
                <input v-model.number="form.incidents" type="number" class="form-control" min="0" />
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
