<template>
  <div class="scheduled-reports">
    <div class="card">
      <div class="card-header bg-primary text-white">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Scheduled Reports</h5>
          <button class="btn btn-sm btn-light">Add Schedule</button>
        </div>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover">
            <thead>
              <tr class="table-light">
                <th>Report Name</th>
                <th>Frequency</th>
                <th>Recipients</th>
                <th>Last Sent</th>
                <th>Next Run</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="report in scheduledReports" :key="report.id">
                <td><strong>{{ report.name }}</strong></td>
                <td>
                  <span class="badge bg-info">{{ report.frequency }}</span>
                </td>
                <td>
                  <small>{{ report.recipients }}</small>
                </td>
                <td>{{ report.lastSent }}</td>
                <td>{{ report.nextRun }}</td>
                <td>
                  <span :class="getStatusBadgeClass(report.status)">
                    {{ report.status }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Active schedules: {{ scheduledReports.length }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface ScheduledReport {
  id: string
  name: string
  frequency: string
  recipients: string
  lastSent: string
  nextRun: string
  status: string
}

const scheduledReports: ScheduledReport[] = [
  {
    id: '1',
    name: 'Daily Power Report',
    frequency: 'Daily',
    recipients: 'power-team@company.com',
    lastSent: '2026-02-19 06:00',
    nextRun: '2026-02-20 06:00',
    status: 'Active'
  },
  {
    id: '2',
    name: 'Weekly Cooling Analysis',
    frequency: 'Weekly (Monday)',
    recipients: 'cooling-team@company.com, ops-lead@company.com',
    lastSent: '2026-02-16 09:00',
    nextRun: '2026-02-23 09:00',
    status: 'Active'
  },
  {
    id: '3',
    name: 'Monthly Capacity Report',
    frequency: 'Monthly (1st)',
    recipients: 'management@company.com',
    lastSent: '2026-02-01 08:30',
    nextRun: '2026-03-01 08:30',
    status: 'Active'
  },
  {
    id: '4',
    name: 'Quarterly Compliance Audit',
    frequency: 'Quarterly',
    recipients: 'compliance-officer@company.com, ciso@company.com',
    lastSent: '2026-02-10 10:00',
    nextRun: '2026-05-10 10:00',
    status: 'Active'
  },
  {
    id: '5',
    name: 'Network Performance Report',
    frequency: 'Bi-weekly',
    recipients: 'network-admin@company.com',
    lastSent: '2026-02-15 14:00',
    nextRun: '2026-03-01 14:00',
    status: 'Paused'
  },
  {
    id: '6',
    name: 'Equipment Inventory Sync',
    frequency: 'Monthly (15th)',
    recipients: 'asset-manager@company.com',
    lastSent: '2026-02-15 12:00',
    nextRun: '2026-03-15 12:00',
    status: 'Active'
  }
]

const getStatusBadgeClass = (status: string): string => {
  const classes = 'badge'
  switch (status) {
    case 'Active':
      return `${classes} bg-success`
    case 'Paused':
      return `${classes} bg-warning text-dark`
    case 'Disabled':
      return `${classes} bg-danger`
    default:
      return `${classes} bg-secondary`
  }
}
</script>

<style scoped>
.scheduled-reports {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
