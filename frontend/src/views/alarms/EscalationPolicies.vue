<template>
  <div class="container-fluid">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Escalation Policies</h1>
      <button class="btn btn-primary" @click="openCreate">+ Create Policy</button>
    </div>

    <!-- Escalation Policies List -->
    <div class="row">
      <div v-for="policy in escalationPolicies" :key="policy.id" class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <div class="d-flex justify-content-between align-items-start">
              <div>
                <h5 class="mb-1">{{ policy.name }}</h5>
                <small class="text-muted">{{ policy.description }}</small>
              </div>
              <span :class="policy.active ? 'badge bg-success' : 'badge bg-secondary'">
                {{ policy.active ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>
          <div class="card-body">
            <div v-for="(level, index) in policy.escalationLevels" :key="index" class="mb-3">
              <div class="d-flex align-items-start">
                <div
                  class="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center flex-shrink-0"
                  style="width: 32px; height: 32px; font-size: 12px"
                >
                  {{ index + 1 }}
                </div>
                <div class="ms-3 flex-grow-1">
                  <p class="mb-1 fw-500">{{ level.contact }}</p>
                  <small class="text-muted d-block">Wait {{ level.waitTime }} before escalation</small>
                  <small class="text-muted">Method: {{ level.method }}</small>
                </div>
              </div>
            </div>
          </div>
          <div class="card-footer">
            <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(policy)">Edit</button>
            <button class="btn btn-sm btn-outline-danger" @click="remove(policy.id)">Delete</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Default Escalation Settings -->
    <div class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">Default Escalation Settings</h5>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">Critical Alerts - Escalation Policy</label>
            <select v-model="defaultSettings.criticalPolicy" class="form-select">
              <option value="immediate">Immediate Escalation</option>
              <option value="ops-critical">Operations - Critical Only</option>
              <option value="all-hands">All Hands on Deck</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">High Alerts - Escalation Policy</label>
            <select v-model="defaultSettings.highPolicy" class="form-select">
              <option value="delayed">Delayed Escalation (5 min)</option>
              <option value="ops-team">Operations Team</option>
              <option value="manager">Manager Notification</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">Escalation Enabled</label>
            <div class="form-check form-switch">
              <input
                v-model="defaultSettings.escalationEnabled"
                type="checkbox"
                class="form-check-input"
                id="escalationEnabled"
              />
              <label class="form-check-label" for="escalationEnabled">
                {{ defaultSettings.escalationEnabled ? 'Yes' : 'No' }}
              </label>
            </div>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">Max Escalation Levels</label>
            <input
              v-model.number="defaultSettings.maxLevels"
              type="number"
              class="form-control"
              min="1"
              max="5"
            />
          </div>
        </div>
        <button class="btn btn-primary">Save Settings</button>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Escalation Policy</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <input v-model="form.description" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Levels (JSON)</label>
              <input v-model="form.levels" type="text" class="form-control" />
            </div>
            <div class="mb-3 form-check">
              <input v-model="form.active" type="checkbox" class="form-check-input" id="formActive" />
              <label class="form-check-label" for="formActive">Active</label>
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
import { ref, reactive, onMounted } from 'vue'
import { escalationPoliciesApi, type EscalationPolicy } from '../../services/api'

interface EscalationLevel {
  contact: string
  waitTime: string
  method: string
}

interface DisplayPolicy extends EscalationPolicy {
  escalationLevels: EscalationLevel[]
}

const escalationPolicies = ref<DisplayPolicy[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<DisplayPolicy | null>(null)
const defaultForm = { name: '', description: '', levels: '', active: true }
const form = ref({ ...defaultForm })

async function loadData() {
  const data = await escalationPoliciesApi.getAll()
  escalationPolicies.value = data.map(p => ({
    ...p,
    escalationLevels: typeof p.levels === 'string' ? JSON.parse(p.levels) : [],
  }))
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(policy: DisplayPolicy) {
  editing.value = policy
  form.value = { name: policy.name, description: policy.description, levels: policy.levels, active: policy.active }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await escalationPoliciesApi.update(editing.value.id, form.value)
    else await escalationPoliciesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await escalationPoliciesApi.remove(id)
  await loadData()
}

const defaultSettings = reactive({
  criticalPolicy: 'immediate',
  highPolicy: 'delayed',
  escalationEnabled: true,
  maxLevels: 3,
})
</script>
