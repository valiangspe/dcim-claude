<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { upsUnitsApi, type UpsUnit } from '../../services/api'
import { THRESHOLDS, GATE_COLOR, GATE_LABEL, gate } from '../../composables/useComplianceExport'

const units = ref<UpsUnit[]>([])
const loading = ref(true)

onMounted(async () => {
  try { units.value = await upsUnitsApi.getAll() }
  finally { loading.value = false }
})

const rows = computed(() => units.value.map(u => ({
  ...u,
  g: gate(u.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical),
  tierSafe: u.loadPct < THRESHOLDS.upsLoad.warning,
  n1LoadPct: u.capacity > 0 ? parseFloat(((u.load / (u.capacity * 0.5)) * 100).toFixed(1)) : 0,
})))

const kpis = computed(() => [
  { label: 'Total UPS Units', value: String(units.value.length), color: 'primary' },
  { label: 'Tier III Safe (<40%)', value: String(rows.value.filter(r => r.tierSafe).length), color: 'success' },
  { label: 'Warning (40–45%)', value: String(rows.value.filter(r => r.g === 'warning').length), color: 'warning' },
  { label: 'Critical (>45%)', value: String(rows.value.filter(r => r.g === 'critical').length), color: 'danger' },
  { label: 'Concurrent Maint.', value: rows.value.every(r => r.tierSafe) ? 'PASS' : 'FAIL', color: rows.value.every(r => r.tierSafe) ? 'success' : 'danger' },
])
</script>

<template>
  <div>
    <h4 class="mb-1">UPS Bus Load</h4>
    <p class="text-muted small mb-4">Tier III 2N redundancy requires all UPS loads below 40% — ensures concurrent maintainability</p>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <h3 :class="`text-${kpi.color} mb-1`">{{ kpi.value }}</h3>
              <small class="text-muted">{{ kpi.label }}</small>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold">
          UPS Units — Warning &gt;40% · Critical &gt;45%
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>UPS Unit</th><th>Load %</th><th>Load / Capacity</th><th>Gate</th><th>Tier III Safe</th><th>Mode</th><th>Status</th></tr>
            </thead>
            <tbody>
              <tr v-for="u in rows" :key="u.id">
                <td class="fw-semibold">{{ u.name }}</td>
                <td style="min-width:180px">
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-grow-1" style="height:10px">
                      <div class="progress-bar" :class="`bg-${GATE_COLOR[u.g]}`" :style="`width:${Math.min(u.loadPct,100)}%`"></div>
                    </div>
                    <span class="fw-semibold" :class="`text-${GATE_COLOR[u.g]}`" style="width:42px">{{ u.loadPct }}%</span>
                  </div>
                </td>
                <td class="text-muted">{{ u.load }} / {{ u.capacity }} kW</td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[u.g]}`">{{ GATE_LABEL[u.g] }}</span></td>
                <td><span class="badge" :class="u.tierSafe ? 'bg-success' : 'bg-danger'">{{ u.tierSafe ? 'Yes' : 'No — Act' }}</span></td>
                <td class="text-muted">{{ u.mode }}</td>
                <td><span class="badge" :class="u.status === 'normal' ? 'bg-success' : 'bg-warning text-dark'">{{ u.status }}</span></td>
              </tr>
              <tr v-if="!rows.length"><td colspan="7" class="text-center text-muted py-4">No UPS data</td></tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">N-1 Stress Test — If one unit goes offline</div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>UPS Unit</th><th>Current Load %</th><th>Estimated N-1 Load %</th><th>N-1 Safe?</th><th>Recommendation</th></tr>
            </thead>
            <tbody>
              <tr v-for="u in rows" :key="u.id">
                <td class="fw-semibold">{{ u.name }}</td>
                <td>{{ u.loadPct }}%</td>
                <td>
                  <span :class="`text-${GATE_COLOR[gate(u.n1LoadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical)]}`" class="fw-semibold">
                    {{ u.n1LoadPct }}%
                  </span>
                </td>
                <td>
                  <span class="badge" :class="u.n1LoadPct < THRESHOLDS.upsLoad.critical ? 'bg-success' : 'bg-danger'">
                    {{ u.n1LoadPct < THRESHOLDS.upsLoad.critical ? 'Safe' : 'Unsafe' }}
                  </span>
                </td>
                <td class="text-muted small">{{ u.n1LoadPct < THRESHOLDS.upsLoad.critical ? 'No action required' : 'Reduce load before scheduling maintenance' }}</td>
              </tr>
              <tr v-if="!rows.length"><td colspan="5" class="text-center text-muted py-4">No UPS data</td></tr>
            </tbody>
          </table>
        </div>
        <div class="card-footer text-muted small">N-1 estimate assumes 50% capacity available when one unit is removed from service.</div>
      </div>
    </template>
  </div>
</template>
