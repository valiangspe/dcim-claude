<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { uptimeRecordsApi, type UptimeRecord, dcIncidentsApi, type DcIncident, complianceChecklistsApi, type ComplianceChecklist, lifecyclePhasesApi, type LifecyclePhase } from '../../services/api'
import { fmtMins, downloadAnnual } from '../../composables/useComplianceExport'

const loading = ref(true)
const uptimeRecords = ref<UptimeRecord[]>([])
const incidents = ref<DcIncident[]>([])
const checklists = ref<ComplianceChecklist[]>([])
const lifecycle = ref<LifecyclePhase[]>([])

onMounted(async () => {
  const r = await Promise.allSettled([uptimeRecordsApi.getAll(), dcIncidentsApi.getAll(), complianceChecklistsApi.getAll(), lifecyclePhasesApi.getAll()])
  const g = <T>(x: PromiseSettledResult<T[]>) => x.status === 'fulfilled' ? x.value : []
  uptimeRecords.value = g(r[0] as PromiseSettledResult<UptimeRecord[]>)
  incidents.value = g(r[1] as PromiseSettledResult<DcIncident[]>)
  checklists.value = g(r[2] as PromiseSettledResult<ComplianceChecklist[]>)
  lifecycle.value = g(r[3] as PromiseSettledResult<LifecyclePhase[]>)
  loading.value = false
})

const uptimeRows = computed(() => uptimeRecords.value.map(u => {
  const totalMins = u.uptimeMinutes + u.downtimeMinutes
  const pct = totalMins > 0 ? parseFloat(((u.uptimeMinutes / totalMins) * 100).toFixed(4)) : 100
  const slaOk = pct >= u.slaTarget
  return { ...u, availPct: pct, slaOk }
}))

const overallAvail = computed(() => {
  const totalUp = uptimeRecords.value.reduce((s, r) => s + r.uptimeMinutes, 0)
  const totalAll = uptimeRecords.value.reduce((s, r) => s + r.uptimeMinutes + r.downtimeMinutes, 0)
  return totalAll > 0 ? parseFloat(((totalUp / totalAll) * 100).toFixed(4)) : null
})

const incidentStats = computed(() => ({
  total: incidents.value.length,
  critical: incidents.value.filter(i => i.severity === 'critical').length,
  resolved: incidents.value.filter(i => i.status === 'resolved' || i.status === 'closed').length,
  totalDowntime: incidents.value.reduce((s, i) => s + (i.downtimeMinutes || 0), 0),
  mttr: incidents.value.filter(i => i.downtimeMinutes > 0).length
    ? Math.round(incidents.value.filter(i => i.downtimeMinutes > 0).reduce((s, i) => s + i.downtimeMinutes, 0) / incidents.value.filter(i => i.downtimeMinutes > 0).length)
    : 0,
}))

const checklistStats = computed(() => ({
  total: checklists.value.length,
  compliant: checklists.value.filter(c => c.status === 'compliant').length,
  nonCompliant: checklists.value.filter(c => c.status === 'non-compliant').length,
  inProgress: checklists.value.filter(c => c.status === 'in-progress').length,
  pct: checklists.value.length ? Math.round((checklists.value.filter(c => c.status === 'compliant').length / checklists.value.length) * 100) : 0,
}))

const lifecycleStats = computed(() => {
  const total = lifecycle.value.reduce((s, l) => s + l.count, 0)
  const eolPhase = lifecycle.value.find(l => l.phase.toLowerCase().includes('eol') || l.phase.toLowerCase().includes('end'))
  return { total, phases: lifecycle.value.length, eolCount: eolPhase?.count ?? 0 }
})

