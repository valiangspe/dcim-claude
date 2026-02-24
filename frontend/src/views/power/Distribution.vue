<script setup lang="ts">
const chains = [
  { name: 'Chain A (Utility → ATS-A → UPS-A1/A2 → PDU-A)', source: 'Utility Feed A', capacity: 2100, load: 1480, redundancy: 'N+1', status: 'active' },
  { name: 'Chain B (Utility → ATS-B → UPS-B1/B2 → PDU-B)', source: 'Utility Feed B', capacity: 2100, load: 1367, redundancy: 'N+1', status: 'active' },
  { name: 'Chain C (Generator → ATS-C → UPS-C1 → PDU-C)', source: 'Generator G1', capacity: 2000, load: 0, redundancy: 'N', status: 'standby' },
]

const pdus = [
  { name: 'PDU-A1', chain: 'Chain A', input: 480, output: 208, load: 380, capacity: 500, breakers: 42 },
  { name: 'PDU-A2', chain: 'Chain A', input: 480, output: 208, load: 360, capacity: 500, breakers: 42 },
  { name: 'PDU-B1', chain: 'Chain B', input: 480, output: 208, load: 410, capacity: 500, breakers: 42 },
  { name: 'PDU-B2', chain: 'Chain B', input: 480, output: 208, load: 320, capacity: 500, breakers: 42 },
  { name: 'PDU-C1', chain: 'Chain C', input: 480, output: 208, load: 0, capacity: 500, breakers: 42 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Distribution & Redundancy</h4>

    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">Power Chains</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Chain</th><th>Source</th><th class="text-end">Capacity</th><th class="text-end">Load</th><th>Redundancy</th><th>Status</th></tr>
          </thead>
          <tbody>
            <tr v-for="c in chains" :key="c.name">
              <td class="fw-semibold">{{ c.name }}</td>
              <td>{{ c.source }}</td>
              <td class="text-end">{{ c.capacity.toLocaleString() }} kW</td>
              <td class="text-end">{{ c.load > 0 ? c.load.toLocaleString() + ' kW' : '---' }}</td>
              <td><span class="badge bg-info text-dark">{{ c.redundancy }}</span></td>
              <td><span class="badge" :class="c.status === 'active' ? 'bg-success' : 'bg-secondary'">{{ c.status }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">PDU Details</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>PDU</th><th>Chain</th><th class="text-end">Input (V)</th><th class="text-end">Output (V)</th><th class="text-end">Load (kW)</th><th class="text-end">Capacity (kW)</th><th class="text-end">Breakers</th></tr>
          </thead>
          <tbody>
            <tr v-for="p in pdus" :key="p.name">
              <td class="fw-semibold">{{ p.name }}</td>
              <td>{{ p.chain }}</td>
              <td class="text-end">{{ p.input }}</td>
              <td class="text-end">{{ p.output }}</td>
              <td class="text-end">{{ p.load > 0 ? p.load : '---' }}</td>
              <td class="text-end">{{ p.capacity }}</td>
              <td class="text-end">{{ p.breakers }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
