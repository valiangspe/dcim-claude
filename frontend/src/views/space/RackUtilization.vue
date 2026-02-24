<script setup lang="ts">
const racks = [
  { id: 'CAB-A01', hall: 'Hall A', totalU: 42, usedU: 38, power: 8.4, weight: 680 },
  { id: 'CAB-A02', hall: 'Hall A', totalU: 42, usedU: 42, power: 11.2, weight: 820 },
  { id: 'CAB-A03', hall: 'Hall A', totalU: 42, usedU: 28, power: 6.1, weight: 510 },
  { id: 'CAB-A04', hall: 'Hall A', totalU: 42, usedU: 35, power: 7.8, weight: 620 },
  { id: 'CAB-B01', hall: 'Hall B', totalU: 42, usedU: 40, power: 9.8, weight: 740 },
  { id: 'CAB-B02', hall: 'Hall B', totalU: 42, usedU: 41, power: 14.3, weight: 890 },
  { id: 'CAB-B03', hall: 'Hall B', totalU: 42, usedU: 22, power: 7.5, weight: 450 },
  { id: 'CAB-C01', hall: 'Hall C', totalU: 42, usedU: 30, power: 5.2, weight: 380 },
  { id: 'CAB-C02', hall: 'Hall C', totalU: 42, usedU: 18, power: 3.8, weight: 290 },
  { id: 'CAB-D01', hall: 'Hall D', totalU: 48, usedU: 12, power: 2.4, weight: 180 },
]

function utilColor(pct: number): string {
  if (pct >= 95) return 'danger'
  if (pct >= 80) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Rack Utilization</h4>

    <div class="row g-3 mb-4">
      <div v-for="rack in racks" :key="rack.id" class="col-sm-6 col-md-4 col-xl-3">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <span class="fw-semibold">{{ rack.id }}</span>
              <small class="text-muted">{{ rack.hall }}</small>
            </div>
            <div class="text-center mb-2">
              <span :class="`display-6 fw-bold text-${utilColor(rack.usedU / rack.totalU * 100)}`">{{ (rack.usedU / rack.totalU * 100).toFixed(0) }}%</span>
            </div>
            <div class="progress mb-2" style="height: 12px;">
              <div :class="`progress-bar bg-${utilColor(rack.usedU / rack.totalU * 100)}`" :style="{ width: (rack.usedU / rack.totalU * 100) + '%' }"></div>
            </div>
            <div class="small text-muted d-flex justify-content-between">
              <span>{{ rack.usedU }}/{{ rack.totalU }}U</span>
              <span>{{ rack.power }} kW</span>
              <span>{{ rack.weight }} kg</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
