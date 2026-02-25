<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { firewallEventsApi, type FirewallEvent, idsAlertsApi, type IdsAlert, blockedIpsApi, type BlockedIp } from '../../services/api'

const firewallEvents = ref<FirewallEvent[]>([])
const idsAlerts = ref<IdsAlert[]>([])
const blockedIPs = ref<BlockedIp[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<FirewallEvent | null>(null)
const defaultForm = { timestamp: '', type: '', sourceIP: '', destPort: 0, action: '', severity: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  const [fw, ids, blocked] = await Promise.all([
    firewallEventsApi.getAll(),
    idsAlertsApi.getAll(),
    blockedIpsApi.getAll(),
  ])
  firewallEvents.value = fw
  idsAlerts.value = ids
  blockedIPs.value = blocked
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(event: FirewallEvent) {
  editing.value = event
  form.value = { timestamp: event.timestamp, type: event.type, sourceIP: event.sourceIP, destPort: event.destPort, action: event.action, severity: event.severity }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await firewallEventsApi.update(editing.value.id, form.value)
    else await firewallEventsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await firewallEventsApi.remove(id)
  await loadData()
}

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

    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>

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
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
        <span>Firewall Events</span>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Event</button>
      </div>
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
              <th style="width:120px">Actions</th>
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
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(event)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(event.id)">Delete</button>
              </td>
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
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Firewall Event</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Timestamp</label>
              <input v-model="form.timestamp" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Event Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Source IP</label>
              <input v-model="form.sourceIP" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Dest Port</label>
              <input v-model.number="form.destPort" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Action</label>
              <input v-model="form.action" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Severity</label>
              <select v-model="form.severity" class="form-select">
                <option value="critical">critical</option>
                <option value="high">high</option>
                <option value="medium">medium</option>
                <option value="low">low</option>
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
