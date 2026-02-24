<script setup lang="ts">
const sensors = [
  { id: 'TEMP-A01-IN', type: 'Temperature', location: 'CAB-A01 Inlet', model: 'APC AP9335T', calibrated: '2026-01-10', interval: 30, status: 'active' },
  { id: 'TEMP-A01-OUT', type: 'Temperature', location: 'CAB-A01 Outlet', model: 'APC AP9335T', calibrated: '2026-01-10', interval: 30, status: 'active' },
  { id: 'TEMP-B01-IN', type: 'Temperature', location: 'CAB-B01 Inlet', model: 'APC AP9335T', calibrated: '2025-12-15', interval: 30, status: 'active' },
  { id: 'TEMP-B02-IN', type: 'Temperature', location: 'CAB-B02 Inlet', model: 'APC AP9335T', calibrated: '2025-12-15', interval: 30, status: 'active' },
  { id: 'HUM-A01', type: 'Humidity', location: 'CAB-A01', model: 'APC AP9335TH', calibrated: '2026-01-10', interval: 60, status: 'active' },
  { id: 'HUM-B02', type: 'Humidity', location: 'CAB-B02', model: 'APC AP9335TH', calibrated: '2025-11-20', interval: 60, status: 'warning' },
  { id: 'FLOW-CRAH-A1', type: 'Airflow', location: 'CRAH-A1', model: 'Dwyer Series', calibrated: '2025-12-01', interval: 60, status: 'active' },
  { id: 'LEAK-HALL-A', type: 'Leak Detection', location: 'Hall A Floor', model: 'TraceTek TT5000', calibrated: '2026-02-01', interval: 10, status: 'active' },
  { id: 'PRESS-HALL-B', type: 'Pressure', location: 'Hall B Sub-floor', model: 'Setra 264', calibrated: '2025-10-15', interval: 60, status: 'inactive' },
]

const typeCounts = [
  { type: 'Temperature', count: 4 },
  { type: 'Humidity', count: 2 },
  { type: 'Airflow', count: 1 },
  { type: 'Leak Detection', count: 1 },
  { type: 'Pressure', count: 1 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Sensor Management</h4>

    <div class="row g-3 mb-4">
      <div v-for="tc in typeCounts" :key="tc.type" class="col">
        <div class="card border-0 shadow-sm text-center h-100">
          <div class="card-body py-3">
            <h4 class="text-primary mb-0">{{ tc.count }}</h4>
            <small class="text-muted">{{ tc.type }}</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
        <span class="fw-semibold">Registered Sensors</span>
        <button class="btn btn-primary btn-sm">+ Add Sensor</button>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>ID</th><th>Type</th><th>Location</th><th>Model</th><th>Calibrated</th><th class="text-end">Interval (s)</th><th>Status</th></tr>
            </thead>
            <tbody>
              <tr v-for="s in sensors" :key="s.id">
                <td class="fw-semibold font-monospace small">{{ s.id }}</td>
                <td>{{ s.type }}</td>
                <td class="text-muted">{{ s.location }}</td>
                <td class="small">{{ s.model }}</td>
                <td>{{ s.calibrated }}</td>
                <td class="text-end">{{ s.interval }}</td>
                <td>
                  <span class="badge" :class="{ 'bg-success': s.status === 'active', 'bg-warning text-dark': s.status === 'warning', 'bg-secondary': s.status === 'inactive' }">{{ s.status }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
