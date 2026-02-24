<script setup lang="ts">
const metrics = [
  { name: 'CER (Cooling Efficiency Ratio)', value: 0.85, unit: '', target: 0.90, description: 'IT heat removed / total cooling energy' },
  { name: 'PUEc (Partial PUE - Cooling)', value: 1.17, unit: '', target: 1.15, description: 'Cooling energy / IT load' },
  { name: 'EER (Energy Efficiency Ratio)', value: 3.8, unit: 'BTU/Wh', target: 4.0, description: 'Cooling output / electrical input' },
  { name: 'COP (Coefficient of Performance)', value: 4.2, unit: '', target: 4.5, description: 'Heat removed / work input' },
]

const monthly = [
  { month: 'Sep 2025', cer: 0.82, puec: 1.19, eer: 3.6, cop: 3.9 },
  { month: 'Oct 2025', cer: 0.83, puec: 1.18, eer: 3.7, cop: 4.0 },
  { month: 'Nov 2025', cer: 0.84, puec: 1.18, eer: 3.7, cop: 4.1 },
  { month: 'Dec 2025', cer: 0.84, puec: 1.17, eer: 3.8, cop: 4.1 },
  { month: 'Jan 2026', cer: 0.85, puec: 1.17, eer: 3.8, cop: 4.2 },
  { month: 'Feb 2026', cer: 0.85, puec: 1.17, eer: 3.8, cop: 4.2 },
]

function vsTarget(val: number, target: number, higherBetter: boolean): string {
  return higherBetter ? (val >= target ? 'text-success' : 'text-danger') : (val <= target ? 'text-success' : 'text-danger')
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cooling Efficiency</h4>

    <div class="row g-3 mb-4">
      <div v-for="m in metrics" :key="m.name" class="col-md-6 col-xl-3">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body text-center">
            <h2 class="text-primary mb-1">{{ m.value }}<small v-if="m.unit" class="text-muted fs-6 ms-1">{{ m.unit }}</small></h2>
            <div class="fw-semibold small">{{ m.name }}</div>
            <small class="text-muted">Target: {{ m.target }} &middot; {{ m.description }}</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">6-Month Trend</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Month</th><th class="text-end">CER</th><th class="text-end">PUEc</th><th class="text-end">EER</th><th class="text-end">COP</th></tr>
          </thead>
          <tbody>
            <tr v-for="r in monthly" :key="r.month">
              <td class="fw-semibold">{{ r.month }}</td>
              <td class="text-end" :class="vsTarget(r.cer, 0.90, true)">{{ r.cer.toFixed(2) }}</td>
              <td class="text-end" :class="vsTarget(r.puec, 1.15, false)">{{ r.puec.toFixed(2) }}</td>
              <td class="text-end" :class="vsTarget(r.eer, 4.0, true)">{{ r.eer.toFixed(1) }}</td>
              <td class="text-end" :class="vsTarget(r.cop, 4.5, true)">{{ r.cop.toFixed(1) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
