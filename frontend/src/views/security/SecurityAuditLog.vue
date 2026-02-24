<script setup lang="ts">
const auditLogs = [
  { id: 1, timestamp: '2026-02-20 15:45:23', user: 'admin-001', action: 'Login', resource: 'Security Dashboard', result: 'success', details: 'HTTPS from 192.168.1.100' },
  { id: 2, timestamp: '2026-02-20 15:42:18', user: 'secmgr-001', action: 'IP Whitelist Added', resource: 'Firewall Policy', result: 'success', details: 'Added 203.0.113.50 to whitelist' },
  { id: 3, timestamp: '2026-02-20 15:38:47', user: 'netadmin-001', action: 'VPN Session Started', resource: 'Network Access', result: 'success', details: 'VPN Gateway A - Duration: 0:15:22' },
  { id: 4, timestamp: '2026-02-20 15:35:12', user: 'analyst-001', action: 'Report Generated', resource: 'Security Report', result: 'success', details: 'Threat Summary Report - Feb 2026' },
  { id: 5, timestamp: '2026-02-20 15:30:54', user: 'admin-002', action: 'Role Modified', resource: 'Access Control', result: 'success', details: 'Security Analyst - Added "audit_read" permission' },
  { id: 6, timestamp: '2026-02-20 15:25:33', user: 'unknown', action: 'Failed Login Attempt', resource: 'Security Dashboard', result: 'failed', details: 'Invalid credentials from 10.0.5.88' },
  { id: 7, timestamp: '2026-02-20 15:20:18', user: 'secmgr-002', action: 'Policy Created', resource: 'Access Policy', result: 'success', details: 'New Data Center C Access Policy' },
  { id: 8, timestamp: '2026-02-20 15:15:42', user: 'admin-001', action: 'Audit Settings Changed', resource: 'System Configuration', result: 'success', details: 'Retention: 90 days, Level: Full' },
  { id: 9, timestamp: '2026-02-20 15:10:28', user: 'tech-001', action: 'Access Granted', resource: 'Server Room B', result: 'success', details: 'RFID Card - Access Duration: 2 hours' },
  { id: 10, timestamp: '2026-02-20 15:05:11', user: 'analyst-001', action: 'Alert Acknowledged', resource: 'Security Alert', result: 'success', details: 'IDS Alert #4782 - Malware Signature Match' },
]

const resultColor = {
  success: 'success',
  failed: 'danger',
} as const

const actionColor = {
  'Login': 'primary',
  'Failed Login Attempt': 'danger',
  'IP Whitelist Added': 'info',
  'VPN Session Started': 'success',
  'Report Generated': 'primary',
  'Role Modified': 'warning',
  'Policy Created': 'info',
  'Audit Settings Changed': 'secondary',
  'Access Granted': 'success',
  'Alert Acknowledged': 'info',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">Security Audit Log</h4>

    <!-- Filter and Stats -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-success mb-1">{{ auditLogs.filter(l => l.result === 'success').length }}</h3>
            <small class="text-muted">Successful Actions</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-danger mb-1">{{ auditLogs.filter(l => l.result === 'failed').length }}</h3>
            <small class="text-muted">Failed Actions</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-info mb-1">{{ new Set(auditLogs.map(l => l.user)).size }}</h3>
            <small class="text-muted">Active Users</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-primary mb-1">90 days</h3>
            <small class="text-muted">Log Retention</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Audit Log Table -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center">
        <span>Audit Log Events</span>
        <span class="badge bg-primary ms-2">{{ auditLogs.length }}</span>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Timestamp</th>
                <th>User</th>
                <th>Action</th>
                <th>Resource</th>
                <th>Result</th>
                <th>Details</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="log in auditLogs" :key="log.id">
                <td class="text-muted" style="font-size: 0.9rem;">{{ log.timestamp }}</td>
                <td><code class="fw-semibold">{{ log.user }}</code></td>
                <td>
                  <span class="badge" :class="`bg-${actionColor[log.action as keyof typeof actionColor] || 'secondary'}`">
                    {{ log.action }}
                  </span>
                </td>
                <td class="text-muted">{{ log.resource }}</td>
                <td><span class="badge" :class="`bg-${resultColor[log.result as keyof typeof resultColor]}`">{{ log.result }}</span></td>
                <td style="max-width: 250px;">
                  <small class="text-muted" title="log.details">{{ log.details }}</small>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