const kpis = computed(() => [
  { label: 'Overall Availability', value: overallAvail.value != null ? overallAvail.value + '%' : '—', color: overallAvail.value != null && overallAvail.value >= 99.9 ? 'success' : overallAvail.value != null && overallAvail.value >= 99 ? 'warning' : 'danger', sub: 'All systems YTD' },
  { label: 'SLA Pass Rate', value: uptimeRows.value.length ? Math.round((uptimeRows.value.filter(u => u.slaOk).length / uptimeRows.value.length) * 100) + '%' : '—', color: uptimeRows.value.every(u => u.slaOk) ? 'success' : 'warning', sub: `${uptimeRows.value.filter(u => u.slaOk).length}/${uptimeRows.value.length} systems` },
  { label: 'Total Incidents', value: String(incidentStats.value.total), color: incidentStats.value.critical > 0 ? 'danger' : 'success', sub: `${incidentStats.value.critical} critical` },
  { label: 'MTTR', value: fmtMins(incidentStats.value.mttr), color: incidentStats.value.mttr > 60 ? 'danger' : incidentStats.value.mttr > 30 ? 'warning' : 'success', sub: 'Mean time to repair' },
  { label: 'Compliance Score', value: checklistStats.value.pct + '%', color: checklistStats.value.pct >= 90 ? 'success' : checklistStats.value.pct >= 70 ? 'warning' : 'danger', sub: `${checklistStats.value.compliant}/${checklistStats.value.total} compliant` },
  { label: 'EOL Assets', value: String(lifecycleStats.value.eolCount), color: lifecycleStats.value.eolCount > 0 ? 'danger' : 'success', sub: `${lifecycleStats.value.total} assets tracked` },
])

