<script setup lang="ts">
const kpis = [
  { label: 'Total Racks', value: '186', color: 'primary' },
  { label: 'Occupied Racks', value: '164', color: 'info' },
  { label: 'Rack Utilization', value: '88.2%', color: 'warning' },
  { label: 'Total Floor Space', value: '42,000 sqft', color: 'dark' },
  { label: 'Available Space', value: '6,200 sqft', color: 'success' },
  { label: 'Total U Slots', value: '8,280', color: 'secondary' },
]

const halls = [
  { name: 'Hall A', sqft: 16000, racks: 72, occupied: 68, totalU: 3024, usedU: 2480, power: 1240 },
  { name: 'Hall B', sqft: 14000, racks: 62, occupied: 58, totalU: 2604, usedU: 2120, power: 980 },
  { name: 'Hall C', sqft: 8000, racks: 32, occupied: 24, totalU: 1344, usedU: 820, power: 412 },
  { name: 'Hall D', sqft: 4000, racks: 20, occupied: 14, totalU: 1308, usedU: 472, power: 215 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Space Overview</h4>

    <div class="row g-3 mb-4">
      <div v-for="k in kpis" :key="k.label" class="col-sm-6 col-lg-4 col-xl-2">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body text-center">
            <h3 :class="`text-${k.color} mb-0`">{{ k.value }}</h3>
            <small class="text-muted">{{ k.label }}</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Per-Hall Breakdown</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Hall</th><th class="text-end">Floor (sqft)</th><th class="text-end">Racks</th>
              <th class="text-end">Occupied</th><th style="width:14%">Rack Util</th>
              <th class="text-end">Total U</th><th class="text-end">Used U</th><th style="width:14%">U Util</th>
              <th class="text-end">Power (kW)</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="h in halls" :key="h.name">
              <td class="fw-semibold">{{ h.name }}</td>
              <td class="text-end">{{ h.sqft.toLocaleString() }}</td>
              <td class="text-end">{{ h.racks }}</td>
              <td class="text-end">{{ h.occupied }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="progress flex-fill" style="height: 8px;">
                    <div class="progress-bar bg-primary" :style="{ width: (h.occupied / h.racks * 100) + '%' }"></div>
                  </div>
                  <small>{{ (h.occupied / h.racks * 100).toFixed(0) }}%</small>
                </div>
              </td>
              <td class="text-end">{{ h.totalU.toLocaleString() }}</td>
              <td class="text-end">{{ h.usedU.toLocaleString() }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="progress flex-fill" style="height: 8px;">
                    <div class="progress-bar bg-info" :style="{ width: (h.usedU / h.totalU * 100) + '%' }"></div>
                  </div>
                  <small>{{ (h.usedU / h.totalU * 100).toFixed(0) }}%</small>
                </div>
              </td>
              <td class="text-end">{{ h.power.toLocaleString() }}</td>
            </tr>
          </tbody>
          <tfoot class="table-light">
            <tr class="fw-semibold">
              <td>Total</td>
              <td class="text-end">{{ halls.reduce((a, h) => a + h.sqft, 0).toLocaleString() }}</td>
              <td class="text-end">{{ halls.reduce((a, h) => a + h.racks, 0) }}</td>
              <td class="text-end">{{ halls.reduce((a, h) => a + h.occupied, 0) }}</td>
              <td></td>
              <td class="text-end">{{ halls.reduce((a, h) => a + h.totalU, 0).toLocaleString() }}</td>
              <td class="text-end">{{ halls.reduce((a, h) => a + h.usedU, 0).toLocaleString() }}</td>
              <td></td>
              <td class="text-end">{{ halls.reduce((a, h) => a + h.power, 0).toLocaleString() }}</td>
            </tr>
          </tfoot>
        </table>
      </div>
    </div>
  </div>
</template>
