<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1 class="h3 mb-0">System Configuration</h1>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Setting</button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="row">
      <div class="col-lg-6">
        <div class="card mb-4">
          <div class="card-header">
            <h5 class="mb-0">Regional Settings</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Timezone</label>
              <p class="fw-medium">{{ config.timezone }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Temperature Unit</label>
              <p class="fw-medium">{{ config.tempUnit }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Power Unit</label>
              <p class="fw-medium">{{ config.powerUnit }}</p>
            </div>
            <div class="mb-0">
              <label class="form-label">Locale</label>
              <p class="fw-medium">{{ config.locale }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card mb-4">
          <div class="card-header">
            <h5 class="mb-0">Data Retention</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Metrics Retention (days)</label>
              <p class="fw-medium">{{ config.metricsRetention }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Logs Retention (days)</label>
              <p class="fw-medium">{{ config.logsRetention }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Alarms Retention (days)</label>
              <p class="fw-medium">{{ config.alarmsRetention }}</p>
            </div>
            <div class="mb-0">
              <label class="form-label">Backup Frequency</label>
              <span class="badge bg-info">{{ config.backupFrequency }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Security</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Session Timeout (minutes)</label>
              <p class="fw-medium">{{ config.sessionTimeout }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Password Policy</label>
              <p class="fw-medium">{{ config.passwordPolicy }}</p>
            </div>
            <div class="mb-0">
              <label class="form-label">MFA Required</label>
              <span :class="['badge', config.mfaRequired ? 'bg-success' : 'bg-warning']">
                {{ config.mfaRequired ? 'Enabled' : 'Disabled' }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Alerts & Notifications</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Alert Aggregation</label>
              <p class="fw-medium">{{ config.alertAggregation }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label">Email Notifications</label>
              <span :class="['badge', config.emailNotifications ? 'bg-success' : 'bg-secondary']">
                {{ config.emailNotifications ? 'Enabled' : 'Disabled' }}
              </span>
            </div>
            <div class="mb-0">
              <label class="form-label">SMS Alerts</label>
              <span :class="['badge', config.smsAlerts ? 'bg-success' : 'bg-secondary']">
                {{ config.smsAlerts ? 'Enabled' : 'Disabled' }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Setting</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Key</label>
              <input v-model="form.key" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Value</label>
              <input v-model="form.value" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Category</label>
              <input v-model="form.category" type="text" class="form-control" />
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
import { systemSettingsApi, type SystemSetting } from '../../services/api'

const showModal = ref(false)
const saving = ref(false)
const editing = ref<SystemSetting | null>(null)
const form = ref({ key: '', value: '', category: '' })
const allSettings = ref<SystemSetting[]>([])

function openCreate() {
  editing.value = null
  form.value = { key: '', value: '', category: '' }
  showModal.value = true
}

function openEdit(item: SystemSetting) {
  editing.value = item
  form.value = { key: item.key, value: item.value, category: item.category }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await systemSettingsApi.update(editing.value.id, form.value)
    else await systemSettingsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await systemSettingsApi.remove(id)
  await loadData()
}

const config = ref<Record<string, any>>({
  timezone: '',
  tempUnit: '',
  powerUnit: '',
  locale: '',
  metricsRetention: 0,
  logsRetention: 0,
  alarmsRetention: 0,
  backupFrequency: '',
  sessionTimeout: 0,
  passwordPolicy: '',
  mfaRequired: false,
  alertAggregation: '',
  emailNotifications: false,
  smsAlerts: false
})
const loading = ref(true)

async function loadData() {
  const settings = await systemSettingsApi.getAll()
  const map: Record<string, string> = {}
  settings.forEach((s: SystemSetting) => {
    map[s.key] = s.value
  })
  config.value = {
    timezone: map['timezone'] || '',
    tempUnit: map['tempUnit'] || '',
    powerUnit: map['powerUnit'] || '',
    locale: map['locale'] || '',
    metricsRetention: Number(map['metricsRetention']) || 0,
    logsRetention: Number(map['logsRetention']) || 0,
    alarmsRetention: Number(map['alarmsRetention']) || 0,
    backupFrequency: map['backupFrequency'] || '',
    sessionTimeout: Number(map['sessionTimeout']) || 0,
    passwordPolicy: map['passwordPolicy'] || '',
    mfaRequired: map['mfaRequired'] === 'true',
    alertAggregation: map['alertAggregation'] || '',
    emailNotifications: map['emailNotifications'] === 'true',
    smsAlerts: map['smsAlerts'] === 'true'
  }
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})
</script>
