<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">System Configuration</h1>

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
  </div>
</template>

<script setup lang="ts">
const config = {
  timezone: 'UTC',
  tempUnit: 'Celsius (°C)',
  powerUnit: 'Kilowatts (kW)',
  locale: 'English (US)',
  metricsRetention: 365,
  logsRetention: 90,
  alarmsRetention: 180,
  backupFrequency: 'Daily at 02:00 UTC',
  sessionTimeout: 30,
  passwordPolicy: 'Complex (12+ chars, mixed case, numbers)',
  mfaRequired: true,
  alertAggregation: '5 minutes',
  emailNotifications: true,
  smsAlerts: false
};
</script>
