<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { powerPhasesApi, type PowerPhase } from '../../services/api'
import { THRESHOLDS, GATE_COLOR, GATE_LABEL, phaseDeviations } from '../../composables/useComplianceExport'

const phases = ref<PowerPhase[]>([])
const loading = ref(true)

onMounted(async () => {
  try { phases.value = await powerPhasesApi.getAll() }
  finally { loading.value = false }
})

const rows = computed(() => phaseDeviations(phases.value))

const avgV = computed(() => phases.value.length ? (phases.value.reduce((s, p) => s + p.voltage, 0) / phases.value.length).toFixed(1) : null)
const maxDev = computed(() => rows.value.length ? Math.max(...rows.value.map(r => r.deviation)) : null)

const kpis = computed(() => [
  { label: 'Phases Monitored', value: String(phases.value.length), color: 'primary' },
  { label: 'Avg Voltage', value: avgV.value ? avgV.value + ' V' : '—', color: 'info' },
  { label: 'Max Deviation', value: maxDev.value != null ? maxDev.value + '%' : '—', color: maxDev.value != null && maxDev.value >= THRESHOLDS.phaseBalance.critical ? 'danger' : maxDev.value != null && maxDev.value >= THRESHOLDS.phaseBalance.warning ? 'warning' : 'success' },
  { label: 'Balanced (<15%)', value: String(rows.value.filter(r => r.gate === 'ok').length), color: 'success' },
  { label: 'Warning (15–25%)', value: String(rows.value.filter(r => r.gate === 'warning').length), color: 'warning' },
  { label: 'Critical (>25%)', value: String(rows.value.filter(r => r.gate === 'critical').length), color: 'danger' },
])
</script>

<template>
  <div>
    <h4 class="mb-1">Phase Balance</h4>
    <p class="text-muted small mb-4">Voltage deviation from average — Warning above 15%, Critical above 25%. Unbalanced phases reduce redundancy effectiveness.</p>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <h3 :class="`text-${kpi.color} mb-1`">{{ kpi.value }}</h3>
              <small class="text-muted">{{ kpi.label }}</small>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">Phase Measurements</div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Phase</th><th>Voltage (V)</th><th>Current (A)</th><th>Freq (Hz)</th><th>THD</th><th>PF</th><th>Deviation</th><th>Gate</th></tr>
            </thead>
            <tbody>
              <tr v-for="p in rows" :key="p.id" :class="p.gate === 'critical' ? 'table-danger' : p.gate === 'warning' ? 'table-warning' : ''">
                <td class="fw-semibold">{{ p.name }}</td>
                <td>{{ p.voltage }} V</td>
                <td>{{ p.current }} A</td>
                <td>{{ p.frequency }} Hz</td>
                <td :class="p.thd > 5 ? 'text-warning' : ''">{{ p.thd }}%</td>
                <td :class="p.pf < 0.9 ? 'text-warning' : ''">{{ p.pf }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress" style="height:6px;width:70px">
                      <div class="progress-bar" :class="`bg-${GATE_COLOR[p.gate]}`" :style="`width:${Math.min(p.deviation / 30 * 100, 100)}%`"></div>
                    </div>
                    <span :class="`text-${GATE_COLOR[p.gate]}`" class="fw-semibold">{{ p.deviation }}%</span>
                  </div>
                </td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[p.gate]}`">{{ GATE_LABEL[p.gate] }}</span></td>
              </tr>
              <tr v-if="!rows.length"><td colspan="8" class="text-center text-muted py-4">No phase data</td></tr>
            </tbody>
          </table>
        </div>
        <div class="card-footer text-muted small">Deviation calculated as % variance from average voltage across all monitored phases.</div>
      </div>
    </template>
  </div>
</template>
