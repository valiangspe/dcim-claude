<script setup lang="ts">
const statuses = [
  { status: 'Installed', count: 218, pct: 86.5, color: 'success' },
  { status: 'Planned', count: 22, pct: 8.7, color: 'warning' },
  { status: 'Archived', count: 13, pct: 5.2, color: 'secondary' },
  { status: 'In Transit', count: 4, pct: 1.6, color: 'info' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Assets by Status</h4>
    <div class="row g-3 mb-4">
      <div v-for="s in statuses" :key="s.status" class="col-sm-6 col-lg-3">
        <div class="card border-0 shadow-sm text-center h-100"><div class="card-body">
          <h2 :class="`text-${s.color} mb-0`">{{ s.count }}</h2>
          <div class="progress my-2" style="height:6px"><div :class="`progress-bar bg-${s.color}`" :style="{width:s.pct+'%'}"></div></div>
          <small class="text-muted">{{ s.status }} ({{ s.pct }}%)</small>
        </div></div>
      </div>
    </div>
  </div>
</template>
