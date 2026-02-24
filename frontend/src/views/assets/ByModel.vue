<script setup lang="ts">
const models = [
  { model: 'Dell PowerEdge R750', vendor: 'Dell', category: 'Server', count: 48, avgAge: '1.2 yr' },
  { model: 'Dell PowerEdge R650', vendor: 'Dell', category: 'Server', count: 36, avgAge: '2.1 yr' },
  { model: 'HPE ProLiant DL380', vendor: 'HPE', category: 'Server', count: 24, avgAge: '1.8 yr' },
  { model: 'NVIDIA DGX A100', vendor: 'NVIDIA', category: 'GPU Server', count: 8, avgAge: '0.8 yr' },
  { model: 'Cisco Nexus 9364C', vendor: 'Cisco', category: 'Switch', count: 12, avgAge: '1.5 yr' },
  { model: 'Arista 7280R3', vendor: 'Arista', category: 'Switch', count: 20, avgAge: '1.0 yr' },
  { model: 'NetApp AFF A400', vendor: 'NetApp', category: 'Storage', count: 4, avgAge: '1.4 yr' },
  { model: 'APC Symmetra PX', vendor: 'APC', category: 'UPS', count: 5, avgAge: '3.2 yr' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Assets by Model</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Model</th><th>Vendor</th><th>Category</th><th class="text-end">Count</th><th class="text-end">Avg Age</th></tr></thead>
      <tbody><tr v-for="m in models" :key="m.model">
        <td class="fw-semibold">{{ m.model }}</td><td>{{ m.vendor }}</td><td><span class="badge bg-secondary">{{ m.category }}</span></td><td class="text-end">{{ m.count }}</td><td class="text-end">{{ m.avgAge }}</td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
