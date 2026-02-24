<template>
  <div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Push Notifications</h1>
      <button class="btn btn-primary">+ Add Channel</button>
    </div>

    <!-- Push Channels Configuration -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Configured Channels</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Channel Name</th>
              <th>Type</th>
              <th>Endpoint</th>
              <th>Alert Types</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="channel in pushChannels" :key="channel.id">
              <td>{{ channel.name }}</td>
              <td>
                <span class="badge bg-info">{{ channel.type }}</span>
              </td>
              <td class="text-truncate" style="max-width: 250px">{{ channel.endpoint }}</td>
              <td>{{ channel.alertTypes.join(', ') }}</td>
              <td>
                <span :class="channel.active ? 'badge bg-success' : 'badge bg-danger'">
                  {{ channel.active ? 'Active' : 'Inactive' }}
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

    <!-- Push Notification Settings -->
    <div class="row">
      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Mobile App Settings</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Push Notifications Enabled</label>
              <div class="form-check form-switch">
                <input
                  v-model="pushSettings.mobileEnabled"
                  type="checkbox"
                  class="form-check-input"
                  id="mobileEnabled"
                />
                <label class="form-check-label" for="mobileEnabled">
                  {{ pushSettings.mobileEnabled ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Include Rich Media</label>
              <div class="form-check form-switch">
                <input
                  v-model="pushSettings.richMedia"
                  type="checkbox"
                  class="form-check-input"
                  id="richMedia"
                />
                <label class="form-check-label" for="richMedia">
                  {{ pushSettings.richMedia ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Delivery Timeout (seconds)</label>
              <input
                v-model.number="pushSettings.deliveryTimeout"
                type="number"
                class="form-control"
                min="10"
                max="300"
              />
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Webhook Settings</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Webhook Enabled</label>
              <div class="form-check form-switch">
                <input
                  v-model="pushSettings.webhookEnabled"
                  type="checkbox"
                  class="form-check-input"
                  id="webhookEnabled"
                />
                <label class="form-check-label" for="webhookEnabled">
                  {{ pushSettings.webhookEnabled ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Retry Policy</label>
              <select v-model="pushSettings.retryPolicy" class="form-select">
                <option value="none">No Retry</option>
                <option value="exponential">Exponential Backoff</option>
                <option value="fixed">Fixed Interval (5 min)</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Max Retries</label>
              <input
                v-model.number="pushSettings.maxRetries"
                type="number"
                class="form-control"
                min="0"
                max="10"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Save Button -->
    <div class="mt-4">
      <button class="btn btn-primary btn-lg">Save All Settings</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'

interface PushChannel {
  id: number
  name: string
  type: string
  endpoint: string
  alertTypes: string[]
  active: boolean
}

const pushChannels: PushChannel[] = [
  {
    id: 1,
    name: 'Mobile App (iOS)',
    type: 'APNs',
    endpoint: 'com.company.dcim.ios',
    alertTypes: ['Critical', 'High'],
    active: true,
  },
  {
    id: 2,
    name: 'Mobile App (Android)',
    type: 'Firebase',
    endpoint: 'com.company.dcim.android',
    alertTypes: ['Critical', 'High', 'Medium'],
    active: true,
  },
  {
    id: 3,
    name: 'Operations Webhook',
    type: 'Webhook',
    endpoint: 'https://api.company.com/webhooks/alerts/operations',
    alertTypes: ['All'],
    active: true,
  },
  {
    id: 4,
    name: 'Slack Integration',
    type: 'Webhook',
    endpoint: 'https://hooks.slack.com/services/T00000000/B00000000/XXXXXXXXXXXXXXXXXXXX',
    alertTypes: ['Critical', 'High', 'Medium'],
    active: true,
  },
  {
    id: 5,
    name: 'PagerDuty',
    type: 'API',
    endpoint: 'https://api.pagerduty.com/incidents',
    alertTypes: ['Critical'],
    active: false,
  },
]

const pushSettings = reactive({
  mobileEnabled: true,
  richMedia: true,
  deliveryTimeout: 120,
  webhookEnabled: true,
  retryPolicy: 'exponential',
  maxRetries: 3,
})
</script>
