<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { upsUnitsApi, type UpsUnit, powerPhasesApi, type PowerPhase, cabinetTemperaturesApi, type CabinetTemperature, alarmsApi, type Alarm, dcIncidentsApi, type DcIncident, sensorsApi, type Sensor } from '../../services/api'
import { THRESHOLDS, GATE_COLOR, GATE_LABEL, gate, phaseDeviations, downloadWeekly } from '../../composables/useComplianceExport'

const loading = ref(true)
const upsUnits = ref<UpsUnit[]>([])
const powerPhases = ref<PowerPhase[]>([])
const cabinetTemps = ref<CabinetTemperature[]>([])
const alarms = ref<Alarm[]>([])
const incidents = ref<DcIncident[]>([])
const sensors = ref<Sensor[]>([])

onMounted(async () => {
  const r = await Promise.allSettled([upsUnitsApi.getAll(), powerPhasesApi.getAll(), cabinetTemperaturesApi.getAll(), alarmsApi.getAll(), dcIncidentsApi.getAll(), sensorsApi.getAll()])
  const g = <T>(x: PromiseSettledResult<T[]>) => x.status === 'fulfilled' ? x.value : []
  upsUnits.value = g(r[0] as PromiseSettledResult<UpsUnit[]>)
  powerPhases.value = g(r[1] as PromiseSettledResult<PowerPhase[]>)
  cabinetTemps.value = g(r[2] as PromiseSettledResult<CabinetTemperature[]>)
  alarms.value = g(r[3] as PromiseSettledResult<Alarm[]>)
  incidents.value = g(r[4] as PromiseSettledResult<DcIncident[]>)
  sensors.value = g(r[5] as PromiseSettledResult<Sensor[]>)
  loading.value = false
})

const upsRows = computed(() => upsUnits.value.map(u => ({ ...u, g: gate(u.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical), tierSafe: u.loadPct < THRESHOLDS.upsLoad.warning })))
const tempRows = computed(() => cabinetTemps.value.map(c => ({ ...c, g: gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical) })).sort((a, b) => b.inlet - a.inlet))
const phaseRows = computed(() => phaseDeviations(powerPhases.value))

const alarmStats = computed(() => ({
  critical: alarms.value.filter(a => a.severity === 'critical').length,
  warning: alarms.value.filter(a => a.severity === 'warning' || a.severity === 'high').length,
  total: alarms.value.length,
  unacked: alarms.value.filter(a => !a.acknowledged).length,
}))
const incidentStats = computed(() => ({
  open: incidents.value.filter(i => ['open', 'investigating', 'in-progress'].includes(i.status)).length,
  critical: incidents.value.filter(i => i.severity === 'critical').length,
}))
const sensorStats = computed(() => ({
  total: sensors.value.length,
  active: sensors.value.filter(s => s.status === 'active' || s.status === 'online').length,
  offline: sensors.value.filter(s => s.status === 'offline' || s.status === 'error').length,
}))

const gates = computed(() => [
  { label: 'UPS Tier III Load', pass: upsRows.value.every(u => u.tierSafe), warn: upsRows.value.some(u => u.g === 'warning'), detail: `${upsRows.value.filter(u => !u.tierSafe).length} unit(s) above 40%` },
  { label: 'Inlet Temperature', pass: tempRows.value.every(t => t.g === 'ok'), warn: tempRows.value.some(t => t.g === 'warning') && !tempRows.value.some(t => t.g === 'critical'), detail: `Max: ${cabinetTemps.value.length ? Math.max(...cabinetTemps.value.map(t => t.inlet)) : '—'}°C` },
  { label: 'Phase Balance', pass: phaseRows.value.every(p => p.gate === 'ok'), warn: phaseRows.value.some(p => p.gate === 'warning') && !phaseRows.value.some(p => p.gate === 'critical'), detail: `${phaseRows.value.filter(p => p.gate !== 'ok').length} phase(s) unbalanced` },
  { label: 'Critical Alarms', pass: alarmStats.value.critical === 0, warn: false, detail: `${alarmStats.value.critical} critical` },
  { label: 'Open Incidents', pass: incidentStats.value.open === 0, warn: incidentStats.value.open > 0, detail: `${incidentStats.value.open} open` },
  { label: 'Sensor Health', pass: sensorStats.value.offline === 0, warn: sensorStats.value.offline > 0, detail: `${sensorStats.value.offline} offline` },
])

