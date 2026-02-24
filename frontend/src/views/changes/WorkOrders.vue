<template>
  <div class="work-orders">
    <div class="card">
      <div class="card-header bg-primary text-white">
        <h5 class="mb-0">Work Orders</h5>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover">
            <thead>
              <tr class="table-light">
                <th>Work Order ID</th>
                <th>Change Request</th>
                <th>Assigned Technician</th>
                <th>Priority</th>
                <th>Status</th>
                <th>Due Date</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="wo in workOrders" :key="wo.id">
                <td><strong>{{ wo.id }}</strong></td>
                <td>{{ wo.changeRequest }}</td>
                <td>{{ wo.technician }}</td>
                <td>
                  <span :class="getPriorityBadgeClass(wo.priority)">
                    {{ wo.priority }}
                  </span>
                </td>
                <td>
                  <span :class="getStatusBadgeClass(wo.status)">
                    {{ wo.status }}
                  </span>
                </td>
                <td>{{ wo.dueDate }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Active work orders: {{ workOrders.length }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface WorkOrder {
  id: string
  changeRequest: string
  technician: string
  priority: string
  status: string
  dueDate: string
}

const workOrders: WorkOrder[] = [
  {
    id: 'WO-001',
    changeRequest: 'CHG-001',
    technician: 'James Park',
    priority: 'High',
    status: 'In Progress',
    dueDate: '2026-02-22'
  },
  {
    id: 'WO-002',
    changeRequest: 'CHG-002',
    technician: 'Patricia Garcia',
    priority: 'Medium',
    status: 'Scheduled',
    dueDate: '2026-02-23'
  },
  {
    id: 'WO-003',
    changeRequest: 'CHG-003',
    technician: 'Robert Martinez',
    priority: 'Low',
    status: 'Not Started',
    dueDate: '2026-02-25'
  },
  {
    id: 'WO-004',
    changeRequest: 'CHG-004',
    technician: 'Jennifer Lopez',
    priority: 'High',
    status: 'In Progress',
    dueDate: '2026-02-21'
  },
  {
    id: 'WO-005',
    changeRequest: 'CHG-005',
    technician: 'Christopher Lee',
    priority: 'Critical',
    status: 'In Progress',
    dueDate: '2026-02-20'
  },
  {
    id: 'WO-006',
    changeRequest: 'CHG-201',
    technician: 'Jessica White',
    priority: 'Medium',
    status: 'Completed',
    dueDate: '2026-02-12'
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
    case 'In Progress':
      return `${classes} bg-info`
    case 'Scheduled':
      return `${classes} bg-warning text-dark`
    case 'Not Started':
      return `${classes} bg-secondary`
    case 'Completed':
      return `${classes} bg-success`
    default:
      return `${classes} bg-light text-dark`
  }
}
</script>

<style scoped>
.work-orders {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
