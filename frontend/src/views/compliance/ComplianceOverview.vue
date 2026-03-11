<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { upsUnitsApi, powerPhasesApi, cabinetTemperaturesApi, alarmsApi, dcIncidentsApi, sensorsApi } from '../../services/api'
import { THRESHOLDS, GATE_COLOR, GATE_LABEL, gate, phaseDeviations, type GateResult } from '../../composables/useComplianceExport'

const router = useRouter()
const loading = ref(true)
const upsUnits = ref([])
const powerPhases = ref([])
const cabinetTemps = ref([])
const alarms = ref([])
const incidents = ref([])
const sensors = ref([])

onMounted(async () => {
  const results = await Promise.allSettled([
    upsUnitsApi.getAll(), powerPhasesApi.getAll(), cabinetTemperaturesApi.getAll(),
    alarmsApi.getAll(), dcIncidentsApi.getAll(), sensorsApi.getAll(),
  ])
  const get = (r: PromiseSettledResult<unknown[]>) => r.status === 'fulfilled' ? r.value : []
  upsUnits.value = get(results[0] as PromiseSettledResult<never[]>) as never
  powerPhases.value = get(results[1] as PromiseSettledResult<never[]>) as never
  cabinetTemps.value = get(results[2] as PromiseSettledResult<never[]>) as never
  alarms.value = get(results[3] as PromiseSettledResult<never[]>) as never
  incidents.value = get(results[4] as PromiseSettledResult<never[]>) as never
  sensors.value = get(results[5] as PromiseSettledResult<never[]>) as never
  loading.value = false
})

const upsGates = computed(() => (upsUnits.value as { loadPct: number }[]).map(u => gate(u.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical)))
const tempGates = computed(() => (cabinetTemps.value as { inlet: number }[]).map(c => gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical)))
const phaseGates = computed(() => phaseDeviations(powerPhases.value as never).map(p => p.gate))

function worstGate(gates: ReturnType<typeof gate>[]) {
  if (gates.includes('critical')) return 'critical'
  if (gates.includes('warning')) return 'warning'
  return 'ok'
}

const cards = computed(() => [
  {
    title: 'UPS Bus Load',
    subtitle: 'Tier III: <40% warn, <45% crit',
    route: '/compliance/ups-load',
    g: worstGate(upsGates.value),
    detail: `${(upsUnits.value as []).length} units · ${upsGates.value.filter(g => g !== 'ok').length} above threshold`,
  },
  {
    title: 'Server Inlet Temp',
    subtitle: 'ASHRAE A1: 25°C warn, 27°C crit',
    route: '/compliance/inlet-temp',
    g: worstGate(tempGates.value),
    detail: `${(cabinetTemps.value as []).length} cabinets · ${tempGates.value.filter(g => g !== 'ok').length} hotspot(s)`,
  },
  {
    title: 'Phase Balance',
    subtitle: '>15% dev warn, >25% crit',
    route: '/compliance/phase-balance',
    g: worstGate(phaseGates.value),
    detail: `${(powerPhases.value as []).length} phases · ${phaseGates.value.filter(g => g !== 'ok').length} unbalanced`,
  },
  {
    title: 'Sensor / IIoT Health',
    subtitle: 'Connectivity & heartbeat',
    route: '/compliance/sensor-health',
    g: ((sensors.value as { status: string }[]).some(s => s.status === 'offline' || s.status === 'error') ? 'warning' : 'ok') as GateResult,
    detail: `${(sensors.value as []).length} sensors · ${(sensors.value as { status: string }[]).filter(s => s.status === 'offline' || s.status === 'error').length} offline`,
  },
])

const overallGate = computed(() => worstGate(cards.value.map(c => c.g as GateResult)))

const reports = [
  { title: 'Weekly Health Report', subtitle: 'UPS · Temps · Phase · IIoT', route: '/compliance/weekly', cadence: 'Weekly' },
  { title: 'Monthly Efficiency Report', subtitle: 'PUE · Energy · Thermal', route: '/compliance/monthly', cadence: 'Monthly' },
  { title: 'Quarterly Capacity Report', subtitle: 'Concurrent Maintainability · Redundancy', route: '/compliance/quarterly', cadence: 'Quarterly' },
  { title: 'Annual SLA Report', subtitle: 'SLA · Compliance · Lifecycle', route: '/compliance/annual', cadence: 'Annual' },
]
</script>

<template>
  <div>
    <h4 class="mb-1">Active Compliance Engine</h4>
    <p class="text-muted small mb-4">Uptime Institute Tier III · TIA-942 · ISO 50001 · ISO/IEC 22237</p>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>

      <div class="alert mb-4" :class="`alert-${GATE_COLOR[overallGate]}`">
        <strong>Overall Compliance Status: {{ GATE_LABEL[overallGate] }}</strong>
        — {{ new Date().toISOString().slice(0, 10) }}
      </div>

      <h6 class="text-muted text-uppercase small fw-semibold mb-3 mt-2">Live KPI Checks</h6>
      <div class="row g-3 mb-4">
        <div v-for="card in cards" :key="card.title" class="col-sm-6 col-xl-3">
          <div class="card border-0 shadow-sm h-100" role="button" @click="router.push(card.route)" style="cursor:pointer">
            <div class="card-body">
              <div class="d-flex justify-content-between align-items-start mb-2">
                <div class="fw-semibold">{{ card.title }}</div>
                <span class="badge" :class="`bg-${GATE_COLOR[card.g as GateResult]}`">{{ GATE_LABEL[card.g as GateResult] }}</span>
              </div>
              <small class="text-muted d-block">{{ card.subtitle }}</small>
              <small class="text-secondary">{{ card.detail }}</small>
            </div>
          </div>
        </div>
      </div>

      <h6 class="text-muted text-uppercase small fw-semibold mb-3 mt-4">Reports</h6>
      <div class="row g-3">
        <div v-for="r in reports" :key="r.title" class="col-sm-6 col-xl-3">
          <div class="card border-0 shadow-sm h-100" role="button" @click="router.push(r.route)" style="cursor:pointer">
            <div class="card-body">
              <div class="d-flex justify-content-between align-items-start mb-2">
                <div class="fw-semibold">{{ r.title }}</div>
                <span class="badge bg-secondary">{{ r.cadence }}</span>
              </div>
              <small class="text-muted">{{ r.subtitle }}</small>
            </div>
          </div>
        </div>
      </div>

    </template>
  </div>
</template>
