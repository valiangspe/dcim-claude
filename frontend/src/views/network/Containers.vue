<script setup lang="ts">
const containers = [
  { name: 'k8s-node-01', ip: '10.0.1.10', pods: 42, cpu: 68, mem: 72, network: '2.4 Gbps', status: 'running' },
  { name: 'k8s-node-02', ip: '10.0.1.11', pods: 38, cpu: 55, mem: 64, network: '1.8 Gbps', status: 'running' },
  { name: 'k8s-node-03', ip: '10.0.1.12', pods: 45, cpu: 82, mem: 78, network: '3.1 Gbps', status: 'running' },
  { name: 'docker-host-01', ip: '10.0.2.10', pods: 18, cpu: 34, mem: 42, network: '0.8 Gbps', status: 'running' },
  { name: 'docker-host-02', ip: '10.0.2.11', pods: 12, cpu: 22, mem: 30, network: '0.5 Gbps', status: 'running' },
  { name: 'k8s-node-04', ip: '10.0.1.13', pods: 0, cpu: 0, mem: 0, network: '0 Gbps', status: 'maintenance' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Containers</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Host</th><th>IP</th><th class="text-end">Pods</th><th class="text-end">CPU %</th><th class="text-end">Mem %</th><th class="text-end">Network</th><th>Status</th></tr></thead>
      <tbody><tr v-for="c in containers" :key="c.name">
        <td class="fw-semibold font-monospace">{{ c.name }}</td><td class="font-monospace">{{ c.ip }}</td>
        <td class="text-end">{{ c.pods }}</td>
        <td class="text-end" :class="c.cpu>80?'text-danger fw-semibold':''">{{ c.cpu || '---' }}%</td>
        <td class="text-end" :class="c.mem>75?'text-warning':''">{{ c.mem || '---' }}%</td>
        <td class="text-end">{{ c.network }}</td>
        <td><span class="badge" :class="c.status==='running'?'bg-success':'bg-secondary'">{{ c.status }}</span></td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
