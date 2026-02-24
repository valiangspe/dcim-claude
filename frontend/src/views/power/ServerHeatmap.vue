<script setup lang="ts">
const hours = ['00', '04', '08', '12', '16', '20']
const days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']

// Power values in kW (rows = days, cols = hours)
const data: number[][] = [
  [2.1, 1.9, 2.8, 3.4, 3.2, 2.6],
  [2.0, 1.8, 2.9, 3.5, 3.3, 2.5],
  [2.2, 2.0, 3.0, 3.6, 3.4, 2.7],
  [2.1, 1.9, 2.7, 3.3, 3.1, 2.5],
  [2.3, 2.1, 3.1, 3.7, 3.5, 2.8],
  [1.5, 1.3, 1.6, 1.8, 1.7, 1.4],
  [1.4, 1.2, 1.5, 1.7, 1.6, 1.3],
]

function cellColor(val: number): string {
  if (val >= 3.5) return 'bg-danger text-white'
  if (val >= 3.0) return 'bg-warning text-dark'
  if (val >= 2.0) return 'bg-info text-dark'
  return 'bg-success text-white'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Server Power Heatmap</h4>
    <p class="text-muted">Average power consumption (kW) by time of day and day of week.</p>

    <div class="card border-0 shadow-sm mb-4">
      <div class="card-body p-0">
        <table class="table table-bordered text-center mb-0">
          <thead class="table-light">
            <tr>
              <th></th>
              <th v-for="h in hours" :key="h">{{ h }}:00</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(row, di) in data" :key="days[di]">
              <td class="fw-semibold table-light">{{ days[di] }}</td>
              <td v-for="(val, hi) in row" :key="hi" :class="cellColor(val)" style="min-width: 70px;">
                {{ val.toFixed(1) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="d-flex gap-3 small">
      <span><span class="badge bg-success">&nbsp;&nbsp;</span> &lt;2.0 kW</span>
      <span><span class="badge bg-info">&nbsp;&nbsp;</span> 2.0 - 3.0 kW</span>
      <span><span class="badge bg-warning">&nbsp;&nbsp;</span> 3.0 - 3.5 kW</span>
      <span><span class="badge bg-danger">&nbsp;&nbsp;</span> &ge;3.5 kW</span>
    </div>
  </div>
</template>
