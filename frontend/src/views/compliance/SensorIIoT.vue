<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { sensorsApi, type Sensor } from '../../services/api'

const sensors = ref<Sensor[]>([])
const loading = ref(true)
const filterType = ref('')
const filterStatus = ref('')

onMounted(async () => {
  try { sensors.value = await sensorsApi.getAll() }
  finally { loading.value = false }
})

const sensorTypes = computed(() => [...new Set(sensors.value.map(s => s.type))])

const rows = computed(() => sensors.value.filter(s => {
  if (filterType.value && s.type !== filterType.value) return false
  if (filterStatus.value && s.status !== filterStatus.value) return false
  return true
}))

const kpis = computed(() => {
  const active = sensors.value.filter(s => s.status === 'active' || s.status === 'online').length
  const offline = sensors.value.filter(s => s.status === 'offline' || s.status === 'error').length
  const health = sensors.value.length > 0 ? Math.round((active / sensors.value.length) * 100) : 100
  return [
    { label: 'Total Sensors', value: String(sensors.value.length), color: 'primary' },
    { label: 'Active / Online', value: String(active), color: 'success' },
    { label: 'Offline / Error', value: String(offline), color: offline > 0 ? 'danger' : 'secondary' },
    { label: 'Connectivity Health', value: health + '%', color: health === 100 ? 'success' : health >= 80 ? 'warning' : 'danger' },
    { label: 'Warning Gate (>5 min)', value: '—', color: 'warning', note: 'Requires live feed' },
    { label: 'Critical Gate (>15 min)', value: '—', color: 'danger', note: 'Requires live feed' },
  ]
})

const statusColor: Record<string, string> = { active: 'success', online: 'success', offline: 'danger', error: 'danger', maintenance: 'warning' }
</script>

<template>
  <div>
    <h4 class="mb-1">Sensor / IIoT Health</h4>
    <p class="text-muted small mb-4">
      Edge gateway connectivity — Warning: sensor silent &gt;5 min · Critical: silent &gt;15 min
      <span class="badge bg-secondary ms-2">Heartbeat thresholds require live data integration</span>
    </p>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <h3 :class="`text-${kpi.color} mb-1`">{{ kpi.value }}</h3>
              <small class="text-muted d-block">{{ kpi.label }}</small>
              <small v-if="kpi.note" class="text-secondary">{{ kpi.note }}</small>
            </div>
          </div>
        </div>
      </div>

      <div class="alert alert-info small mb-4">
        <strong>IIoT Heartbeat Gates:</strong> The 5-minute (Warning) and 15-minute (Critical) silence thresholds require a live BMS/SCADA data feed with per-sensor last-seen timestamps.
        The table below shows currently registered sensors and their configured status. Real-time heartbeat monitoring becomes active once live ingest is connected.
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between flex-wrap gap-2">
          <span>Registered Sensors</span>
          <div class="d-flex gap-2">
            <select v-model="filterType" class="form-select form-select-sm" style="width:150px">
              <option value="">All Types</option>
              <option v-for="t in sensorTypes" :key="t" :value="t">{{ t }}</option>
            </select>
            <select v-model="filterStatus" class="form-select form-select-sm" style="width:140px">
              <option value="">All Statuses</option>
              <option value="active">Active</option>
              <option value="online">Online</option>
              <option value="offline">Offline</option>
              <option value="error">Error</option>
              <option value="maintenance">Maintenance</option>
            </select>
          </div>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Sensor ID</th><th>Name</th><th>Type</th><th>Location</th><th>Status</th><th>Polling Interval</th><th>Min Threshold</th><th>Max Threshold</th></tr>
            </thead>
            <tbody>
              <tr v-for="s in rows" :key="s.id" :class="(s.status === 'offline' || s.status === 'error') ? 'table-danger' : ''">
                <td class="font-monospace small">{{ s.sensorId }}</td>
                <td class="fw-semibold">{{ s.name }}</td>
                <td><span class="badge bg-light text-dark border">{{ s.type }}</span></td>
                <td class="text-muted">{{ s.location }}</td>
                <td><span class="badge" :class="`bg-${statusColor[s.status] ?? 'secondary'}`">{{ s.status }}</span></td>
                <td class="text-muted">{{ s.pollingInterval || '—' }}</td>
                <td class="text-muted small">{{ s.minThreshold || '—' }}</td>
                <td class="text-muted small">{{ s.maxThreshold || '—' }}</td>
              </tr>
              <tr v-if="!rows.length"><td colspan="8" class="text-center text-muted py-4">No sensors registered</td></tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>
  </div>
</template>
