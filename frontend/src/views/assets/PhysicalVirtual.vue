<script setup lang="ts">
const summary = [
  { label: 'Physical Servers', count: 124, color: 'primary' },
  { label: 'Virtual Machines', count: 486, color: 'info' },
  { label: 'Containers', count: 1247, color: 'success' },
  { label: 'VM-to-Physical Ratio', count: '3.9:1', color: 'dark' },
]
const hosts = [
  { name: 'esxi-01', type: 'VMware ESXi', physical: true, vms: 18, vcpus: 72, vmem: 384 },
  { name: 'esxi-02', type: 'VMware ESXi', physical: true, vms: 16, vcpus: 64, vmem: 384 },
  { name: 'kvm-01', type: 'KVM/QEMU', physical: true, vms: 12, vcpus: 48, vmem: 256 },
  { name: 'hyper-01', type: 'Hyper-V', physical: true, vms: 22, vcpus: 88, vmem: 512 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Physical / Virtual</h4>
    <div class="row g-3 mb-4">
      <div v-for="s in summary" :key="s.label" class="col-sm-6 col-lg-3">
        <div class="card border-0 shadow-sm text-center h-100"><div class="card-body"><h3 :class="`text-${s.color} mb-0`">{{ s.count }}</h3><small class="text-muted">{{ s.label }}</small></div></div>
      </div>
    </div>
    <div class="card border-0 shadow-sm"><div class="card-header bg-transparent fw-semibold">Hypervisor Hosts</div><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Host</th><th>Hypervisor</th><th class="text-end">VMs</th><th class="text-end">vCPUs</th><th class="text-end">vMem (GB)</th></tr></thead>
      <tbody><tr v-for="h in hosts" :key="h.name"><td class="fw-semibold font-monospace">{{ h.name }}</td><td>{{ h.type }}</td><td class="text-end">{{ h.vms }}</td><td class="text-end">{{ h.vcpus }}</td><td class="text-end">{{ h.vmem }}</td></tr></tbody>
    </table></div></div>
  </div>
</template>
