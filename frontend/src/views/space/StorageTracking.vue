<script setup lang="ts">
const systems = [
  { name: 'SAN-01 (NetApp AFF A400)', type: 'SAN', totalTB: 200, usedTB: 142, iops: 85000, throughput: 4.2, status: 'online' },
  { name: 'SAN-02 (NetApp AFF A400)', type: 'SAN', totalTB: 200, usedTB: 168, iops: 92000, throughput: 4.8, status: 'online' },
  { name: 'NAS-01 (Synology)', type: 'NAS', totalTB: 80, usedTB: 58, iops: 12000, throughput: 1.2, status: 'online' },
  { name: 'Backup (Veeam)', type: 'Backup', totalTB: 500, usedTB: 312, iops: 5000, throughput: 2.1, status: 'online' },
  { name: 'Object Store (MinIO)', type: 'Object', totalTB: 100, usedTB: 45, iops: 8000, throughput: 1.8, status: 'online' },
]

const totalCap = systems.reduce((a, s) => a + s.totalTB, 0)
const totalUsed = systems.reduce((a, s) => a + s.usedTB, 0)

function utilColor(pct: number): string {
  if (pct >= 85) return 'danger'
  if (pct >= 70) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Storage Tracking</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
          <div class="card-body"><h3 class="text-primary mb-0">{{ totalCap }} TB</h3><small class="text-muted">Total Capacity</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
          <div class="card-body"><h3 class="text-warning mb-0">{{ totalUsed }} TB</h3><small class="text-muted">Used</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-success bg-opacity-10">
          <div class="card-body"><h3 class="text-success mb-0">{{ totalCap - totalUsed }} TB</h3><small class="text-muted">Available</small></div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
          <div class="card-body"><h3 class="text-info mb-0">{{ (totalUsed / totalCap * 100).toFixed(1) }}%</h3><small class="text-muted">Overall Utilization</small></div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>System</th><th>Type</th><th class="text-end">Total (TB)</th><th class="text-end">Used (TB)</th><th style="width:18%">Utilization</th><th class="text-end">IOPS</th><th class="text-end">Throughput (GB/s)</th><th>Status</th></tr>
          </thead>
          <tbody>
            <tr v-for="s in systems" :key="s.name">
              <td class="fw-semibold">{{ s.name }}</td>
              <td><span class="badge bg-secondary">{{ s.type }}</span></td>
              <td class="text-end">{{ s.totalTB }}</td>
              <td class="text-end">{{ s.usedTB }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="progress flex-fill" style="height: 8px;">
                    <div :class="`progress-bar bg-${utilColor(s.usedTB / s.totalTB * 100)}`" :style="{ width: (s.usedTB / s.totalTB * 100) + '%' }"></div>
                  </div>
                  <small>{{ (s.usedTB / s.totalTB * 100).toFixed(0) }}%</small>
                </div>
              </td>
              <td class="text-end">{{ s.iops.toLocaleString() }}</td>
              <td class="text-end">{{ s.throughput.toFixed(1) }}</td>
              <td><span class="badge bg-success">{{ s.status }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
