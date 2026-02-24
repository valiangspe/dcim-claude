<template>
  <div class="approval-workflows">
    <div class="card">
      <div class="card-header bg-warning text-dark">
        <h5 class="mb-0">Approval Workflows</h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div v-for="workflow in workflows" :key="workflow.id" class="col-md-6">
            <div class="card border-light shadow-sm">
              <div class="card-body">
                <h6 class="card-title fw-bold">{{ workflow.name }}</h6>
                <p class="card-text text-muted small">{{ workflow.type }}</p>
                <div class="mb-3">
                  <small class="d-block mb-2 fw-semibold">Approval Steps:</small>
                  <div class="d-flex flex-column gap-2">
                    <div v-for="step in workflow.steps" :key="step.order" class="d-flex align-items-center">
                      <div class="badge" :class="getStepBadgeClass(step.status)">
                        {{ step.order }}
                      </div>
                      <div class="ms-2 flex-grow-1">
                        <small class="d-block">{{ step.role }}</small>
                        <small class="text-muted">{{ step.approver }}</small>
                      </div>
                      <span class="badge bg-success" v-if="step.status === 'completed'">
                        ✓
                      </span>
                      <span class="badge bg-warning text-dark" v-else-if="step.status === 'pending'">
                        Pending
                      </span>
                    </div>
                  </div>
                </div>
              </div>
              <div class="card-footer bg-light d-flex justify-content-between">
                <small class="text-muted">Priority: {{ workflow.priority }}</small>
                <button class="btn btn-sm btn-outline-primary">View</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface ApprovalStep {
  order: number
  role: string
  approver: string
  status: string
}

interface Workflow {
  id: string
  name: string
  type: string
  priority: string
  steps: ApprovalStep[]
}

const workflows: Workflow[] = [
  {
    id: 'WF-001',
    name: 'Standard Change Approval',
    type: 'Hardware',
    priority: 'Medium',
    steps: [
      { order: 1, role: 'Technical Lead', approver: 'Tom Harris', status: 'completed' },
      { order: 2, role: 'Department Manager', approver: 'Linda Scott', status: 'completed' },
      { order: 3, role: 'IT Director', approver: 'Mark Johnson', status: 'pending' }
    ]
  },
  {
    id: 'WF-002',
    name: 'Emergency Change Approval',
    type: 'Emergency',
    priority: 'Critical',
    steps: [
      { order: 1, role: 'Duty Manager', approver: 'Susan Miller', status: 'completed' },
      { order: 2, role: 'Director', approver: 'Richard Davis', status: 'pending' }
    ]
  },
  {
    id: 'WF-003',
    name: 'Security Access Approval',
    type: 'Access',
    priority: 'High',
    steps: [
      { order: 1, role: 'Direct Supervisor', approver: 'Kevin Brown', status: 'completed' },
      { order: 2, role: 'Security Officer', approver: 'Rachel Green', status: 'completed' },
      { order: 3, role: 'CISO', approver: 'Andrew White', status: 'pending' }
    ]
  },
  {
    id: 'WF-004',
    name: 'Network Configuration Approval',
    type: 'Network',
    priority: 'High',
    steps: [
      { order: 1, role: 'Network Team Lead', approver: 'Jennifer Chen', status: 'completed' },
      { order: 2, role: 'Compliance Officer', approver: 'Paul Kumar', status: 'pending' }
    ]
  }
]

const getStepBadgeClass = (status: string): string => {
  const baseClass = 'badge'
  switch (status) {
    case 'completed':
      return `${baseClass} bg-success`
    case 'pending':
      return `${baseClass} bg-warning text-dark`
    case 'rejected':
      return `${baseClass} bg-danger`
    default:
      return `${baseClass} bg-secondary`
  }
}
</script>

<style scoped>
.approval-workflows {
  padding: 1rem;
}

.card {
  transition: box-shadow 0.3s;
}

.card:hover {
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15) !important;
}
</style>
