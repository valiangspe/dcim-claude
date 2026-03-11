<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { cabinetTemperaturesApi, type CabinetTemperature } from '../../services/api'
import { THRESHOLDS, GATE_COLOR, GATE_LABEL, gate } from '../../composables/useComplianceExport'

const temps = ref<CabinetTemperature[]>([])
const loading = ref(true)
const sortBy = ref<'inlet' | 'cabinetId'>('inlet')

onMounted(async () => {
  try { temps.value = await cabinetTemperaturesApi.getAll() }
  finally { loading.value = false }
})

const rows = computed(() => {
  const mapped = temps.value.map(c => ({ ...c, g: gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical) }))
  return sortBy.value === 'inlet' ? mapped.sort((a, b) => b.inlet - a.inlet) : mapped.sort((a, b) => a.cabinetId.localeCompare(b.cabinetId))
})

const maxInlet = computed(() => temps.value.length ? Math.max(...temps.value.map(c => c.inlet)) : null)
const avgInlet = computed(() => temps.value.length ? (temps.value.reduce((s, c) => s + c.inlet, 0) / temps.value.length).toFixed(1) : null)

const kpis = computed(() => [
  { label: 'Max Inlet Temp', value: maxInlet.value != null ? maxInlet.value + '°C' : '—', color: maxInlet.value != null ? GATE_COLOR[gate(maxInlet.value, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical)] : 'secondary' },
  { label: 'Avg Inlet Temp', value: avgInlet.value != null ? avgInlet.value + '°C' : '—', color: 'primary' },
  { label: 'OK (<25°C)', value: String(rows.value.filter(r => r.g === 'ok').length), color: 'success' },
  { label: 'Warning (25–27°C)', value: String(rows.value.filter(r => r.g === 'warning').length), color: 'warning' },
  { label: 'Critical (>27°C)', value: String(rows.value.filter(r => r.g === 'critical').length), color: 'danger' },
  { label: 'Total Cabinets', value: String(temps.value.length), color: 'info' },
])
</script>

<template>
  <div>
    <h4 class="mb-1">Server Inlet Temperature</h4>
    <p class="text-muted small mb-4">ASHRAE A1 envelope — Warning above 25°C, Critical above 27°C</p>

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
        <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
          <span>Cabinet Inlet Temperatures</span>
          <div class="d-flex align-items-center gap-2">
            <small class="text-muted">Sort:</small>
            <select v-model="sortBy" class="form-select form-select-sm" style="width:130px">
              <option value="inlet">By Temp (↓)</option>
              <option value="cabinetId">By Cabinet</option>
            </select>
          </div>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Cabinet</th><th>Location</th><th>Inlet °C</th><th>Outlet °C</th><th>Delta °C</th><th>Gate</th><th>ASHRAE Alarm</th></tr>
            </thead>
            <tbody>
              <tr v-for="c in rows" :key="c.id" :class="c.g === 'critical' ? 'table-danger' : c.g === 'warning' ? 'table-warning' : ''">
                <td class="fw-semibold">{{ c.cabinetId }}</td>
                <td class="text-muted">{{ c.location }}</td>
                <td>
                  <span class="fw-semibold fs-6" :class="`text-${GATE_COLOR[c.g]}`">{{ c.inlet }}°C</span>
                  <div class="progress mt-1" style="height:4px;width:80px">
                    <div class="progress-bar" :class="`bg-${GATE_COLOR[c.g]}`" :style="`width:${Math.min((c.inlet / 30) * 100, 100)}%`"></div>
                  </div>
                </td>
                <td>{{ c.outlet }}°C</td>
                <td :class="c.delta > 15 ? 'text-warning' : 'text-muted'">{{ c.delta }}°C</td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[c.g]}`">{{ GATE_LABEL[c.g] }}</span></td>
                <td>
                  <span v-if="c.alarm" class="badge bg-danger">Active</span>
                  <span v-else class="text-muted small">—</span>
                </td>
              </tr>
              <tr v-if="!rows.length"><td colspan="7" class="text-center text-muted py-4">No temperature data</td></tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>
  </div>
</template>
