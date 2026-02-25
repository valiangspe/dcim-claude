<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { accessRolesApi, type AccessRole, accessRoleUsersApi, type AccessRoleUser } from '../../services/api'

const roles = ref<AccessRole[]>([])
const usersByRole = ref<AccessRoleUser[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<AccessRole | null>(null)
const defaultForm = { name: '', permissionCount: 0, userCount: 0, description: '', status: 'active' }
const form = ref({ ...defaultForm })

async function loadData() {
  const [r, u] = await Promise.all([accessRolesApi.getAll(), accessRoleUsersApi.getAll()])
  roles.value = r
  usersByRole.value = u
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(role: AccessRole) {
  editing.value = role
  form.value = { name: role.name, permissionCount: role.permissionCount, userCount: role.userCount, description: role.description, status: role.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await accessRolesApi.update(editing.value.id, form.value)
    else await accessRolesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await accessRolesApi.remove(id)
  await loadData()
}

const statusColor = {
  active: 'success',
  inactive: 'secondary',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">Access Control</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>

    <!-- Summary Stats -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-primary mb-1">{{ roles.filter(r => r.status === 'active').length }}</h3>
            <small class="text-muted">Active Roles</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-info mb-1">{{ usersByRole.length }}</h3>
            <small class="text-muted">Active Users</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-success mb-1">{{ Math.max(...roles.map(r => r.permissionCount)) }}</h3>
            <small class="text-muted">Max Permissions</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-warning mb-1">2</h3>
            <small class="text-muted">Pending Reviews</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Roles Table -->
    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
        <span>Roles & Permissions</span>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Role</button>
      </div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Role Name</th>
              <th>Description</th>
              <th>Permissions</th>
              <th>Users</th>
              <th>Status</th>
              <th style="width:120px">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="role in roles" :key="role.id">
              <td class="fw-semibold">{{ role.name }}</td>
              <td class="text-muted">{{ role.description }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <span>{{ role.permissionCount }}</span>
                  <div class="progress" style="width: 100px; height: 6px;">
                    <div class="progress-bar bg-primary" :style="{ width: (role.permissionCount / 89 * 100) + '%' }"></div>
                  </div>
                </div>
              </td>
              <td>
                <span v-if="role.userCount > 0" class="badge bg-light text-dark">{{ role.userCount }}</span>
                <span v-else class="text-muted">---</span>
              </td>
              <td><span class="badge" :class="`bg-${statusColor[role.status as keyof typeof statusColor]}`">{{ role.status }}</span></td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(role)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(role.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Users Table -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Active Users</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Username</th>
              <th>Role</th>
              <th>Department</th>
              <th>Last Login</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in usersByRole" :key="user.name">
              <td class="fw-semibold"><code>{{ user.name }}</code></td>
              <td><span class="badge bg-light text-dark">{{ user.role }}</span></td>
              <td class="text-muted">{{ user.department }}</td>
              <td class="text-muted">{{ user.lastLogin }}</td>
              <td><span class="badge bg-success">{{ user.status }}</span></td>
            </tr>
          </tbody>
        </table>
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
              <label class="form-label">Description</label>
              <input v-model="form.description" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Permission Count</label>
              <input v-model.number="form.permissionCount" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">User Count</label>
              <input v-model.number="form.userCount" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="active">active</option>
                <option value="inactive">inactive</option>
              </select>
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
