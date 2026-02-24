<template>
  <div class="container-fluid">
    <h1 class="mb-4">Alarm History</h1>

    <!-- Filters -->
    <div class="card mb-4">
      <div class="card-body">
        <div class="row">
          <div class="col-md-3">
            <label class="form-label">Severity</label>
            <select v-model="filterSeverity" class="form-select">
              <option value="">All</option>
              <option value="Critical">Critical</option>
              <option value="High">High</option>
              <option value="Medium">Medium</option>
              <option value="Low">Low</option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">Time Range</label>
            <select v-model="timeRange" class="form-select">
              <option value="24h">Last 24 Hours</option>
              <option value="7d">Last 7 Days</option>
              <option value="30d">Last 30 Days</option>
              <option value="all">All Time</option>
            </select>
          </div>
          <div class="col-md-6 d-flex align-items-end">
            <button class="btn btn-primary">Search</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Historical Alarms Table -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Resolved Alarms</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-sm mb-0">
          <thead class="table-light">
            <tr>
              <th>Triggered</th>
              <th>Resolved</th>
              <th>Duration</th>
              <th>Source</th>
              <th>Severity</th>
              <th>Message</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="alarm in historicalAlarms" :key="alarm.id">
              <td class="text-nowrap">{{ formatDateTime(alarm.triggered) }}</td>
              <td class="text-nowrap">{{ formatDateTime(alarm.resolved) }}</td>
              <td class="text-nowrap">{{ calculateDuration(alarm.triggered, alarm.resolved) }}</td>
              <td>{{ alarm.source }}</td>
              <td>
                <span :class="getSeverityBadgeClass(alarm.severity)">
                  {{ alarm.severity }}
                </span>
              </td>
              <td>{{ alarm.message }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface HistoricalAlarm {
  id: number
  triggered: Date
  resolved: Date
  source: string
  severity: 'Critical' | 'High' | 'Medium' | 'Low'
  message: string
}

const filterSeverity = ref('')
const timeRange = ref('24h')

const historicalAlarms: HistoricalAlarm[] = [
  {
    id: 1,
    triggered: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    resolved: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000 + 45 * 60 * 1000),
    source: 'Server Rack A-02',
    severity: 'High',
    message: 'Memory threshold exceeded',
  },
  {
    id: 2,
    triggered: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
    resolved: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000 + 2 * 60 * 60 * 1000),
    source: 'Cooling Unit B-01',
    severity: 'Medium',
    message: 'Filter replacement due',
  },
  {
    id: 3,
    triggered: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
    resolved: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000 + 30 * 60 * 1000),
    source: 'PDU-C-02',
    severity: 'Low',
    message: 'Firmware update available',
  },
  {
    id: 4,
    triggered: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
    resolved: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000 + 90 * 60 * 1000),
    source: 'Network Switch E-03',
    severity: 'Critical',
    message: 'Link down detected',
  },
  {
    id: 5,
    triggered: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
    resolved: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000 + 15 * 60 * 1000),
    source: 'UPS Battery Bank',
    severity: 'High',
    message: 'Self-test failure',
  },
]

function formatDateTime(date: Date): string {
  return date.toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function calculateDuration(start: Date, end: Date): string {
  const diffMs = end.getTime() - start.getTime()
  const diffMins = Math.floor(diffMs / 60000)

  if (diffMins < 60) return `${diffMins}m`
  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours}h ${diffMins % 60}m`
  const diffDays = Math.floor(diffHours / 24)
  return `${diffDays}d ${diffHours % 24}h`
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
