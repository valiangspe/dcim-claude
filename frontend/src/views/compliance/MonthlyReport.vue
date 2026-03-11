<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { pueReadingsApi, type PueReading, energyCostRecordsApi, type EnergyCostRecord, coolingMetricsApi, type CoolingMetric, cabinetTemperaturesApi, type CabinetTemperature } from '../../services/api'
import { THRESHOLDS, GATE_COLOR, GATE_LABEL, gate, downloadMonthly } from '../../composables/useComplianceExport'

const loading = ref(true)
const pueReadings = ref<PueReading[]>([])
const energyCosts = ref<EnergyCostRecord[]>([])
const coolingMetrics = ref<CoolingMetric[]>([])
const cabinetTemps = ref<CabinetTemperature[]>([])

onMounted(async () => {
  const r = await Promise.allSettled([pueReadingsApi.getAll(), energyCostRecordsApi.getAll(), coolingMetricsApi.getAll(), cabinetTemperaturesApi.getAll()])
  const g = <T>(x: PromiseSettledResult<T[]>) => x.status === 'fulfilled' ? x.value : []
  pueReadings.value = g(r[0] as PromiseSettledResult<PueReading[]>)
  energyCosts.value = g(r[1] as PromiseSettledResult<EnergyCostRecord[]>)
  coolingMetrics.value = g(r[2] as PromiseSettledResult<CoolingMetric[]>)
  cabinetTemps.value = g(r[3] as PromiseSettledResult<CabinetTemperature[]>)
  loading.value = false
})

const avgPue = computed(() => pueReadings.value.length ? parseFloat((pueReadings.value.reduce((s, r) => s + r.pue, 0) / pueReadings.value.length).toFixed(3)) : null)
const pueGate = computed(() => avgPue.value != null ? gate(avgPue.value, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical) : 'ok')
const totalKwh = computed(() => energyCosts.value.reduce((s, r) => s + r.kwh, 0))
const totalCost = computed(() => energyCosts.value.reduce((s, r) => s + r.cost, 0))
const maxInlet = computed(() => cabinetTemps.value.length ? Math.max(...cabinetTemps.value.map(c => c.inlet)) : null)
const avgInlet = computed(() => cabinetTemps.value.length ? (cabinetTemps.value.reduce((s, c) => s + c.inlet, 0) / cabinetTemps.value.length).toFixed(1) : null)

const kpis = computed(() => [
  { label: 'Avg PUE', value: avgPue.value?.toString() ?? '—', color: GATE_COLOR[pueGate.value], sub: `Gate: ${GATE_LABEL[pueGate.value]}` },
  { label: 'Total kWh', value: totalKwh.value.toLocaleString(), color: 'primary', sub: 'Energy consumed' },
  { label: 'Total Cost', value: '$' + totalCost.value.toLocaleString(), color: 'info', sub: 'All periods' },
  { label: 'Max Inlet Temp', value: maxInlet.value != null ? maxInlet.value + '°C' : '—', color: maxInlet.value != null ? GATE_COLOR[gate(maxInlet.value, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical)] : 'secondary', sub: 'Peak this period' },
  { label: 'Avg Inlet Temp', value: avgInlet.value != null ? avgInlet.value + '°C' : '—', color: 'secondary', sub: 'Across all cabinets' },
  { label: 'Hotspots ≥25°C', value: String(cabinetTemps.value.filter(c => c.inlet >= 25).length), color: cabinetTemps.value.some(c => c.inlet >= 25) ? 'warning' : 'success', sub: 'Cabinets at risk' },
])

function doDownload() { downloadMonthly(pueReadings.value, energyCosts.value, coolingMetrics.value, cabinetTemps.value) }
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-start mb-4 flex-wrap gap-3">
      <div>
        <h4 class="mb-1">Monthly Efficiency &amp; Thermal Report</h4>
        <p class="text-muted small mb-0">PUE trends · Energy cost · Thermal profile · Cooling metrics — ISO 50001</p>
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

      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>PUE Readings — ISO 50001 (Warn &gt;1.5, Critical &gt;1.8)</span>
          <span class="badge" :class="`bg-${GATE_COLOR[pueGate]}`">Avg {{ avgPue ?? '—' }} — {{ GATE_LABEL[pueGate] }}</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Date</th><th>PUE</th><th>Component</th><th>Power (kW)</th><th>Share %</th><th>Gate</th></tr></thead>
            <tbody>
              <tr v-for="r in pueReadings" :key="r.id">
                <td>{{ r.date }}</td>
                <td class="fw-semibold" :class="`text-${GATE_COLOR[gate(r.pue, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical)]}`">{{ r.pue }}</td>
                <td>{{ r.component }}</td><td>{{ r.power }}</td><td>{{ r.pct }}%</td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[gate(r.pue, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical)]}`">{{ GATE_LABEL[gate(r.pue, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical)] }}</span></td>
              </tr>
              <tr v-if="!pueReadings.length"><td colspan="6" class="text-center text-muted py-3">No PUE data</td></tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="row g-3 mb-4">
        <div class="col-lg-6">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Energy Cost by Month</div>
            <div class="card-body p-0">
              <table class="table table-hover align-middle mb-0 small">
                <thead class="table-light"><tr><th>Month</th><th>kWh</th><th>Cost ($)</th><th>Rate</th></tr></thead>
                <tbody>
                  <tr v-for="r in energyCosts" :key="r.id">
                    <td class="fw-semibold">{{ r.month }}</td><td>{{ r.kwh.toLocaleString() }}</td>
                    <td>${{ r.cost.toLocaleString() }}</td><td class="text-muted">${{ r.rate }}</td>
                  </tr>
                  <tr v-if="!energyCosts.length"><td colspan="4" class="text-center text-muted py-3">No data</td></tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
        <div class="col-lg-6">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Cooling Metrics</div>
            <div class="card-body p-0">
              <table class="table table-hover align-middle mb-0 small">
                <thead class="table-light"><tr><th>Metric</th><th>Value</th><th>Unit</th></tr></thead>
                <tbody>
                  <tr v-for="m in coolingMetrics" :key="m.id">
                    <td>{{ m.name }}</td><td class="fw-semibold">{{ m.value }}</td><td class="text-muted">{{ m.unit }}</td>
                  </tr>
                  <tr v-if="!coolingMetrics.length"><td colspan="3" class="text-center text-muted py-3">No data</td></tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">Thermal Profile — Cabinet Inlet Temps</div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Cabinet</th><th>Location</th><th>Inlet °C</th><th>Outlet °C</th><th>Delta</th><th>Gate</th></tr></thead>
            <tbody>
              <tr v-for="c in cabinetTemps" :key="c.id" :class="gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical) === 'critical' ? 'table-danger' : gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical) === 'warning' ? 'table-warning' : ''">
                <td class="fw-semibold">{{ c.cabinetId }}</td><td class="text-muted">{{ c.location }}</td>
                <td><span class="fw-semibold" :class="`text-${GATE_COLOR[gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical)]}`">{{ c.inlet }}°C</span></td>
                <td>{{ c.outlet }}°C</td><td class="text-muted">{{ c.delta }}°C</td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical)]}`">{{ GATE_LABEL[gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical)] }}</span></td>
              </tr>
              <tr v-if="!cabinetTemps.length"><td colspan="6" class="text-center text-muted py-3">No data</td></tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>
  </div>
</template>
