<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Roles and Permissions</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm">+ Add Role</button>
      </div>
    </div>

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
                  <button class="btn btn-sm btn-outline-secondary">Edit</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const roles = [
  {
    id: 1,
    name: 'Administrator',
    userCount: 2,
    dashboard: true,
    power: true,
    cooling: true,
    space: true,
    security: true
  },
  {
    id: 2,
    name: 'Operator',
    userCount: 5,
    dashboard: true,
    power: true,
    cooling: true,
    space: false,
    security: false
  },
  {
    id: 3,
    name: 'Engineer',
    userCount: 3,
    dashboard: true,
    power: true,
    cooling: true,
    space: true,
    security: true
  },
  {
    id: 4,
    name: 'Viewer',
    userCount: 8,
    dashboard: true,
    power: false,
    cooling: false,
    space: false,
    security: false
  },
  {
    id: 5,
    name: 'Auditor',
    userCount: 1,
    dashboard: true,
    power: false,
    cooling: false,
    space: true,
    security: true
  }
];
</script>
