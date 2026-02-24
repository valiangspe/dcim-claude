<script setup lang="ts">
const currentPue = 1.38
const target = 1.40

const daily = [
  { date: '2026-02-14', pue: 1.41 }, { date: '2026-02-15', pue: 1.39 },
  { date: '2026-02-16', pue: 1.40 }, { date: '2026-02-17', pue: 1.38 },
  { date: '2026-02-18', pue: 1.37 }, { date: '2026-02-19', pue: 1.39 },
  { date: '2026-02-20', pue: 1.38 },
]

function pueColor(pue: number): string {
  if (pue <= 1.35) return 'success'
  if (pue <= 1.45) return 'warning'
  return 'danger'
}

const breakdown = [
  { component: 'IT Load', power: 2063, pct: 72.4 },
  { component: 'Cooling', power: 498, pct: 17.5 },
  { component: 'Lighting & Other', power: 142, pct: 5.0 },
  { component: 'Power Distribution Losses', power: 144, pct: 5.1 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">PUE Monitoring</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-4">
        <div :class="`card border-0 shadow-sm text-center bg-${pueColor(currentPue)} bg-opacity-10`">
          <div class="card-body py-4">
            <div :class="`display-3 fw-bold text-${pueColor(currentPue)}`">{{ currentPue.toFixed(2) }}</div>
            <div class="text-muted">Current PUE</div>
            <small class="text-muted">Target: {{ target.toFixed(2) }}</small>
          </div>
        </div>
      </div>
      <div class="col-md-8">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">Power Breakdown</div>
          <div class="card-body">
            <div v-for="b in breakdown" :key="b.component" class="mb-3">
              <div class="d-flex justify-content-between mb-1">
                <span>{{ b.component }}</span>
                <span class="fw-semibold">{{ b.power.toLocaleString() }} kW ({{ b.pct }}%)</span>
              </div>
              <div class="progress" style="height: 8px;">
                <div class="progress-bar bg-primary" :style="{ width: b.pct + '%' }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">7-Day PUE Trend</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Date</th><th>PUE</th><th style="width:50%">Bar</th><th class="text-end">vs Target</th></tr>
          </thead>
          <tbody>
            <tr v-for="d in daily" :key="d.date">
              <td>{{ d.date }}</td>
              <td :class="`fw-semibold text-${pueColor(d.pue)}`">{{ d.pue.toFixed(2) }}</td>
              <td>
                <div class="progress" style="height: 10px;">
                  <div :class="`progress-bar bg-${pueColor(d.pue)}`" :style="{ width: ((d.pue - 1.0) / 0.6 * 100) + '%' }"></div>
                </div>
              </td>
              <td class="text-end">
                <span :class="d.pue <= target ? 'text-success' : 'text-danger'">
                  {{ d.pue <= target ? 'On target' : '+' + (d.pue - target).toFixed(2) }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
