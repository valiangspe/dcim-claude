<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { upsUnitsApi, type UpsUnit, powerChainsApi, type PowerChain, racksApi, type Rack } from '../../services/api'
import { THRESHOLDS, GATE_COLOR, GATE_LABEL, gate, downloadQuarterly } from '../../composables/useComplianceExport'

const loading = ref(true)
const upsUnits = ref<UpsUnit[]>([])
const powerChains = ref<PowerChain[]>([])
const racks = ref<Rack[]>([])

onMounted(async () => {
  const r = await Promise.allSettled([upsUnitsApi.getAll(), powerChainsApi.getAll(), racksApi.getAll()])
  const g = <T>(x: PromiseSettledResult<T[]>) => x.status === 'fulfilled' ? x.value : []
  upsUnits.value = g(r[0] as PromiseSettledResult<UpsUnit[]>)
  powerChains.value = g(r[1] as PromiseSettledResult<PowerChain[]>)
  racks.value = g(r[2] as PromiseSettledResult<Rack[]>)
  loading.value = false
})

const upsRows = computed(() => upsUnits.value.map(u => ({
  ...u, g: gate(u.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical),
  tierSafe: u.loadPct < THRESHOLDS.upsLoad.warning,
  n1: u.capacity > 0 ? parseFloat(((u.load / (u.capacity * 0.5)) * 100).toFixed(1)) : 0,
})))
const chainRows = computed(() => powerChains.value.map(c => {
  const pct = c.capacity > 0 ? parseFloat(((c.load / c.capacity) * 100).toFixed(1)) : 0
  return { ...c, loadPct: pct, safe: pct < THRESHOLDS.upsLoad.warning }
}))
const rackRows = computed(() => racks.value.map(r => {
  const pct = r.totalU > 0 ? parseFloat(((r.usedU / r.totalU) * 100).toFixed(1)) : 0
  return { ...r, pct, g: gate(pct, THRESHOLDS.rackUtil.warning, THRESHOLDS.rackUtil.critical) }
}))
const cmPass = computed(() => upsRows.value.every(u => u.tierSafe) && chainRows.value.every(c => c.safe))
const overallRackPct = computed(() => {
  const total = racks.value.reduce((s, r) => s + r.totalU, 0)
  const used = racks.value.reduce((s, r) => s + r.usedU, 0)
  return total > 0 ? parseFloat(((used / total) * 100).toFixed(1)) : 0
})

