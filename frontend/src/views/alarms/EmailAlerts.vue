<template>
  <div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Email Alerts Configuration</h1>
      <button class="btn btn-primary">+ Add Recipient</button>
    </div>

    <!-- Email Recipients Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Email Recipients</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Email Address</th>
              <th>Frequency</th>
              <th>Alert Types</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="recipient in emailRecipients" :key="recipient.id">
              <td>{{ recipient.email }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ recipient.frequency }}</span>
              </td>
              <td>{{ recipient.alertTypes.join(', ') }}</td>
              <td>
                <span :class="recipient.active ? 'badge bg-success' : 'badge bg-danger'">
                  {{ recipient.active ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1">Edit</button>
                <button class="btn btn-sm btn-outline-danger">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Email Frequency Settings -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Email Frequency Settings</h5>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">Critical Alerts</label>
            <select v-model="frequencySettings.critical" class="form-select">
              <option value="immediate">Immediate</option>
              <option value="5min">Every 5 Minutes</option>
              <option value="hourly">Hourly Digest</option>
              <option value="daily">Daily Digest</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">High Priority Alerts</label>
            <select v-model="frequencySettings.high" class="form-select">
              <option value="immediate">Immediate</option>
              <option value="5min">Every 5 Minutes</option>
              <option value="hourly">Hourly Digest</option>
              <option value="daily">Daily Digest</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">Medium Priority Alerts</label>
            <select v-model="frequencySettings.medium" class="form-select">
              <option value="5min">Every 5 Minutes</option>
              <option value="hourly">Hourly Digest</option>
              <option value="daily">Daily Digest</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">Low Priority Alerts</label>
            <select v-model="frequencySettings.low" class="form-select">
              <option value="hourly">Hourly Digest</option>
              <option value="daily">Daily Digest</option>
              <option value="disabled">Disabled</option>
            </select>
          </div>
        </div>
        <button class="btn btn-primary">Save Settings</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'

interface EmailRecipient {
  id: number
  email: string
  frequency: string
  alertTypes: string[]
  active: boolean
}

const emailRecipients: EmailRecipient[] = [
  {
    id: 1,
    email: 'ops-team@company.com',
    frequency: 'Immediate',
    alertTypes: ['Critical', 'High'],
    active: true,
  },
  {
    id: 2,
    email: 'manager@company.com',
    frequency: 'Hourly Digest',
    alertTypes: ['All'],
    active: true,
  },
  {
    id: 3,
    email: 'cooling-team@company.com',
    frequency: 'Immediate',
    alertTypes: ['Cooling', 'Temperature'],
    active: true,
  },
  {
    id: 4,
    email: 'network-team@company.com',
    frequency: 'Daily Digest',
    alertTypes: ['Network', 'Connectivity'],
    active: false,
  },
  {
    id: 5,
    email: 'power-team@company.com',
    frequency: 'Immediate',
    alertTypes: ['Power', 'UPS'],
    active: true,
  },
]

const frequencySettings = reactive({
  critical: 'immediate',
  high: '5min',
  medium: 'hourly',
  low: 'daily',
})
</script>
