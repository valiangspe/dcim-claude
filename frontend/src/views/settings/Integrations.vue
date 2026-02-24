<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">BMS/SCADA Integrations</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm">+ Add Integration</button>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>System Name</th>
              <th>Type</th>
              <th>Endpoint</th>
              <th>Status</th>
              <th>Last Sync</th>
              <th>Metrics</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="integration in integrations" :key="integration.id">
              <td class="fw-medium">{{ integration.name }}</td>
              <td>
                <span class="badge bg-info">{{ integration.type }}</span>
              </td>
              <td class="text-muted small">{{ integration.endpoint }}</td>
              <td>
                <span :class="['badge', integration.status === 'Connected' ? 'bg-success' : integration.status === 'Syncing' ? 'bg-primary' : 'bg-danger']">
                  {{ integration.status }}
                </span>
              </td>
              <td class="text-muted">{{ integration.lastSync }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ integration.metrics }}</span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-secondary">Config</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const integrations = [
  {
    id: 1,
    name: 'Building Management System',
    type: 'BMS',
    endpoint: 'bms-primary.datacenter.local:9000',
    status: 'Connected',
    lastSync: '2026-02-20 14:35:12 UTC',
    metrics: 47
  },
  {
    id: 2,
    name: 'HVAC Control System',
    type: 'SCADA',
    endpoint: 'hvac-scada-01.datacenter.local:502',
    status: 'Connected',
    lastSync: '2026-02-20 14:34:58 UTC',
    metrics: 23
  },
  {
    id: 3,
    name: 'Power Distribution Unit',
    type: 'MODBUS',
    endpoint: '10.0.1.45:502',
    status: 'Connected',
    lastSync: '2026-02-20 14:35:05 UTC',
    metrics: 156
  },
  {
    id: 4,
    name: 'Access Control System',
    type: 'REST API',
    endpoint: 'access.datacenter.local/api',
    status: 'Syncing',
    lastSync: '2026-02-20 14:35:00 UTC',
    metrics: 12
  },
  {
    id: 5,
    name: 'Environmental Sensor Network',
    type: 'MQTT',
    endpoint: 'mqtt.datacenter.local:1883',
    status: 'Connected',
    lastSync: '2026-02-20 14:34:55 UTC',
    metrics: 89
  }
];
</script>
