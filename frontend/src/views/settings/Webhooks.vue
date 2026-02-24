<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Webhooks</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm">+ Add Webhook</button>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Name</th>
                <th>Endpoint URL</th>
                <th>Events Triggered</th>
                <th>Status</th>
                <th>Last Event</th>
                <th>Delivery Rate</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="webhook in webhooks" :key="webhook.id">
                <td class="fw-medium">{{ webhook.name }}</td>
                <td class="font-monospace small text-truncate" :title="webhook.url">{{ webhook.url }}</td>
                <td>
                  <span v-for="event in webhook.events" :key="event" class="badge bg-secondary me-1">
                    {{ event }}
                  </span>
                </td>
                <td>
                  <span :class="['badge', webhook.status === 'Active' ? 'bg-success' : webhook.status === 'Failing' ? 'bg-danger' : 'bg-warning']">
                    {{ webhook.status }}
                  </span>
                </td>
                <td class="text-muted small">{{ webhook.lastEvent }}</td>
                <td>
                  <div class="progress" style="height: 20px; width: 80px;">
                    <div class="progress-bar" :class="webhook.deliveryRate >= 95 ? 'bg-success' : 'bg-warning'"
                         :style="{ width: webhook.deliveryRate + '%' }"></div>
                  </div>
                  <span class="small">{{ webhook.deliveryRate }}%</span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary">Edit</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const webhooks = [
  {
    id: 1,
    name: 'Slack Alerts',
    url: 'https://hooks.slack.com/services/T0000000/B0000000/XXXXXXXXXXXX',
    events: ['Alert', 'Alarm'],
    status: 'Active',
    lastEvent: '2026-02-20 14:32:15 UTC',
    deliveryRate: 99.8
  },
  {
    id: 2,
    name: 'SIEM Integration',
    url: 'https://siem.company.com/api/v2/events',
    events: ['Security', 'Access', 'Config Change'],
    status: 'Active',
    lastEvent: '2026-02-20 14:35:10 UTC',
    deliveryRate: 100
  },
  {
    id: 3,
    name: 'Email Notifications',
    url: 'https://notification.datacenter.local/mail',
    events: ['Critical', 'Alarm'],
    status: 'Active',
    lastEvent: '2026-02-20 14:20:45 UTC',
    deliveryRate: 98.5
  },
  {
    id: 4,
    name: 'PagerDuty Incident',
    url: 'https://events.pagerduty.com/v2/enqueue',
    events: ['Critical', 'Emergency'],
    status: 'Failing',
    lastEvent: '2026-02-20 10:15:30 UTC',
    deliveryRate: 87.3
  },
  {
    id: 5,
    name: 'Analytics Pipeline',
    url: 'https://analytics.internal/events',
    events: ['Metrics', 'Usage'],
    status: 'Paused',
    lastEvent: '2026-02-15 12:00:00 UTC',
    deliveryRate: 0
  }
];
</script>
