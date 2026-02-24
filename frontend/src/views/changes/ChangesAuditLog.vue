<template>
  <div class="audit-log">
    <div class="card">
      <div class="card-header bg-secondary text-white">
        <h5 class="mb-0">Change Audit Log</h5>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-sm table-hover">
            <thead>
              <tr class="table-light">
                <th>Log ID</th>
                <th>Change Request</th>
                <th>Action</th>
                <th>User</th>
                <th>Timestamp</th>
                <th>Details</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="log in auditLogs" :key="log.id">
                <td><small>{{ log.id }}</small></td>
                <td><strong>{{ log.changeRequest }}</strong></td>
                <td>
                  <span :class="getActionBadgeClass(log.action)">
                    {{ log.action }}
                  </span>
                </td>
                <td>{{ log.user }}</td>
                <td><small>{{ log.timestamp }}</small></td>
                <td>{{ log.details }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Total audit entries: {{ auditLogs.length }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface AuditLog {
  id: string
  changeRequest: string
  action: string
  user: string
  timestamp: string
  details: string
}

const auditLogs: AuditLog[] = [
  {
    id: 'LOG-2895',
    changeRequest: 'CHG-005',
    action: 'Created',
    user: 'David Brown',
    timestamp: '2026-02-14 09:15:00',
    details: 'Emergency hardware replacement initiated'
  },
  {
    id: 'LOG-2894',
    changeRequest: 'CHG-005',
    action: 'Approved',
    user: 'Manager Admin',
    timestamp: '2026-02-14 09:45:00',
    details: 'Expedited approval granted'
  },
  {
    id: 'LOG-2893',
    changeRequest: 'CHG-004',
    action: 'Created',
    user: 'Emma Wilson',
    timestamp: '2026-02-15 10:30:00',
    details: 'Access control policy update'
  },
  {
    id: 'LOG-2892',
    changeRequest: 'CHG-003',
    action: 'Updated',
    user: 'Mike Chen',
    timestamp: '2026-02-16 14:20:00',
    details: 'Software version changed to v2.1.5'
  },
  {
    id: 'LOG-2891',
    changeRequest: 'CHG-002',
    action: 'Approved',
    user: 'Senior Admin',
    timestamp: '2026-02-17 11:00:00',
    details: 'Network routing configuration approved'
  },
  {
    id: 'LOG-2890',
    changeRequest: 'CHG-001',
    action: 'Created',
    user: 'John Smith',
    timestamp: '2026-02-18 08:45:00',
    details: 'Server hardware upgrade request'
  },
  {
    id: 'LOG-2889',
    changeRequest: 'CHG-201',
    action: 'Completed',
    user: 'Jessica White',
    timestamp: '2026-02-12 16:30:00',
    details: 'Work order completed successfully'
  },
  {
    id: 'LOG-2888',
    changeRequest: 'CHG-201',
    action: 'Verified',
    user: 'QA Lead',
    timestamp: '2026-02-12 17:00:00',
    details: 'Post-implementation verification passed'
  }
]

const getActionBadgeClass = (action: string): string => {
  const classes = 'badge'
  switch (action) {
    case 'Created':
      return `${classes} bg-info`
    case 'Updated':
      return `${classes} bg-primary`
    case 'Approved':
      return `${classes} bg-success`
    case 'Rejected':
      return `${classes} bg-danger`
    case 'Completed':
      return `${classes} bg-success`
    case 'Verified':
      return `${classes} bg-primary`
    default:
      return `${classes} bg-secondary`
  }
}
</script>

<style scoped>
.audit-log {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
