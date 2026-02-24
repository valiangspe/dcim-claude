<template>
  <div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>SMS Notifications</h1>
      <button class="btn btn-primary">+ Add Phone Number</button>
    </div>

    <!-- SMS Recipients Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">SMS Recipients</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Contact Name</th>
              <th>Phone Number</th>
              <th>Alert Severity</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="recipient in smsRecipients" :key="recipient.id">
              <td>{{ recipient.name }}</td>
              <td>{{ recipient.phone }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ recipient.severity }}</span>
              </td>
              <td>
                <div class="form-check form-switch">
                  <input
                    type="checkbox"
                    class="form-check-input"
                    :checked="recipient.enabled"
                    :disabled="true"
                  />
                  <label class="form-check-label">
                    {{ recipient.enabled ? 'Enabled' : 'Disabled' }}
                  </label>
                </div>
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

    <!-- SMS Settings -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">SMS Settings</h5>
      </div>
      <div class="card-body">
        <div class="mb-3">
          <label class="form-label">SMS Alerts Enabled</label>
          <div class="form-check form-switch">
            <input
              v-model="smsSettings.enabled"
              type="checkbox"
              class="form-check-input"
              id="smsEnabled"
            />
            <label class="form-check-label" for="smsEnabled">
              {{ smsSettings.enabled ? 'Yes' : 'No' }}
            </label>
          </div>
        </div>
        <div class="mb-3">
          <label class="form-label">Minimum Alert Severity for SMS</label>
          <select v-model="smsSettings.minSeverity" class="form-select">
            <option value="critical">Critical</option>
            <option value="high">High</option>
            <option value="medium">Medium</option>
            <option value="low">Low</option>
          </select>
        </div>
        <div class="mb-3">
          <label class="form-label">Daily SMS Limit</label>
          <input
            v-model.number="smsSettings.dailyLimit"
            type="number"
            class="form-control"
            min="1"
            max="1000"
          />
        </div>
        <div class="mb-3">
          <label class="form-label">Quiet Hours (24-hour format)</label>
          <div class="row">
            <div class="col-md-4">
              <label class="form-label">Start Time</label>
              <input
                v-model="smsSettings.quietHourStart"
                type="time"
                class="form-control"
              />
            </div>
            <div class="col-md-4">
              <label class="form-label">End Time</label>
              <input v-model="smsSettings.quietHourEnd" type="time" class="form-control" />
            </div>
            <div class="col-md-4">
              <label class="form-label">Enabled</label>
              <div class="form-check form-switch pt-2">
                <input
                  v-model="smsSettings.quietHourEnabled"
                  type="checkbox"
                  class="form-check-input"
                  id="quietHoursEnabled"
                />
                <label class="form-check-label" for="quietHoursEnabled"> </label>
              </div>
            </div>
          </div>
        </div>
        <button class="btn btn-primary">Save Settings</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'

interface SmsRecipient {
  id: number
  name: string
  phone: string
  severity: string
  enabled: boolean
}

const smsRecipients: SmsRecipient[] = [
  {
    id: 1,
    name: 'John Smith',
    phone: '+1 (555) 123-4567',
    severity: 'Critical',
    enabled: true,
  },
  {
    id: 2,
    name: 'Jane Doe',
    phone: '+1 (555) 987-6543',
    severity: 'Critical, High',
    enabled: true,
  },
  {
    id: 3,
    name: 'Mike Johnson',
    phone: '+1 (555) 456-7890',
    severity: 'Critical',
    enabled: false,
  },
  {
    id: 4,
    name: 'Sarah Williams',
    phone: '+1 (555) 321-0987',
    severity: 'All',
    enabled: true,
  },
]

const smsSettings = reactive({
  enabled: true,
  minSeverity: 'critical',
  dailyLimit: 50,
  quietHourStart: '22:00',
  quietHourEnd: '06:00',
  quietHourEnabled: true,
})
</script>
