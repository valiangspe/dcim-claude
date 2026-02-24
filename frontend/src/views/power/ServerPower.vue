<script setup lang="ts">
const servers = [
  { name: 'srv-web-01', rack: 'CAB-A01', power: 0.42, cpu: 67, psu: '2x750W', status: 'online' },
  { name: 'srv-web-02', rack: 'CAB-A01', power: 0.38, cpu: 54, psu: '2x750W', status: 'online' },
  { name: 'srv-db-01', rack: 'CAB-A02', power: 0.81, cpu: 82, psu: '2x1200W', status: 'online' },
  { name: 'srv-db-02', rack: 'CAB-A02', power: 0.76, cpu: 78, psu: '2x1200W', status: 'online' },
  { name: 'srv-app-01', rack: 'CAB-B01', power: 0.55, cpu: 45, psu: '2x750W', status: 'online' },
  { name: 'srv-app-02', rack: 'CAB-B01', power: 0.52, cpu: 41, psu: '2x750W', status: 'online' },
  { name: 'srv-gpu-01', rack: 'CAB-B02', power: 2.40, cpu: 91, psu: '4x1600W', status: 'online' },
  { name: 'srv-gpu-02', rack: 'CAB-B02', power: 2.35, cpu: 89, psu: '4x1600W', status: 'online' },
  { name: 'srv-stor-01', rack: 'CAB-C01', power: 0.68, cpu: 22, psu: '2x1200W', status: 'online' },
  { name: 'srv-bkup-01', rack: 'CAB-C02', power: 0.12, cpu: 5, psu: '2x750W', status: 'idle' },
]

const totalPower = servers.reduce((a, s) => a + s.power, 0)
</script>

<template>
  <div>
    <h4 class="mb-4">Server Power</h4>

    <div class="alert alert-light border mb-4">
      <strong>Total Server Power:</strong> {{ totalPower.toFixed(2) }} kW across {{ servers.length }} servers
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Server</th><th>Rack</th><th>PSU</th><th class="text-end">Power (kW)</th><th class="text-end">CPU %</th><th>Status</th></tr>
          </thead>
          <tbody>
            <tr v-for="s in servers" :key="s.name">
              <td class="fw-semibold font-monospace">{{ s.name }}</td>
              <td>{{ s.rack }}</td>
              <td class="text-muted small">{{ s.psu }}</td>
              <td class="text-end">{{ s.power.toFixed(2) }}</td>
              <td class="text-end">
                <span :class="s.cpu > 80 ? 'text-danger fw-semibold' : s.cpu > 60 ? 'text-warning' : ''">{{ s.cpu }}%</span>
              </td>
              <td><span class="badge" :class="s.status === 'online' ? 'bg-success' : 'bg-secondary'">{{ s.status }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
