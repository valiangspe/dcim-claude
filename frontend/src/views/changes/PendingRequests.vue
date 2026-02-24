<template>
  <div class="pending-requests">
    <div class="card">
      <div class="card-header bg-primary text-white">
        <h5 class="mb-0">Pending Change Requests</h5>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover">
            <thead>
              <tr class="table-light">
                <th>Request ID</th>
                <th>Requester</th>
                <th>Type</th>
                <th>Priority</th>
                <th>Status</th>
                <th>Requested Date</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="req in pendingRequests" :key="req.id">
                <td><strong>{{ req.id }}</strong></td>
                <td>{{ req.requester }}</td>
                <td>{{ req.type }}</td>
                <td>
                  <span :class="getPriorityBadgeClass(req.priority)">
                    {{ req.priority }}
                  </span>
                </td>
                <td>
                  <span :class="getStatusBadgeClass(req.status)">
                    {{ req.status }}
                  </span>
                </td>
                <td>{{ req.requestedDate }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Total pending: {{ pendingRequests.length }} requests
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface PendingRequest {
  id: string
  requester: string
  type: string
  priority: string
  status: string
  requestedDate: string
}

const pendingRequests: PendingRequest[] = [
  {
    id: 'CHG-001',
    requester: 'John Smith',
    type: 'Hardware',
    priority: 'High',
    status: 'Pending Review',
    requestedDate: '2026-02-18'
  },
  {
    id: 'CHG-002',
    requester: 'Sarah Johnson',
    type: 'Network',
    priority: 'Medium',
    status: 'Waiting Approval',
    requestedDate: '2026-02-17'
  },
  {
    id: 'CHG-003',
    requester: 'Mike Chen',
    type: 'Software',
    priority: 'Low',
    status: 'Pending Review',
    requestedDate: '2026-02-16'
  },
  {
    id: 'CHG-004',
    requester: 'Emma Wilson',
    type: 'Access',
    priority: 'High',
    status: 'Waiting Approval',
    requestedDate: '2026-02-15'
  },
  {
    id: 'CHG-005',
    requester: 'David Brown',
    type: 'Emergency',
    priority: 'Critical',
    status: 'In Progress',
    requestedDate: '2026-02-14'
  }
]

const getPriorityBadgeClass = (priority: string): string => {
  const classes = 'badge'
  switch (priority) {
    case 'Critical':
      return `${classes} bg-danger`
    case 'High':
      return `${classes} bg-warning text-dark`
    case 'Medium':
      return `${classes} bg-info`
    case 'Low':
      return `${classes} bg-success`
    default:
      return `${classes} bg-secondary`
  }
}

const getStatusBadgeClass = (status: string): string => {
  const classes = 'badge'
  switch (status) {
    case 'Pending Review':
      return `${classes} bg-secondary`
    case 'Waiting Approval':
      return `${classes} bg-warning text-dark`
    case 'In Progress':
      return `${classes} bg-info`
    case 'Completed':
      return `${classes} bg-success`
    default:
      return `${classes} bg-light text-dark`
  }
}
</script>

<style scoped>
.pending-requests {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