const overallOk = computed(() => gates.value.every(g => g.pass))
const overallWarn = computed(() => !overallOk.value && !gates.value.some(g => !g.pass && !g.warn))

function doDownload() { downloadWeekly(upsUnits.value, powerPhases.value, cabinetTemps.value, alarms.value, incidents.value, sensors.value) }
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-start mb-4 flex-wrap gap-3">
      <div>
        <h4 class="mb-1">Weekly Operational Health Report</h4>
        <p class="text-muted small mb-0">UPS bus load · Inlet temps · Phase balance · Alarms · IIoT connectivity</p>
      </div>
      <button class="btn btn-success" @click="doDownload">⬇ Download XLSX</button>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>

      <div class="alert mb-4" :class="overallOk ? 'alert-success' : overallWarn ? 'alert-warning' : 'alert-danger'">
        <strong>Overall: {{ overallOk ? 'PASS' : overallWarn ? 'WARNING' : 'CRITICAL' }}</strong>
        — {{ new Date().toISOString().slice(0, 10) }}
      </div>

      <!-- Gate summary -->
      <div class="row g-2 mb-4">
        <div v-for="g in gates" :key="g.label" class="col-sm-6 col-lg-4 col-xl-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body py-2">
              <div class="d-flex justify-content-between align-items-center mb-1">
                <small class="fw-semibold">{{ g.label }}</small>
                <span class="badge" :class="g.pass ? 'bg-success' : g.warn ? 'bg-warning text-dark' : 'bg-danger'">
                  {{ g.pass ? 'PASS' : g.warn ? 'WARN' : 'FAIL' }}
                </span>
              </div>
              <small class="text-muted">{{ g.detail }}</small>
            </div>
          </div>
        </div>
      </div>

      <!-- UPS -->
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>UPS Bus Load — Tier III Gate</span>
          <span class="badge" :class="upsRows.every(u => u.tierSafe) ? 'bg-success' : 'bg-danger'">{{ upsRows.every(u => u.tierSafe) ? 'PASS' : 'FAIL' }}</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>UPS</th><th>Load %</th><th>kW</th><th>Gate</th><th>Tier III</th><th>Mode</th></tr></thead>
            <tbody>
              <tr v-for="u in upsRows" :key="u.id">
                <td class="fw-semibold">{{ u.name }}</td>
                <td style="min-width:160px">
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-grow-1" style="height:8px"><div class="progress-bar" :class="`bg-${GATE_COLOR[u.g]}`" :style="`width:${u.loadPct}%`"></div></div>
                    <span :class="`text-${GATE_COLOR[u.g]}`" class="fw-semibold">{{ u.loadPct }}%</span>
                  </div>
                </td>
                <td class="text-muted">{{ u.load }}/{{ u.capacity }}</td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[u.g]}`">{{ GATE_LABEL[u.g] }}</span></td>
                <td><span class="badge" :class="u.tierSafe ? 'bg-success' : 'bg-danger'">{{ u.tierSafe ? 'Safe' : 'Act' }}</span></td>
                <td class="text-muted">{{ u.mode }}</td>
              </tr>
              <tr v-if="!upsRows.length"><td colspan="6" class="text-center text-muted py-3">No data</td></tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Temps -->
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Server Inlet Temperature — 25°C / 27°C</span>
          <span class="badge" :class="tempRows.some(t => t.g === 'critical') ? 'bg-danger' : tempRows.some(t => t.g === 'warning') ? 'bg-warning text-dark' : 'bg-success'">{{ tempRows.some(t => t.g === 'critical') ? 'CRITICAL' : tempRows.some(t => t.g === 'warning') ? 'WARNING' : 'PASS' }}</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Cabinet</th><th>Location</th><th>Inlet °C</th><th>Outlet °C</th><th>Delta</th><th>Gate</th></tr></thead>
            <tbody>
              <tr v-for="c in tempRows" :key="c.id" :class="c.g === 'critical' ? 'table-danger' : c.g === 'warning' ? 'table-warning' : ''">
                <td class="fw-semibold">{{ c.cabinetId }}</td><td class="text-muted">{{ c.location }}</td>
                <td><span class="fw-semibold" :class="`text-${GATE_COLOR[c.g]}`">{{ c.inlet }}°C</span></td>
                <td>{{ c.outlet }}°C</td><td class="text-muted">{{ c.delta }}°C</td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[c.g]}`">{{ GATE_LABEL[c.g] }}</span></td>
              </tr>
              <tr v-if="!tempRows.length"><td colspan="6" class="text-center text-muted py-3">No data</td></tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Phases -->
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Phase Balance — 15% / 25% Deviation</span>
          <span class="badge" :class="phaseRows.some(p => p.gate === 'critical') ? 'bg-danger' : phaseRows.some(p => p.gate === 'warning') ? 'bg-warning text-dark' : 'bg-success'">{{ phaseRows.some(p => p.gate === 'critical') ? 'CRITICAL' : phaseRows.some(p => p.gate === 'warning') ? 'WARNING' : 'PASS' }}</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Phase</th><th>Voltage (V)</th><th>Current (A)</th><th>Freq (Hz)</th><th>PF</th><th>Deviation</th><th>Gate</th></tr></thead>
            <tbody>
              <tr v-for="p in phaseRows" :key="p.id">
                <td class="fw-semibold">{{ p.name }}</td><td>{{ p.voltage }}</td><td>{{ p.current }}</td><td>{{ p.frequency }}</td><td>{{ p.pf }}</td>
                <td><span :class="`text-${GATE_COLOR[p.gate]}`" class="fw-semibold">{{ p.deviation }}%</span></td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[p.gate]}`">{{ GATE_LABEL[p.gate] }}</span></td>
              </tr>
              <tr v-if="!phaseRows.length"><td colspan="7" class="text-center text-muted py-3">No data</td></tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Alarms + IIoT -->
      <div class="row g-3">
        <div class="col-md-4">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Active Alarms</div>
            <div class="card-body row g-2 text-center">
              <div class="col-6"><h3 class="text-danger">{{ alarmStats.critical }}</h3><small class="text-muted">Critical</small></div>
              <div class="col-6"><h3 class="text-warning">{{ alarmStats.warning }}</h3><small class="text-muted">Warning</small></div>
              <div class="col-6"><h3>{{ alarmStats.total }}</h3><small class="text-muted">Total</small></div>
              <div class="col-6"><h3 class="text-info">{{ alarmStats.unacked }}</h3><small class="text-muted">Unacked</small></div>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Open Incidents</div>
            <div class="card-body text-center">
              <h2 :class="incidentStats.open > 0 ? 'text-danger' : 'text-success'">{{ incidentStats.open }}</h2>
              <small class="text-muted d-block">Open / Active</small>
              <small class="text-secondary">{{ incidentStats.critical }} critical severity</small>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Sensor / IIoT Health</div>
            <div class="card-body text-center">
              <h2 :class="sensorStats.offline > 0 ? 'text-warning' : 'text-success'">{{ sensorStats.active }}/{{ sensorStats.total }}</h2>
              <small class="text-muted d-block">Active / Total</small>
              <small :class="sensorStats.offline > 0 ? 'text-danger' : 'text-success'">{{ sensorStats.offline }} offline</small>
            </div>
          </div>
        </div>
      </div>

    </template>
  </div>
</template>
