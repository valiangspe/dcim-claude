<script setup lang="ts">
const roles = [
  { id: 1, name: 'Administrator', permissionCount: 89, userCount: 3, description: 'Full system access', status: 'active' },
  { id: 2, name: 'Security Manager', permissionCount: 45, userCount: 5, description: 'Security operations', status: 'active' },
  { id: 3, name: 'Network Admin', permissionCount: 34, userCount: 8, description: 'Network management', status: 'active' },
  { id: 4, name: 'Data Center Tech', permissionCount: 22, userCount: 12, description: 'Facility operations', status: 'active' },
  { id: 5, name: 'Security Analyst', permissionCount: 28, userCount: 6, description: 'Threat monitoring', status: 'active' },
  { id: 6, name: 'Auditor', permissionCount: 15, userCount: 2, description: 'Compliance audits', status: 'active' },
  { id: 7, name: 'Guest Access', permissionCount: 3, userCount: 0, description: 'Limited visitor access', status: 'inactive' },
]

const usersByRole = [
  { name: 'admin-001', role: 'Administrator', department: 'IT Security', lastLogin: '2026-02-20 14:23', status: 'active' },
  { name: 'admin-002', role: 'Administrator', department: 'IT Security', lastLogin: '2026-02-20 09:15', status: 'active' },
  { name: 'admin-003', role: 'Administrator', department: 'IT Security', lastLogin: '2026-02-19 16:42', status: 'active' },
  { name: 'secmgr-001', role: 'Security Manager', department: 'Security', lastLogin: '2026-02-20 15:01', status: 'active' },
  { name: 'secmgr-002', role: 'Security Manager', department: 'Security', lastLogin: '2026-02-20 14:30', status: 'active' },
  { name: 'netadmin-001', role: 'Network Admin', department: 'Network Operations', lastLogin: '2026-02-20 13:45', status: 'active' },
  { name: 'tech-001', role: 'Data Center Tech', department: 'Facilities', lastLogin: '2026-02-20 15:20', status: 'active' },
  { name: 'analyst-001', role: 'Security Analyst', department: 'Security', lastLogin: '2026-02-20 12:15', status: 'active' },
]

const statusColor = {
  active: 'success',
  inactive: 'secondary',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">Access Control</h4>

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
      <div class="card-header bg-transparent fw-semibold">Roles & Permissions</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Role Name</th>
              <th>Description</th>
              <th>Permissions</th>
              <th>Users</th>
              <th>Status</th>
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
  </div>
</template>
