<template>
  <div class="completed-requests">
    <div class="card">
      <div class="card-header bg-success text-white">
        <h5 class="mb-0">Completed Change Requests</h5>
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
                <th>Requested Date</th>
                <th>Completion Date</th>
                <th>Duration (Days)</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="req in completedRequests" :key="req.id">
                <td><strong>{{ req.id }}</strong></td>
                <td>{{ req.requester }}</td>
                <td>{{ req.type }}</td>
                <td>
                  <span :class="getPriorityBadgeClass(req.priority)">
                    {{ req.priority }}
                  </span>
                </td>
                <td>{{ req.requestedDate }}</td>
                <td>{{ req.completionDate }}</td>
                <td>
                  <span class="badge bg-info">{{ req.duration }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Total completed: {{ completedRequests.length }} requests
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface CompletedRequest {
  id: string
  requester: string
  type: string
  priority: string
  requestedDate: string
  completionDate: string
  duration: number
}

const completedRequests: CompletedRequest[] = [
  {
    id: 'CHG-201',
    requester: 'Alice Brown',
    type: 'Hardware',
    priority: 'High',
    requestedDate: '2026-02-10',
    completionDate: '2026-02-12',
    duration: 2
  },
  {
    id: 'CHG-202',
    requester: 'Bob Davis',
    type: 'Network',
    priority: 'Medium',
    requestedDate: '2026-02-05',
    completionDate: '2026-02-08',
    duration: 3
  },
  {
    id: 'CHG-203',
    requester: 'Carol Martinez',
    type: 'Software',
    priority: 'Low',
    requestedDate: '2026-02-01',
    completionDate: '2026-02-04',
    duration: 3
  },
  {
    id: 'CHG-204',
    requester: 'Diana Lee',
    type: 'Access',
    priority: 'High',
    requestedDate: '2026-01-28',
    completionDate: '2026-01-30',
    duration: 2
  },
  {
    id: 'CHG-205',
    requester: 'Frank Wilson',
    type: 'Hardware',
    priority: 'Critical',
    requestedDate: '2026-01-20',
    completionDate: '2026-01-21',
    duration: 1
  },
  {
    id: 'CHG-206',
    requester: 'Grace Taylor',
    type: 'Software',
    priority: 'Medium',
    requestedDate: '2026-01-15',
    completionDate: '2026-01-18',
    duration: 3
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
</script>

<style scoped>
.completed-requests {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
