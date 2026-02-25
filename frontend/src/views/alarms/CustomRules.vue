<template>
  <div class="container-fluid">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Custom Alert Rules</h1>
      <button class="btn btn-primary" @click="openCreate">+ Create Rule</button>
    </div>

    <!-- Alert Rules Table -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Active Alert Rules</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-sm mb-0">
          <thead class="table-light">
            <tr>
              <th>Rule Name</th>
              <th>Condition</th>
              <th>Threshold</th>
              <th>Action</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="rule in alertRules" :key="rule.id">
              <td>{{ rule.name }}</td>
              <td>{{ rule.condition }}</td>
              <td>{{ rule.threshold }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ rule.action }}</span>
              </td>
              <td>
                <span :class="rule.enabled ? 'badge bg-success' : 'badge bg-secondary'">
                  {{ rule.enabled ? 'Enabled' : 'Disabled' }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(rule)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(rule.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Rule Statistics -->
    <div class="row mt-4">
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Total Rules</h6>
            <p class="h4 mb-0">{{ alertRules.length }}</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Active Rules</h6>
            <p class="h4 mb-0">{{ alertRules.filter((r) => r.enabled).length }}</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Triggered (24h)</h6>
            <p class="h4 mb-0">24</p>
          </div>
        </div>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Alert Rule</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Condition</label>
              <input v-model="form.condition" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Threshold</label>
              <input v-model="form.threshold" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Action</label>
              <input v-model="form.action" type="text" class="form-control" />
            </div>
            <div class="mb-3 form-check">
              <input v-model="form.enabled" type="checkbox" class="form-check-input" id="formEnabled" />
              <label class="form-check-label" for="formEnabled">Enabled</label>
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
import { alertRulesApi, type AlertRule } from '../../services/api'

const alertRules = ref<AlertRule[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<AlertRule | null>(null)
const defaultForm = { name: '', condition: '', threshold: '', action: '', enabled: true }
const form = ref({ ...defaultForm })

async function loadData() {
  alertRules.value = await alertRulesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(rule: AlertRule) {
  editing.value = rule
  form.value = { name: rule.name, condition: rule.condition, threshold: rule.threshold, action: rule.action, enabled: rule.enabled }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await alertRulesApi.update(editing.value.id, form.value)
    else await alertRulesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await alertRulesApi.remove(id)
  await loadData()
}
</script>
