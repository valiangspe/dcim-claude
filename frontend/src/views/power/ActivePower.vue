<script setup lang="ts">
const cabinets = [
  { id: 'CAB-A01', location: 'Hall A, Row 1', power: 8.4, capacity: 12, phase: 'L1' },
  { id: 'CAB-A02', location: 'Hall A, Row 1', power: 11.2, capacity: 12, phase: 'L2' },
  { id: 'CAB-A03', location: 'Hall A, Row 2', power: 6.1, capacity: 12, phase: 'L3' },
  { id: 'CAB-B01', location: 'Hall B, Row 1', power: 9.8, capacity: 16, phase: 'L1' },
  { id: 'CAB-B02', location: 'Hall B, Row 1', power: 14.3, capacity: 16, phase: 'L2' },
  { id: 'CAB-B03', location: 'Hall B, Row 2', power: 7.5, capacity: 16, phase: 'L3' },
  { id: 'CAB-C01', location: 'Hall C, Row 1', power: 5.2, capacity: 8, phase: 'L1' },
  { id: 'CAB-C02', location: 'Hall C, Row 1', power: 3.8, capacity: 8, phase: 'L2' },
]

function utilColor(pct: number): string {
  if (pct >= 85) return 'danger'
  if (pct >= 70) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Active Power</h4>
    <p class="text-muted">Actual active power per sub-location / cabinet.</p>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Cabinet</th>
              <th>Location</th>
              <th>Phase</th>
              <th class="text-end">Active (kW)</th>
              <th class="text-end">Capacity (kW)</th>
              <th style="width:22%">Utilization</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in cabinets" :key="c.id">
              <td class="fw-semibold">{{ c.id }}</td>
              <td class="text-muted">{{ c.location }}</td>
              <td><span class="badge bg-secondary">{{ c.phase }}</span></td>
              <td class="text-end">{{ c.power.toFixed(1) }}</td>
              <td class="text-end">{{ c.capacity }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="progress flex-fill" style="height: 8px;">
                    <div :class="`progress-bar bg-${utilColor(c.power / c.capacity * 100)}`" :style="{ width: (c.power / c.capacity * 100) + '%' }"></div>
                  </div>
                  <small>{{ (c.power / c.capacity * 100).toFixed(0) }}%</small>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
