<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">System Audit Logs</h1>

    <div class="row mb-3">
      <div class="col-md-3">
        <input type="text" class="form-control form-control-sm" placeholder="Search logs...">
      </div>
      <div class="col-md-3">
        <select class="form-select form-select-sm">
          <option value="">All Events</option>
          <option value="login">Login</option>
          <option value="config">Configuration</option>
          <option value="user">User Management</option>
          <option value="security">Security</option>
        </select>
      </div>
      <div class="col-md-3">
        <select class="form-select form-select-sm">
          <option value="">All Users</option>
          <option value="admin">John Administrator</option>
          <option value="ops">Sarah Operations</option>
          <option value="system">System</option>
        </select>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Timestamp</th>
                <th>User</th>
                <th>Event Type</th>
                <th>Action</th>
                <th>Resource</th>
                <th>Result</th>
                <th>IP Address</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="log in logs" :key="log.id">
                <td class="small">{{ log.timestamp }}</td>
                <td class="fw-medium">{{ log.user }}</td>
                <td>
                  <span class="badge" :class="getBadgeClass(log.eventType)">
                    {{ log.eventType }}
                  </span>
                </td>
                <td class="small">{{ log.action }}</td>
                <td class="small text-muted">{{ log.resource }}</td>
                <td>
                  <span :class="['badge', log.result === 'Success' ? 'bg-success' : 'bg-danger']">
                    {{ log.result }}
                  </span>
                </td>
                <td class="font-monospace small">{{ log.ipAddress }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const logs = [
  {
    id: 1,
    timestamp: '2026-02-20 14:35:22 UTC',
    user: 'John Administrator',
    eventType: 'Login',
    action: 'User login',
    resource: 'Dashboard',
    result: 'Success',
    ipAddress: '192.168.1.100'
  },
  {
    id: 2,
    timestamp: '2026-02-20 14:30:15 UTC',
    user: 'Sarah Operations',
    eventType: 'Configuration',
    action: 'Modified alarm thresholds',
    resource: 'SystemConfig',
    result: 'Success',
    ipAddress: '192.168.1.101'
  },
  {
    id: 3,
    timestamp: '2026-02-20 14:25:45 UTC',
    user: 'System',
    eventType: 'Security',
    action: 'API key rotation',
    resource: 'ApiAccess',
    result: 'Success',
    ipAddress: '127.0.0.1'
  },
  {
    id: 4,
    timestamp: '2026-02-20 14:20:30 UTC',
    user: 'Mike Engineer',
    eventType: 'User Management',
    action: 'Password change',
    resource: 'User#4',
    result: 'Success',
    ipAddress: '192.168.1.102'
  },
  {
    id: 5,
    timestamp: '2026-02-20 14:15:10 UTC',
    user: 'John Administrator',
    eventType: 'Configuration',
    action: 'Added new sensor',
    resource: 'SensorConfig',
    result: 'Success',
    ipAddress: '192.168.1.100'
  },
  {
    id: 6,
    timestamp: '2026-02-20 14:10:55 UTC',
    user: 'Lisa Viewer',
    eventType: 'Login',
    action: 'Failed login attempt',
    resource: 'Dashboard',
    result: 'Failed',
    ipAddress: '10.0.2.50'
  },
  {
    id: 7,
    timestamp: '2026-02-20 14:05:22 UTC',
    user: 'System',
    eventType: 'Security',
    action: 'Backup completed',
    resource: 'Database',
    result: 'Success',
    ipAddress: '127.0.0.1'
  },
  {
    id: 8,
    timestamp: '2026-02-20 13:55:40 UTC',
    user: 'Sarah Operations',
    eventType: 'User Management',
    action: 'Created new user',
    resource: 'User#6',
    result: 'Success',
    ipAddress: '192.168.1.101'
  }
];

function getBadgeClass(eventType: string): string {
  const classMap: Record<string, string> = {
    'Login': 'bg-primary',
    'Configuration': 'bg-info',
    'User Management': 'bg-warning',
    'Security': 'bg-danger'
  };
  return classMap[eventType] || 'bg-secondary';
}
</script>