function doDownload() { downloadQuarterly(upsUnits.value, powerChains.value, racks.value) }
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-start mb-4 flex-wrap gap-3">
      <div>
        <h4 class="mb-1">Quarterly Capacity &amp; Redundancy Report</h4>
        <p class="text-muted small mb-0">Concurrent maintainability · N-1 stress test · Rack capacity · Power chain redundancy</p>
      </div>
      <button class="btn btn-success" @click="doDownload">⬇ Download XLSX</button>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>

      <div class="alert mb-4" :class="cmPass ? 'alert-success' : 'alert-danger'">
        <strong>Tier III Concurrent Maintainability: {{ cmPass ? 'PASS' : 'FAIL' }}</strong>
        <span v-if="!cmPass"> — {{ upsRows.filter(u => !u.tierSafe).length }} UPS unit(s) and {{ chainRows.filter(c => !c.safe).length }} chain(s) exceed the 40% load threshold.</span>
        <span v-else> — All systems below 40% load. Safe to remove any single component for maintenance.</span>
      </div>

      <div class="row g-3 mb-4">
        <div class="col-sm-6 col-lg-3">
          <div class="card border-0 shadow-sm h-100"><div class="card-body">
            <h3 :class="cmPass ? 'text-success' : 'text-danger'">{{ cmPass ? 'Safe' : 'At Risk' }}</h3>
            <small class="text-muted d-block">Concurrent Maintainability</small><small class="text-secondary">Tier III validation</small>
          </div></div>
        </div>
        <div class="col-sm-6 col-lg-3">
          <div class="card border-0 shadow-sm h-100"><div class="card-body">
            <h3 :class="`text-${GATE_COLOR[gate(overallRackPct, THRESHOLDS.rackUtil.warning, THRESHOLDS.rackUtil.critical)]}`">{{ overallRackPct }}%</h3>
            <small class="text-muted d-block">Overall Rack Utilization</small><small class="text-secondary">{{ racks.length }} racks</small>
          </div></div>
        </div>
        <div class="col-sm-6 col-lg-3">
          <div class="card border-0 shadow-sm h-100"><div class="card-body">
            <h3 :class="upsRows.filter(u => !u.tierSafe).length > 0 ? 'text-danger' : 'text-success'">{{ upsRows.filter(u => u.tierSafe).length }}/{{ upsRows.length }}</h3>
            <small class="text-muted d-block">UPS Tier III Safe</small><small class="text-secondary">Below 40% load</small>
          </div></div>
        </div>
        <div class="col-sm-6 col-lg-3">
          <div class="card border-0 shadow-sm h-100"><div class="card-body">
            <h3 :class="chainRows.filter(c => !c.safe).length > 0 ? 'text-danger' : 'text-success'">{{ chainRows.filter(c => c.safe).length }}/{{ chainRows.length }}</h3>
            <small class="text-muted d-block">Chains Within Threshold</small><small class="text-secondary">Below 40% load</small>
          </div></div>
        </div>
      </div>

      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold">N-1 Stress Test — UPS Units</div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>UPS Unit</th><th>Current Load %</th><th>N-1 Est. Load %</th><th>N-1 Safe?</th><th>Recommendation</th></tr></thead>
            <tbody>
              <tr v-for="u in upsRows" :key="u.id">
                <td class="fw-semibold">{{ u.name }}</td>
                <td><span :class="`text-${GATE_COLOR[u.g]}`">{{ u.loadPct }}%</span></td>
                <td><span class="fw-semibold" :class="`text-${GATE_COLOR[gate(u.n1, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical)]}`">{{ u.n1 }}%</span></td>
                <td><span class="badge" :class="u.n1 < THRESHOLDS.upsLoad.critical ? 'bg-success' : 'bg-danger'">{{ u.n1 < THRESHOLDS.upsLoad.critical ? 'Safe' : 'Unsafe' }}</span></td>
                <td class="text-muted small">{{ u.n1 < THRESHOLDS.upsLoad.critical ? 'No action required' : 'Reduce load before maintenance' }}</td>
              </tr>
              <tr v-if="!upsRows.length"><td colspan="5" class="text-center text-muted py-3">No UPS data</td></tr>
            </tbody>
          </table>
        </div>
        <div class="card-footer text-muted small">N-1 estimate assumes 50% capacity remaining when one unit is removed from service.</div>
      </div>

      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold">Power Chain Redundancy</div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Chain</th><th>Source</th><th>Load %</th><th>Redundancy</th><th>Status</th><th>Concurrent Safe</th></tr></thead>
            <tbody>
              <tr v-for="c in chainRows" :key="c.id">
                <td class="fw-semibold">{{ c.name }}</td><td class="text-muted">{{ c.source }}</td>
                <td style="min-width:140px">
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-grow-1" style="height:6px"><div class="progress-bar" :class="`bg-${GATE_COLOR[gate(c.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical)]}`" :style="`width:${c.loadPct}%`"></div></div>
                    <span>{{ c.loadPct }}%</span>
                  </div>
                </td>
                <td><span class="badge bg-light text-dark border">{{ c.redundancy }}</span></td>
                <td><span class="badge" :class="c.status === 'active' ? 'bg-success' : 'bg-warning text-dark'">{{ c.status }}</span></td>
                <td><span class="badge" :class="c.safe ? 'bg-success' : 'bg-danger'">{{ c.safe ? 'Yes' : 'No' }}</span></td>
              </tr>
              <tr v-if="!chainRows.length"><td colspan="6" class="text-center text-muted py-3">No chain data</td></tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Rack Space Capacity</span>
          <span class="badge" :class="`bg-${GATE_COLOR[gate(overallRackPct, THRESHOLDS.rackUtil.warning, THRESHOLDS.rackUtil.critical)]}`">{{ GATE_LABEL[gate(overallRackPct, THRESHOLDS.rackUtil.warning, THRESHOLDS.rackUtil.critical)] }}</span>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0 small">
            <thead class="table-light"><tr><th>Rack</th><th>Location</th><th>Utilization</th><th>Free U</th><th>Gate</th></tr></thead>
            <tbody>
              <tr v-for="r in rackRows" :key="r.id">
                <td class="fw-semibold">{{ r.rackId }}</td><td class="text-muted">{{ r.hall }}</td>
                <td style="min-width:160px">
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-grow-1" style="height:8px"><div class="progress-bar" :class="`bg-${GATE_COLOR[r.g]}`" :style="`width:${r.pct}%`"></div></div>
                    <span :class="`text-${GATE_COLOR[r.g]}`">{{ r.pct }}%</span>
                  </div>
                </td>
                <td>{{ r.totalU - r.usedU }} U</td>
                <td><span class="badge" :class="`bg-${GATE_COLOR[r.g]}`">{{ GATE_LABEL[r.g] }}</span></td>
              </tr>
              <tr v-if="!rackRows.length"><td colspan="5" class="text-center text-muted py-3">No rack data</td></tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>
  </div>
</template>
