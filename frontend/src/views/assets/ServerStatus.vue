<script setup lang="ts">
const servers = [
  { name: 'srv-web-01', rack: 'CAB-A01', os: 'Ubuntu 22.04', uptime: '142d 8h', status: 'online' },
  { name: 'srv-web-02', rack: 'CAB-A01', os: 'Ubuntu 22.04', uptime: '142d 8h', status: 'online' },
  { name: 'srv-db-01', rack: 'CAB-A02', os: 'RHEL 9', uptime: '98d 12h', status: 'online' },
  { name: 'srv-db-02', rack: 'CAB-A02', os: 'RHEL 9', uptime: '98d 12h', status: 'online' },
  { name: 'srv-gpu-01', rack: 'CAB-B02', os: 'Ubuntu 22.04', uptime: '45d 3h', status: 'online' },
  { name: 'srv-gpu-02', rack: 'CAB-B02', os: 'Ubuntu 22.04', uptime: '45d 3h', status: 'online' },
  { name: 'srv-app-01', rack: 'CAB-B01', os: 'Debian 12', uptime: '210d 6h', status: 'online' },
  { name: 'srv-bkup-01', rack: 'CAB-C02', os: 'FreeBSD 14', uptime: '0d', status: 'offline' },
  { name: 'srv-test-01', rack: 'CAB-D01', os: 'Ubuntu 24.04', uptime: '0d', status: 'maintenance' },
]
const counts = { online: servers.filter(s=>s.status==='online').length, offline: servers.filter(s=>s.status==='offline').length, maintenance: servers.filter(s=>s.status==='maintenance').length }
</script>

<template>
  <div>
    <h4 class="mb-4">Server Status</h4>
    <div class="row g-3 mb-4">
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-success bg-opacity-10"><div class="card-body"><h3 class="text-success mb-0">{{ counts.online }}</h3><small class="text-muted">Online</small></div></div></div>
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-danger bg-opacity-10"><div class="card-body"><h3 class="text-danger mb-0">{{ counts.offline }}</h3><small class="text-muted">Offline</small></div></div></div>
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10"><div class="card-body"><h3 class="text-warning mb-0">{{ counts.maintenance }}</h3><small class="text-muted">Maintenance</small></div></div></div>
    </div>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Server</th><th>Rack</th><th>OS</th><th>Uptime</th><th>Status</th></tr></thead>
      <tbody><tr v-for="s in servers" :key="s.name">
        <td class="fw-semibold font-monospace">{{ s.name }}</td><td>{{ s.rack }}</td><td>{{ s.os }}</td><td>{{ s.uptime }}</td>
        <td><span class="badge" :class="{'bg-success':s.status==='online','bg-danger':s.status==='offline','bg-warning text-dark':s.status==='maintenance'}">{{ s.status }}</span></td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
