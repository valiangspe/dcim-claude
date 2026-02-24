<template>
  <div class="container-fluid">
    <h1 class="mb-4">Active Alarms</h1>

    <!-- Severity Summary -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card border-danger">
          <div class="card-body">
            <h6 class="card-title text-danger">Critical</h6>
            <p class="h4 mb-0">3</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-warning">
          <div class="card-body">
            <h6 class="card-title text-warning">High</h6>
            <p class="h4 mb-0">7</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-info">
          <div class="card-body">
            <h6 class="card-title text-info">Medium</h6>
            <p class="h4 mb-0">12</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-secondary">
          <div class="card-body">
            <h6 class="card-title text-secondary">Low</h6>
            <p class="h4 mb-0">5</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Active Alarms Table -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Active Alarms</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Time</th>
              <th>Source</th>
              <th>Severity</th>
              <th>Message</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="alarm in activeAlarms" :key="alarm.id">
              <td class="text-nowrap">{{ formatTime(alarm.time) }}</td>
              <td>{{ alarm.source }}</td>
              <td>
                <span :class="getSeverityBadgeClass(alarm.severity)">
                  {{ alarm.severity }}
                </span>
              </td>
              <td>{{ alarm.message }}</td>
              <td>
                <button class="btn btn-sm btn-outline-primary">Acknowledge</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Alarm {
  id: number
  time: Date
  source: string
  severity: 'Critical' | 'High' | 'Medium' | 'Low'
  message: string
}

const activeAlarms: Alarm[] = [
  {
    id: 1,
    time: new Date(Date.now() - 5 * 60000),
    source: 'Server Rack A-01',
    severity: 'Critical',
    message: 'Temperature threshold exceeded: 42°C',
  },
  {
    id: 2,
    time: new Date(Date.now() - 15 * 60000),
    source: 'PDU-B-03',
    severity: 'Critical',
    message: 'Power consumption anomaly detected',
  },
  {
    id: 3,
    time: new Date(Date.now() - 25 * 60000),
    source: 'Cooling Unit C-02',
    severity: 'High',
    message: 'Refrigerant pressure low',
  },
  {
    id: 4,
    time: new Date(Date.now() - 45 * 60000),
    source: 'Network Switch D-01',
    severity: 'High',
    message: 'Port utilization above 85%',
  },
  {
    id: 5,
    time: new Date(Date.now() - 75 * 60000),
    source: 'UPS Battery Bank',
    severity: 'Medium',
    message: 'Battery health degradation detected',
  },
]

function formatTime(date: Date): string {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)

  if (diffMins < 60) return `${diffMins}m ago`
  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours}h ago`
  return date.toLocaleDateString()
}

function getSeverityBadgeClass(severity: string): string {
  const classes: Record<string, string> = {
    Critical: 'badge bg-danger',
    High: 'badge bg-warning text-dark',
    Medium: 'badge bg-info',
    Low: 'badge bg-secondary',
  }
  return classes[severity] || 'badge bg-secondary'
}
</script>
