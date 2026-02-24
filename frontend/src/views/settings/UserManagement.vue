<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">User Management</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm">+ Add User</button>
      </div>
    </div>

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
                <button class="btn btn-sm btn-outline-secondary">Edit</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const users = [
  {
    id: 1,
    name: 'John Administrator',
    email: 'john.admin@datacenter.local',
    role: 'Administrator',
    lastLogin: '2026-02-20 14:32 UTC',
    status: 'Active'
  },
  {
    id: 2,
    name: 'Sarah Operations',
    email: 'sarah.ops@datacenter.local',
    role: 'Operator',
    lastLogin: '2026-02-20 08:15 UTC',
    status: 'Active'
  },
  {
    id: 3,
    name: 'Mike Engineer',
    email: 'mike.eng@datacenter.local',
    role: 'Engineer',
    lastLogin: '2026-02-19 23:45 UTC',
    status: 'Active'
  },
  {
    id: 4,
    name: 'Lisa Viewer',
    email: 'lisa.view@datacenter.local',
    role: 'Viewer',
    lastLogin: '2026-02-15 10:22 UTC',
    status: 'Active'
  },
  {
    id: 5,
    name: 'Robert Inactive',
    email: 'robert.inactive@datacenter.local',
    role: 'Operator',
    lastLogin: '2025-12-01 16:00 UTC',
    status: 'Inactive'
  }
];
</script>
