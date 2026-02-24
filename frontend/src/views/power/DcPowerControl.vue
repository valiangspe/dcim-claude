<script setup lang="ts">
const rectifiers = [
  { id: 'RECT-01', voltage: 48.2, current: 312, power: 15.05, efficiency: 96.2, status: 'active' },
  { id: 'RECT-02', voltage: 48.1, current: 298, power: 14.33, efficiency: 95.8, status: 'active' },
  { id: 'RECT-03', voltage: 48.3, current: 305, power: 14.73, efficiency: 96.0, status: 'active' },
  { id: 'RECT-04', voltage: 0, current: 0, power: 0, efficiency: 0, status: 'standby' },
]

const buses = [
  { name: 'DC Bus A', voltage: 48.2, current: 620, power: 29.88, temp: 34 },
  { name: 'DC Bus B', voltage: 48.1, current: 295, power: 14.19, temp: 31 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">DC Power Control</h4>

    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">Rectifiers</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>ID</th><th class="text-end">Voltage (V)</th><th class="text-end">Current (A)</th><th class="text-end">Power (kW)</th><th class="text-end">Efficiency</th><th>Status</th></tr>
          </thead>
          <tbody>
            <tr v-for="r in rectifiers" :key="r.id">
              <td class="fw-semibold">{{ r.id }}</td>
              <td class="text-end">{{ r.voltage > 0 ? r.voltage.toFixed(1) : '---' }}</td>
              <td class="text-end">{{ r.current > 0 ? r.current : '---' }}</td>
              <td class="text-end">{{ r.power > 0 ? r.power.toFixed(2) : '---' }}</td>
              <td class="text-end">{{ r.efficiency > 0 ? r.efficiency.toFixed(1) + '%' : '---' }}</td>
              <td><span class="badge" :class="r.status === 'active' ? 'bg-success' : 'bg-secondary'">{{ r.status }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">DC Buses</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Bus</th><th class="text-end">Voltage (V)</th><th class="text-end">Current (A)</th><th class="text-end">Power (kW)</th><th class="text-end">Temp (&deg;C)</th></tr>
          </thead>
          <tbody>
            <tr v-for="b in buses" :key="b.name">
              <td class="fw-semibold">{{ b.name }}</td>
              <td class="text-end">{{ b.voltage.toFixed(1) }}</td>
              <td class="text-end">{{ b.current }}</td>
              <td class="text-end">{{ b.power.toFixed(2) }}</td>
              <td class="text-end">{{ b.temp }}&deg;C</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
