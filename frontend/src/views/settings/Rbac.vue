<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Roles and Permissions</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Role</button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Role</th>
                <th>Users</th>
                <th>Dashboard</th>
                <th>Power</th>
                <th>Cooling</th>
                <th>Space</th>
                <th>Security</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="role in roles" :key="role.id">
                <td class="fw-medium">{{ role.name }}</td>
                <td>
                  <span class="badge bg-light text-dark">{{ role.userCount }}</span>
                </td>
                <td>
                  <span :class="['badge', role.dashboard ? 'bg-success' : 'bg-secondary']">
                    {{ role.dashboard ? 'View' : 'None' }}
                  </span>
                </td>
                <td>
                  <span :class="['badge', role.power ? 'bg-success' : 'bg-secondary']">
                    {{ role.power ? 'Full' : 'None' }}
                  </span>
                </td>
                <td>
                  <span :class="['badge', role.cooling ? 'bg-success' : 'bg-secondary']">
                    {{ role.cooling ? 'Full' : 'None' }}
                  </span>
                </td>
                <td>
                  <span :class="['badge', role.space ? 'bg-success' : 'bg-secondary']">
                    {{ role.space ? 'Edit' : 'None' }}
                  </span>
                </td>
                <td>
                  <span :class="['badge', role.security ? 'bg-success' : 'bg-secondary']">
                    {{ role.security ? 'Full' : 'None' }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(role)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(role.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Role</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">User Count</label>
              <input v-model.number="form.userCount" type="number" class="form-control" />
            </div>
            <div class="form-check mb-2">
              <input v-model="form.dashboard" type="checkbox" class="form-check-input" id="fDashboard" />
              <label class="form-check-label" for="fDashboard">Dashboard</label>
            </div>
            <div class="form-check mb-2">
              <input v-model="form.power" type="checkbox" class="form-check-input" id="fPower" />
              <label class="form-check-label" for="fPower">Power</label>
            </div>
            <div class="form-check mb-2">
              <input v-model="form.cooling" type="checkbox" class="form-check-input" id="fCooling" />
              <label class="form-check-label" for="fCooling">Cooling</label>
            </div>
            <div class="form-check mb-2">
              <input v-model="form.space" type="checkbox" class="form-check-input" id="fSpace" />
              <label class="form-check-label" for="fSpace">Space</label>
            </div>
            <div class="form-check mb-2">
              <input v-model="form.security" type="checkbox" class="form-check-input" id="fSecurity" />
              <label class="form-check-label" for="fSecurity">Security</label>
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
import { rbacRolesApi, type RbacRole } from '../../services/api'

const roles = ref<RbacRole[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<RbacRole | null>(null)
const defaultForm = { name: '', userCount: 0, dashboard: false, power: false, cooling: false, space: false, security: false }
const form = ref({ ...defaultForm })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: RbacRole) {
  editing.value = item
  form.value = { name: item.name, userCount: item.userCount, dashboard: item.dashboard, power: item.power, cooling: item.cooling, space: item.space, security: item.security }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await rbacRolesApi.update(editing.value.id, form.value)
    else await rbacRolesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await rbacRolesApi.remove(id)
  await loadData()
}

async function loadData() {
  roles.value = await rbacRolesApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})
</script>