function doDownload() { downloadAnnual(uptimeRecords.value, incidents.value, checklists.value, lifecycle.value) }
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-start mb-4 flex-wrap gap-3">
      <div>
        <h4 class="mb-1">Annual SLA &amp; Compliance Review</h4>
        <p class="text-muted small mb-0">Availability SLA · Incident summary · ISO 22237 compliance · Asset lifecycle — Uptime Institute / ISO 22237</p>
      </div>
      <button class="btn btn-success" @click="doDownload">⬇ Download XLSX</button>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>

      <div class="row g-3 mb-4">
        <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <h3 :class="`text-${kpi.color} mb-1`">{{ kpi.value }}</h3>
              <small class="text-muted d-block">{{ kpi.label }}</small>
              <small class="text-secondary">{{ kpi.sub }}</small>
            </div>
          </div>
        </div>
      </div>

      <!-- Uptime / SLA -->
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Availability &amp; SLA Validation</span>
          <span class="badge" :class="uptimeRows.every(u => u.slaOk) ? 'bg-success' : 'bg-warning text-dark'">{{ uptimeRows.every(u => u.slaOk) ? 'All SLAs Met' : 'SLA Breach' }}</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Site / System</th><th>Period</th><th>Availability %</th><th>SLA Target</th><th>Downtime</th><th>Incidents</th><th>Status</th></tr></thead>
            <tbody>
              <tr v-for="u in uptimeRows" :key="u.id" :class="!u.slaOk ? 'table-warning' : ''">
                <td class="fw-semibold">{{ u.site }} — {{ u.system }}</td>
                <td class="text-muted">{{ u.periodStart }} – {{ u.periodEnd }}</td>
                <td><span class="fw-semibold" :class="u.slaOk ? 'text-success' : 'text-danger'">{{ u.availPct }}%</span></td>
                <td class="text-muted">{{ u.slaTarget }}%</td>
                <td>{{ fmtMins(u.downtimeMinutes) }}</td>
                <td>{{ u.incidents }}</td>
                <td><span class="badge" :class="u.slaOk ? 'bg-success' : 'bg-warning text-dark'">{{ u.slaOk ? 'Met' : 'Breach' }}</span></td>
              </tr>
              <tr v-if="!uptimeRows.length"><td colspan="7" class="text-center text-muted py-3">No uptime data</td></tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Incidents -->
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Annual Incident Summary</span>
          <span class="badge" :class="incidentStats.critical > 0 ? 'bg-danger' : 'bg-success'">{{ incidentStats.total }} Total · {{ incidentStats.critical }} Critical</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Incident</th><th>Category</th><th>Severity</th><th>System</th><th>Downtime</th><th>Root Cause</th><th>Status</th></tr></thead>
            <tbody>
              <tr v-for="i in incidents" :key="i.id">
                <td class="fw-semibold">{{ i.title }}</td>
                <td class="text-muted">{{ i.category }}</td>
                <td><span class="badge" :class="i.severity === 'critical' ? 'bg-danger' : i.severity === 'high' ? 'bg-warning text-dark' : 'bg-secondary'">{{ i.severity }}</span></td>
                <td class="text-muted">{{ i.affectedSystem }}</td>
                <td>{{ fmtMins(i.downtimeMinutes) }}</td>
                <td class="text-muted small">{{ i.rootCause || '—' }}</td>
                <td><span class="badge" :class="(i.status === 'resolved' || i.status === 'closed') ? 'bg-success' : 'bg-warning text-dark'">{{ i.status }}</span></td>
              </tr>
              <tr v-if="!incidents.length"><td colspan="7" class="text-center text-muted py-3">No incidents recorded</td></tr>
            </tbody>
          </table>
        </div>
        <div class="card-footer text-muted small">Total downtime: {{ fmtMins(incidentStats.totalDowntime) }} · MTTR: {{ fmtMins(incidentStats.mttr) }}</div>
      </div>

      <!-- Compliance checklists -->
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>ISO 22237 Compliance Requirements</span>
          <span class="badge" :class="checklistStats.pct >= 90 ? 'bg-success' : checklistStats.pct >= 70 ? 'bg-warning text-dark' : 'bg-danger'">{{ checklistStats.pct }}% Compliant</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Category</th><th>Requirement</th><th>ISO Ref</th><th>Assigned To</th><th>Due Date</th><th>Status</th></tr></thead>
            <tbody>
              <tr v-for="c in checklists" :key="c.id">
                <td><span class="badge bg-light text-dark border">{{ c.category }}</span></td>
                <td class="fw-semibold">{{ c.requirement }}</td>
                <td class="font-monospace text-muted small">{{ c.isoReference }}</td>
                <td class="text-muted">{{ c.assignedTo || '—' }}</td>
                <td class="text-muted">{{ c.dueDate || '—' }}</td>
                <td>
                  <span class="badge" :class="c.status === 'compliant' ? 'bg-success' : c.status === 'non-compliant' ? 'bg-danger' : c.status === 'in-progress' ? 'bg-warning text-dark' : 'bg-secondary'">{{ c.status }}</span>
                </td>
              </tr>
              <tr v-if="!checklists.length"><td colspan="6" class="text-center text-muted py-3">No checklist data</td></tr>
            </tbody>
          </table>
        </div>
        <div class="card-footer text-muted small">{{ checklistStats.compliant }} compliant · {{ checklistStats.nonCompliant }} non-compliant · {{ checklistStats.inProgress }} in-progress</div>
      </div>

      <!-- Lifecycle -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Asset Lifecycle Status</span>
          <span class="badge" :class="lifecycleStats.eolCount > 0 ? 'bg-danger' : 'bg-success'">{{ lifecycleStats.total }} assets · {{ lifecycleStats.eolCount }} EOL</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Lifecycle Phase</th><th>Asset Count</th><th>Items</th><th>ETA / Notes</th></tr></thead>
            <tbody>
              <tr v-for="l in lifecycle" :key="l.id" :class="l.phase.toLowerCase().includes('eol') || l.phase.toLowerCase().includes('end') ? 'table-danger' : ''">
                <td class="fw-semibold">{{ l.phase }}</td>
                <td><span class="badge bg-primary">{{ l.count }}</span></td>
                <td class="text-muted small">{{ l.items || '—' }}</td>
                <td class="text-muted">{{ l.eta || '—' }}</td>
              </tr>
              <tr v-if="!lifecycle.length"><td colspan="4" class="text-center text-muted py-3">No lifecycle data</td></tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>
  </div>
</template>
