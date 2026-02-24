<script setup lang="ts">
const items = [
  { name: 'APC Symmetra PX #3', customer: 'Infrastructure', purchased: '2020-06-15', age: '5.7 yr', warranty: 'Expired', status: 'warning' },
  { name: 'Dell R630 Cluster (x8)', customer: 'App Team', purchased: '2021-03-22', age: '4.9 yr', warranty: 'Expired', status: 'warning' },
  { name: 'Cisco Nexus 9332C #1', customer: 'Network', purchased: '2022-01-10', age: '4.1 yr', warranty: 'Active', status: 'ok' },
  { name: 'NetApp AFF A400 #1', customer: 'Storage', purchased: '2022-09-05', age: '3.5 yr', warranty: 'Active', status: 'ok' },
  { name: 'Dell R750 Batch A (x24)', customer: 'Web Team', purchased: '2023-11-20', age: '2.3 yr', warranty: 'Active', status: 'ok' },
  { name: 'NVIDIA DGX A100 (x8)', customer: 'ML Team', purchased: '2024-06-01', age: '1.7 yr', warranty: 'Active', status: 'ok' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Item Age Tracking</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Item</th><th>Customer / Team</th><th>Purchased</th><th class="text-end">Age</th><th>Warranty</th></tr></thead>
      <tbody><tr v-for="i in items" :key="i.name" :class="i.status==='warning'?'table-warning':''">
        <td class="fw-semibold">{{ i.name }}</td><td>{{ i.customer }}</td><td>{{ i.purchased }}</td><td class="text-end">{{ i.age }}</td>
        <td><span class="badge" :class="i.warranty==='Active'?'bg-success':'bg-danger'">{{ i.warranty }}</span></td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
