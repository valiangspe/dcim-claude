<script setup lang="ts">
const history = [
  { date: '2026-02-18', type: 'Import', file: 'servers_batch_c.csv', records: 12, status: 'success' },
  { date: '2026-02-15', type: 'Export', file: 'full_inventory.csv', records: 253, status: 'success' },
  { date: '2026-02-10', type: 'Import', file: 'network_switches.csv', records: 4, status: 'success' },
  { date: '2026-02-05', type: 'Import', file: 'parts_restock.csv', records: 48, status: 'partial' },
  { date: '2026-01-28', type: 'Export', file: 'assets_by_hall.csv', records: 186, status: 'success' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Bulk Import / Export</h4>
    <div class="row g-3 mb-4">
      <div class="col-md-6"><div class="card border-0 shadow-sm h-100"><div class="card-body text-center">
        <h5>Import CSV</h5><p class="text-muted small">Upload a CSV file to bulk-import assets, parts, or configuration.</p>
        <button class="btn btn-primary">Choose File & Import</button>
      </div></div></div>
      <div class="col-md-6"><div class="card border-0 shadow-sm h-100"><div class="card-body text-center">
        <h5>Export CSV</h5><p class="text-muted small">Export current inventory, assets, or parts data to CSV.</p>
        <button class="btn btn-outline-primary">Export All Assets</button>
      </div></div></div>
    </div>
    <div class="card border-0 shadow-sm"><div class="card-header bg-transparent fw-semibold">Recent Operations</div><div class="card-body p-0"><table class="table table-hover mb-0">
      <thead class="table-light"><tr><th>Date</th><th>Type</th><th>File</th><th class="text-end">Records</th><th>Status</th></tr></thead>
      <tbody><tr v-for="h in history" :key="h.date+h.file"><td>{{ h.date }}</td><td><span class="badge" :class="h.type==='Import'?'bg-primary':'bg-info text-dark'">{{ h.type }}</span></td>
        <td class="font-monospace small">{{ h.file }}</td><td class="text-end">{{ h.records }}</td>
        <td><span class="badge" :class="h.status==='success'?'bg-success':'bg-warning text-dark'">{{ h.status }}</span></td>
      </tr></tbody>
    </table></div></div>
  </div>
</template>
