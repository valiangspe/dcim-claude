<script setup lang="ts">
const classes = [
  { cls: 'Servers', installed: 124, planned: 12, archived: 8, total: 144 },
  { cls: 'Network Switches', installed: 32, planned: 4, archived: 2, total: 38 },
  { cls: 'Storage Arrays', installed: 8, planned: 2, archived: 1, total: 11 },
  { cls: 'PDUs', installed: 24, planned: 2, archived: 0, total: 26 },
  { cls: 'UPS Units', installed: 5, planned: 1, archived: 0, total: 6 },
  { cls: 'Patch Panels', installed: 18, planned: 0, archived: 2, total: 20 },
  { cls: 'Cooling Units', installed: 7, planned: 1, archived: 0, total: 8 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Inventory</h4>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Class</th><th class="text-end">Installed</th><th class="text-end">Planned</th><th class="text-end">Archived</th><th class="text-end">Total</th></tr></thead>
      <tbody><tr v-for="c in classes" :key="c.cls">
        <td class="fw-semibold">{{ c.cls }}</td><td class="text-end text-success">{{ c.installed }}</td><td class="text-end text-warning">{{ c.planned }}</td><td class="text-end text-muted">{{ c.archived }}</td><td class="text-end fw-semibold">{{ c.total }}</td>
      </tr></tbody>
      <tfoot class="table-light"><tr class="fw-semibold"><td>Total</td><td class="text-end">{{ classes.reduce((a,c)=>a+c.installed,0) }}</td><td class="text-end">{{ classes.reduce((a,c)=>a+c.planned,0) }}</td><td class="text-end">{{ classes.reduce((a,c)=>a+c.archived,0) }}</td><td class="text-end">{{ classes.reduce((a,c)=>a+c.total,0) }}</td></tr></tfoot>
    </table></div></div>
  </div>
</template>
