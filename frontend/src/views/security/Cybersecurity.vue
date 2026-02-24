<script setup lang="ts">
const firewallEvents = [
  { id: 1, timestamp: '2026-02-20 15:40:22', type: 'Port Scan Detected', sourceIP: '192.168.1.45', destPort: 22, action: 'blocked', severity: 'medium' },
  { id: 2, timestamp: '2026-02-20 15:35:18', type: 'DDoS Pattern', sourceIP: '203.0.113.8', destPort: 443, action: 'rate-limited', severity: 'high' },
  { id: 3, timestamp: '2026-02-20 15:28:47', type: 'SQL Injection Attempt', sourceIP: '198.51.100.22', destPort: 3306, action: 'blocked', severity: 'critical' },
  { id: 4, timestamp: '2026-02-20 15:20:12', type: 'Suspicious Traffic', sourceIP: '10.0.5.77', destPort: 8080, action: 'logged', severity: 'low' },
]

const idsAlerts = [
  { id: 1, timestamp: '2026-02-20 15:38:45', alert: 'Malware Signature Match', protocol: 'HTTP', confidence: 89, status: 'investigating' },
  { id: 2, timestamp: '2026-02-20 15:32:19', alert: 'Buffer Overflow Attempt', protocol: 'HTTPS', confidence: 94, status: 'blocked' },
  { id: 3, timestamp: '2026-02-20 15:25:33', alert: 'Command Injection Pattern', protocol: 'SSH', confidence: 76, status: 'logged' },
  { id: 4, timestamp: '2026-02-20 15:15:07', alert: 'Suspicious DNS Query', protocol: 'DNS', confidence: 82, status: 'blocked' },
]

const blockedIPs = [
  { ip: '192.0.2.100', reason: 'Brute Force Attack', incidents: 34, firstSeen: '2026-02-18', status: 'active' },
  { ip: '203.0.113.42', reason: 'Malware C2 Communication', incidents: 12, firstSeen: '2026-02-19', status: 'active' },
  { ip: '198.51.100.88', reason: 'Suspicious Scanning', incidents: 8, firstSeen: '2026-02-20', status: 'active' },
  { ip: '192.0.2.55', reason: 'DDoS Source', incidents: 156, firstSeen: '2026-02-17', status: 'inactive' },
]

const severityColor = {
  critical: 'danger',
  high: 'warning',
  medium: 'info',
  low: 'secondary',
} as const

const statusColor = {
  active: 'danger',
  inactive: 'secondary',
  investigating: 'warning',
  blocked: 'success',
  logged: 'info',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">Cybersecurity</h4>

    <!-- Stats Row -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-danger mb-1">{{ firewallEvents.length }}</h3>
            <small class="text-muted">Firewall Events (24h)</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-warning mb-1">{{ idsAlerts.length }}</h3>
            <small class="text-muted">IDS Alerts (24h)</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-info mb-1">{{ blockedIPs.filter(ip => ip.status === 'active').length }}</h3>
            <small class="text-muted">Blocked IPs (Active)</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-success mb-1">99.7%</h3>
            <small class="text-muted">IDS Uptime</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Tabs for different views -->
    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">Firewall Events</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Timestamp</th>
              <th>Event Type</th>
              <th>Source IP</th>
              <th>Dest Port</th>
              <th>Action</th>
              <th>Severity</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="event in firewallEvents" :key="event.id">
              <td class="text-muted">{{ event.timestamp }}</td>
              <td class="fw-semibold">{{ event.type }}</td>
              <td><code>{{ event.sourceIP }}</code></td>
              <td>{{ event.destPort }}</td>
              <td><span class="badge bg-secondary">{{ event.action }}</span></td>
              <td><span class="badge" :class="`bg-${severityColor[event.severity as keyof typeof severityColor]}`">{{ event.severity }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- IDS Alerts -->
    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">IDS Alerts</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Timestamp</th>
              <th>Alert Type</th>
              <th>Protocol</th>
              <th>Confidence</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="alert in idsAlerts" :key="alert.id">
              <td class="text-muted">{{ alert.timestamp }}</td>
              <td class="fw-semibold">{{ alert.alert }}</td>
              <td><span class="badge bg-light text-dark">{{ alert.protocol }}</span></td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <span>{{ alert.confidence }}%</span>
                  <div class="progress" style="width: 60px; height: 6px;">
                    <div class="progress-bar" :class="alert.confidence > 85 ? 'bg-danger' : alert.confidence > 70 ? 'bg-warning' : 'bg-info'" :style="{ width: alert.confidence + '%' }"></div>
                  </div>
                </div>
              </td>
              <td><span class="badge" :class="`bg-${statusColor[alert.status as keyof typeof statusColor]}`">{{ alert.status }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Blocked IPs -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Blocked IP Addresses</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>IP Address</th>
              <th>Reason</th>
              <th>Incidents</th>
              <th>First Seen</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="ip in blockedIPs" :key="ip.ip">
              <td><code class="fw-semibold">{{ ip.ip }}</code></td>
              <td>{{ ip.reason }}</td>
              <td><span class="badge bg-light text-dark">{{ ip.incidents }}</span></td>
              <td class="text-muted">{{ ip.firstSeen }}</td>
              <td><span class="badge" :class="`bg-${statusColor[ip.status as keyof typeof statusColor]}`">{{ ip.status }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
