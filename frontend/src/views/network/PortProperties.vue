<script setup lang="ts">
const ports = [
  { id: 'Core-SW-01:Eth1/1', speed: '100G', vlan: 100, color: 'Blue', connected: 'Core-SW-02:Eth1/1', status: 'up' },
  { id: 'Core-SW-01:Eth1/2', speed: '100G', vlan: 100, color: 'Blue', connected: 'Agg-SW-A1:Eth1/49', status: 'up' },
  { id: 'Agg-SW-A1:Eth1/1', speed: '25G', vlan: 200, color: 'Green', connected: 'ToR-A01:Eth1/49', status: 'up' },
  { id: 'Agg-SW-A1:Eth1/2', speed: '25G', vlan: 200, color: 'Green', connected: 'ToR-A02:Eth1/49', status: 'up' },
  { id: 'ToR-A01:Eth1/1', speed: '10G', vlan: 300, color: 'Yellow', connected: 'srv-web-01:eth0', status: 'up' },
  { id: 'ToR-A01:Eth1/5', speed: '10G', vlan: 300, color: 'Yellow', connected: '---', status: 'down' },
  { id: 'ToR-B01:Eth1/48', speed: '10G', vlan: 400, color: 'Red', connected: 'srv-gpu-01:eth0', status: 'up' },
  { id: 'ToR-B01:Eth1/47', speed: '10G', vlan: 400, color: 'Red', connected: '---', status: 'free' },
]
function statusBadge(s: string) { return s === 'up' ? 'bg-success' : s === 'down' ? 'bg-danger' : 'bg-secondary' }
function colorBadge(c: string) { return c === 'Blue' ? 'bg-primary' : c === 'Green' ? 'bg-success' : c === 'Yellow' ? 'bg-warning text-dark' : 'bg-danger' }
</script>

<template>
  <div>
    <h4 class="mb-4">Port Properties</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><div class="table-responsive"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Port</th><th>Speed</th><th>VLAN</th><th>Color</th><th>Connected To</th><th>Status</th></tr></thead>
      <tbody><tr v-for="p in ports" :key="p.id">
        <td class="fw-semibold font-monospace small">{{ p.id }}</td>
        <td><span class="badge bg-secondary">{{ p.speed }}</span></td>
        <td>{{ p.vlan }}</td>
        <td><span class="badge" :class="colorBadge(p.color)">{{ p.color }}</span></td>
        <td class="font-monospace small">{{ p.connected }}</td>
        <td><span class="badge" :class="statusBadge(p.status)">{{ p.status }}</span></td>
      </tr></tbody>
    </table></div></div></div>
  </div>
</template>
