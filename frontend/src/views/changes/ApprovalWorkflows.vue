<template>
  <div class="approval-workflows">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else class="card">
      <div class="card-header bg-warning text-dark d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Approval Workflows</h5>
        <button class="btn btn-sm btn-dark" @click="openCreate">+ Add</button>
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
                    <div v-for="step in parseSteps(workflow.steps)" :key="step.order" class="d-flex align-items-center">
                      <div class="badge" :class="getStepBadgeClass(step.status)">
                        {{ step.order }}
                      </div>
                      <div class="ms-2 flex-grow-1">
                        <small class="d-block">{{ step.role }}</small>
                        <small class="text-muted">{{ step.approver }}</small>
                      </div>
                      <span class="badge bg-success" v-if="step.status === 'completed'">
                        Done
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
                <div>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(workflow)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(workflow.id)">Delete</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'New' }} Workflow</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Workflow ID</label>
              <input v-model="form.workflowId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Priority</label>
              <select v-model="form.priority" class="form-select">
                <option>Critical</option>
                <option>High</option>
                <option>Medium</option>
                <option>Low</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Steps (JSON)</label>
              <textarea v-model="form.steps" class="form-control" rows="3"></textarea>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { approvalWorkflowsApi, type ApprovalWorkflow } from '../../services/api'

interface ApprovalStep {
  order: number
  role: string
  approver: string
  status: string
}

const workflows = ref<ApprovalWorkflow[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ApprovalWorkflow | null>(null)

const defaultForm = { workflowId: '', name: '', type: '', priority: 'Medium', steps: '[]' }
const form = ref({ ...defaultForm })

function parseSteps(steps: string): ApprovalStep[] {
  try {
    return JSON.parse(steps)
  } catch {
    return []
  }
}

async function loadData() {
  workflows.value = await approvalWorkflowsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(workflow: ApprovalWorkflow) {
  editing.value = workflow
  form.value = { workflowId: workflow.workflowId, name: workflow.name, type: workflow.type, priority: workflow.priority, steps: workflow.steps }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await approvalWorkflowsApi.update(editing.value.id, form.value)
    else await approvalWorkflowsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await approvalWorkflowsApi.remove(id)
  await loadData()
}

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
