<script setup lang="ts">
const kpis = [
  { label: 'Security Incidents', value: '12', color: 'danger', trend: '+3 this month' },
  { label: 'Data Breaches', value: '0', color: 'success', trend: 'No incidents' },
  { label: 'Access Events', value: '8,247', color: 'primary', trend: '+324 today' },
  { label: 'Failed Auth', value: '34', color: 'warning', trend: '+8 today' },
  { label: 'Active Policies', value: '47', color: 'info', trend: '5 pending review' },
  { label: 'Compliance Score', value: '94%', color: 'success', trend: '+2% last week' },
]

const incidents = [
  { id: 1, type: 'Unauthorized Access Attempt', timestamp: '2026-02-20 14:32', location: 'Data Center A', severity: 'high', status: 'resolved' },
  { id: 2, type: 'Configuration Change', timestamp: '2026-02-20 13:15', location: 'Security Control Room', severity: 'medium', status: 'investigating' },
  { id: 3, type: 'Access Control Bypass', timestamp: '2026-02-20 11:42', location: 'Server Room B', severity: 'critical', status: 'active' },
  { id: 4, type: 'Policy Violation', timestamp: '2026-02-19 16:28', location: 'Network Operations', severity: 'low', status: 'resolved' },
  { id: 5, type: 'Unusual Activity Pattern', timestamp: '2026-02-19 09:10', location: 'Data Center C', severity: 'medium', status: 'resolved' },
]

const severityColor = {
  critical: 'danger',
  high: 'warning',
  medium: 'info',
  low: 'secondary',
} as const

const statusBadge = {
  active: 'danger',
  investigating: 'warning',
  resolved: 'success',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">Security Overview</h4>

    <!-- KPI Cards -->
    <div class="row g-3 mb-4">
      <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg-4 col-xl-2">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body">
            <h3 :class="`text-${kpi.color} mb-1`">{{ kpi.value }}</h3>
            <small class="text-muted d-block">{{ kpi.label }}</small>
            <small class="text-secondary">{{ kpi.trend }}</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Incidents -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center">
        <span>Recent Security Incidents</span>
        <span class="badge bg-danger ms-2">{{ incidents.length }}</span>
      </div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Type</th>
              <th>Location</th>
              <th>Timestamp</th>
              <th>Severity</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="incident in incidents" :key="incident.id">
              <td class="fw-semibold">{{ incident.type }}</td>
              <td>{{ incident.location }}</td>
              <td class="text-muted">{{ incident.timestamp }}</td>
              <td><span class="badge" :class="`bg-${severityColor[incident.severity]}`">{{ incident.severity }}</span></td>
              <td><span class="badge" :class="`bg-${statusBadge[incident.status]}`">{{ incident.status }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
