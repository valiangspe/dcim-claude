<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">User Management</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add User</button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
    <div class="card">
      <div class="card-body">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Role</th>
              <th>Last Login</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in users" :key="user.id">
              <td class="fw-medium">{{ user.name }}</td>
              <td>{{ user.email }}</td>
              <td>
                <span class="badge bg-info">{{ user.role }}</span>
              </td>
              <td class="text-muted">{{ user.lastLogin }}</td>
              <td>
                <span :class="['badge', user.status === 'Active' ? 'bg-success' : 'bg-secondary']">
                  {{ user.status }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(user)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(user.id)">Delete</button>
              </td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} User</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Email</label>
              <input v-model="form.email" type="email" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Role</label>
              <select v-model="form.role" class="form-select">
                <option>Administrator</option>
                <option>Operator</option>
                <option>Engineer</option>
                <option>Viewer</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>Active</option>
                <option>Inactive</option>
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

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { usersApi, type User } from '../../services/api'

const users = ref<User[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<User | null>(null)
const saving = ref(false)

const defaultForm = { name: '', email: '', role: 'Operator', status: 'Active' }
const form = ref({ ...defaultForm })

async function loadData() {
  users.value = await usersApi.getAll()
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

function openEdit(user: User) {
  editing.value = user
  form.value = { name: user.name, email: user.email, role: user.role, status: user.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await usersApi.update(editing.value.id, form.value)
    } else {
      await usersApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this user?')) return
  await usersApi.remove(id)
  await loadData()
}
</script>
