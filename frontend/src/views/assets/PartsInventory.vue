<script setup lang="ts">
const parts = [
  { name: '32GB DDR5 RDIMM', category: 'Memory', inStock: 48, reserved: 12, reorder: 20, unit: '$142' },
  { name: '1.92TB NVMe SSD', category: 'Storage', inStock: 24, reserved: 8, reorder: 10, unit: '$285' },
  { name: '25G SFP28 Transceiver', category: 'Network', inStock: 64, reserved: 10, reorder: 30, unit: '$38' },
  { name: '100G QSFP28 Transceiver', category: 'Network', inStock: 16, reserved: 4, reorder: 8, unit: '$180' },
  { name: '750W PSU', category: 'Power', inStock: 12, reserved: 2, reorder: 6, unit: '$95' },
  { name: '1200W PSU', category: 'Power', inStock: 8, reserved: 4, reorder: 6, unit: '$165' },
  { name: 'Cat6A Patch Cable 2m', category: 'Cabling', inStock: 200, reserved: 0, reorder: 50, unit: '$8' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Parts Inventory</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Part</th><th>Category</th><th class="text-end">In Stock</th><th class="text-end">Reserved</th><th class="text-end">Available</th><th class="text-end">Reorder Level</th><th class="text-end">Unit Cost</th><th>Status</th></tr></thead>
      <tbody><tr v-for="p in parts" :key="p.name">
        <td class="fw-semibold">{{ p.name }}</td><td><span class="badge bg-secondary">{{ p.category }}</span></td>
        <td class="text-end">{{ p.inStock }}</td><td class="text-end">{{ p.reserved }}</td><td class="text-end">{{ p.inStock-p.reserved }}</td><td class="text-end">{{ p.reorder }}</td><td class="text-end">{{ p.unit }}</td>
        <td><span class="badge" :class="(p.inStock-p.reserved)<=p.reorder?'bg-danger':'bg-success'">{{ (p.inStock-p.reserved)<=p.reorder?'LOW':'OK' }}</span></td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
