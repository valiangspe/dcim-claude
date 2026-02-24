<template>
  <div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Escalation Policies</h1>
      <button class="btn btn-primary">+ Create Policy</button>
    </div>

    <!-- Escalation Policies List -->
    <div class="row">
      <div v-for="policy in escalationPolicies" :key="policy.id" class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <div class="d-flex justify-content-between align-items-start">
              <div>
                <h5 class="mb-1">{{ policy.name }}</h5>
                <small class="text-muted">{{ policy.description }}</small>
              </div>
              <span :class="policy.active ? 'badge bg-success' : 'badge bg-secondary'">
                {{ policy.active ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>
          <div class="card-body">
            <div v-for="(level, index) in policy.escalationLevels" :key="index" class="mb-3">
              <div class="d-flex align-items-start">
                <div
                  class="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center flex-shrink-0"
                  style="width: 32px; height: 32px; font-size: 12px"
                >
                  {{ index + 1 }}
                </div>
                <div class="ms-3 flex-grow-1">
                  <p class="mb-1 fw-500">{{ level.contact }}</p>
                  <small class="text-muted d-block">Wait {{ level.waitTime }} before escalation</small>
                  <small class="text-muted">Method: {{ level.method }}</small>
                </div>
              </div>
            </div>
          </div>
          <div class="card-footer">
            <button class="btn btn-sm btn-outline-primary me-1">Edit</button>
            <button class="btn btn-sm btn-outline-danger">Delete</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Default Escalation Settings -->
    <div class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">Default Escalation Settings</h5>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">Critical Alerts - Escalation Policy</label>
            <select v-model="defaultSettings.criticalPolicy" class="form-select">
              <option value="immediate">Immediate Escalation</option>
              <option value="ops-critical">Operations - Critical Only</option>
              <option value="all-hands">All Hands on Deck</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">High Alerts - Escalation Policy</label>
            <select v-model="defaultSettings.highPolicy" class="form-select">
              <option value="delayed">Delayed Escalation (5 min)</option>
              <option value="ops-team">Operations Team</option>
              <option value="manager">Manager Notification</option>
            </select>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">Escalation Enabled</label>
            <div class="form-check form-switch">
              <input
                v-model="defaultSettings.escalationEnabled"
                type="checkbox"
                class="form-check-input"
                id="escalationEnabled"
              />
              <label class="form-check-label" for="escalationEnabled">
                {{ defaultSettings.escalationEnabled ? 'Yes' : 'No' }}
              </label>
            </div>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">Max Escalation Levels</label>
            <input
              v-model.number="defaultSettings.maxLevels"
              type="number"
              class="form-control"
              min="1"
              max="5"
            />
          </div>
        </div>
        <button class="btn btn-primary">Save Settings</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'

interface EscalationLevel {
  contact: string
  waitTime: string
  method: string
}

interface EscalationPolicy {
  id: number
  name: string
  description: string
  active: boolean
  escalationLevels: EscalationLevel[]
}

const escalationPolicies: EscalationPolicy[] = [
  {
    id: 1,
    name: 'Critical - Immediate',
    description: 'For critical infrastructure failures',
    active: true,
    escalationLevels: [
      {
        contact: 'On-Duty Operations Engineer',
        waitTime: 'Immediate',
        method: 'SMS + Email + Push',
      },
      {
        contact: 'Operations Manager',
        waitTime: '5 minutes',
        method: 'Call + SMS',
      },
      {
        contact: 'Director of Infrastructure',
        waitTime: '15 minutes',
        method: 'Call + Email',
      },
    ],
  },
  {
    id: 2,
    name: 'High Priority - Delayed',
    description: 'For high severity issues with grace period',
    active: true,
    escalationLevels: [
      {
        contact: 'Operations Team Lead',
        waitTime: '5 minutes',
        method: 'Email + Push',
      },
      {
        contact: 'Senior Operations Engineer',
        waitTime: '15 minutes',
        method: 'SMS + Email',
      },
      {
        contact: 'Operations Manager',
        waitTime: '30 minutes',
        method: 'Call',
      },
    ],
  },
  {
    id: 3,
    name: 'Medium Priority - Business Hours',
    description: 'For medium severity alerts during business hours only',
    active: true,
    escalationLevels: [
      {
        contact: 'Operations Team',
        waitTime: '15 minutes',
        method: 'Email',
      },
      {
        contact: 'Technical Lead',
        waitTime: '30 minutes',
        method: 'Email + Chat',
      },
    ],
  },
  {
    id: 4,
    name: 'Low Priority - Daily Summary',
    description: 'For low severity alerts and maintenance notifications',
    active: false,
    escalationLevels: [
      {
        contact: 'Operations Manager',
        waitTime: '24 hours',
        method: 'Daily Email Digest',
      },
    ],
  },
]

const defaultSettings = reactive({
  criticalPolicy: 'immediate',
  highPolicy: 'delayed',
  escalationEnabled: true,
  maxLevels: 3,
})
</script>
