<script setup lang="ts">
const accessLogs = [
  { id: 1, timestamp: '2026-02-20 15:42:18', door: 'Data Center A - Main Entrance', person: 'John Smith', method: 'RFID', result: 'allowed' },
  { id: 2, timestamp: '2026-02-20 15:38:52', door: 'Server Room B1', person: 'Sarah Johnson', method: 'Fingerprint', result: 'allowed' },
  { id: 3, timestamp: '2026-02-20 15:35:11', door: 'Network Hub C', person: 'Mike Chen', method: 'RFID', result: 'allowed' },
  { id: 4, timestamp: '2026-02-20 15:31:47', door: 'Secure Storage A', person: 'Unknown User', method: 'Card', result: 'denied' },
  { id: 5, timestamp: '2026-02-20 15:28:23', door: 'Data Center A - Main Entrance', person: 'Emma Davis', method: 'Biometric', result: 'allowed' },
  { id: 6, timestamp: '2026-02-20 15:24:09', door: 'UPS Room B', person: 'Robert Wilson', method: 'RFID', result: 'allowed' },
  { id: 7, timestamp: '2026-02-20 15:19:34', door: 'Backup Vault', person: 'Lisa Anderson', method: 'Fingerprint', result: 'allowed' },
  { id: 8, timestamp: '2026-02-20 15:15:08', door: 'Server Room B1', person: 'James Taylor', method: 'Card', result: 'denied' },
]

const resultBadge = {
  allowed: 'success',
  denied: 'danger',
} as const

const methodBadge = {
  'RFID': 'primary',
  'Fingerprint': 'info',
  'Card': 'secondary',
  'Biometric': 'primary',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">Physical Security</h4>

    <!-- Stats Row -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-success mb-1">{{ accessLogs.filter(l => l.result === 'allowed').length }}</h3>
            <small class="text-muted">Authorized Access</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-danger mb-1">{{ accessLogs.filter(l => l.result === 'denied').length }}</h3>
            <small class="text-muted">Denied Access</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-primary mb-1">24</h3>
            <small class="text-muted">Active Doors</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-info mb-1">98%</h3>
            <small class="text-muted">System Uptime</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Access Logs Table -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Access Control Logs (Last 8 Events)</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Timestamp</th>
              <th>Door / Location</th>
              <th>Person</th>
              <th>Method</th>
              <th>Result</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in accessLogs" :key="log.id">
              <td class="text-muted">{{ log.timestamp }}</td>
              <td class="fw-semibold">{{ log.door }}</td>
              <td>{{ log.person }}</td>
              <td><span class="badge" :class="`bg-${methodBadge[log.method as keyof typeof methodBadge]}`">{{ log.method }}</span></td>
              <td><span class="badge" :class="`bg-${resultBadge[log.result as keyof typeof resultBadge]}`">{{ log.result }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
