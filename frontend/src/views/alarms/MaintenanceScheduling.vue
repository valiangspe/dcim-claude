<template>
  <div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Maintenance Scheduling</h1>
      <button class="btn btn-primary">+ Schedule Maintenance</button>
    </div>

    <!-- Maintenance Windows Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Scheduled Maintenance Windows</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>System/Component</th>
              <th>Start Time</th>
              <th>End Time</th>
              <th>Duration</th>
              <th>Impact</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="maintenance in maintenanceWindows" :key="maintenance.id">
              <td>{{ maintenance.component }}</td>
              <td>{{ formatDateTime(maintenance.startTime) }}</td>
              <td>{{ formatDateTime(maintenance.endTime) }}</td>
              <td>{{ calculateDuration(maintenance.startTime, maintenance.endTime) }}</td>
              <td>
                <span :class="getImpactBadgeClass(maintenance.impact)">
                  {{ maintenance.impact }}
                </span>
              </td>
              <td>
                <span :class="getStatusBadgeClass(maintenance.status)">
                  {{ maintenance.status }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1">Edit</button>
                <button class="btn btn-sm btn-outline-danger">Cancel</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Maintenance Policy Settings -->
    <div class="row">
      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Maintenance Windows Policy</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Suppress Alarms During Maintenance</label>
              <div class="form-check form-switch">
                <input
                  v-model="maintenancePolicy.suppressAlarms"
                  type="checkbox"
                  class="form-check-input"
                  id="suppressAlarms"
                />
                <label class="form-check-label" for="suppressAlarms">
                  {{ maintenancePolicy.suppressAlarms ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Minimum Notice Required (hours)</label>
              <input
                v-model.number="maintenancePolicy.minimumNotice"
                type="number"
                class="form-control"
                min="1"
                max="168"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Maximum Duration (hours)</label>
              <input
                v-model.number="maintenancePolicy.maxDuration"
                type="number"
                class="form-control"
                min="1"
                max="168"
              />
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Notification Settings</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Notify Before Maintenance Starts</label>
              <select v-model="maintenancePolicy.preNotification" class="form-select">
                <option value="1h">1 hour before</option>
                <option value="30min">30 minutes before</option>
                <option value="15min">15 minutes before</option>
                <option value="5min">5 minutes before</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Notify After Maintenance Completes</label>
              <div class="form-check form-switch">
                <input
                  v-model="maintenancePolicy.postNotification"
                  type="checkbox"
                  class="form-check-input"
                  id="postNotification"
                />
                <label class="form-check-label" for="postNotification">
                  {{ maintenancePolicy.postNotification ? 'Yes' : 'No' }}
                </label>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Notification Recipients</label>
              <input
                v-model="maintenancePolicy.recipients"
                type="text"
                class="form-control"
                placeholder="ops-team@company.com, manager@company.com"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Save Button -->
    <div class="mt-4">
      <button class="btn btn-primary btn-lg">Save Policy Settings</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'

interface MaintenanceWindow {
  id: number
  component: string
  startTime: Date
  endTime: Date
  impact: 'Critical' | 'High' | 'Medium' | 'Low'
  status: 'Scheduled' | 'In Progress' | 'Completed' | 'Cancelled'
}

const maintenanceWindows: MaintenanceWindow[] = [
  {
    id: 1,
    component: 'Cooling Unit A-01 Maintenance',
    startTime: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000),
    endTime: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000 + 4 * 60 * 60 * 1000),
    impact: 'High',
    status: 'Scheduled',
  },
  {
    id: 2,
    component: 'Network Switch D-02 Firmware Update',
    startTime: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000),
    endTime: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000 + 2 * 60 * 60 * 1000),
    impact: 'Medium',
    status: 'Scheduled',
  },
  {
    id: 3,
    component: 'UPS Battery Replacement',
    startTime: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
    endTime: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000 + 6 * 60 * 60 * 1000),
    impact: 'Critical',
    status: 'Scheduled',
  },
  {
    id: 4,
    component: 'Server Rack A Power Distribution Upgrade',
    startTime: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
    endTime: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000 + 8 * 60 * 60 * 1000),
    impact: 'High',
    status: 'Completed',
  },
  {
    id: 5,
    component: 'HVAC Filter Replacement',
    startTime: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
    endTime: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000 + 1 * 60 * 60 * 1000),
    impact: 'Low',
    status: 'Completed',
  },
]

const maintenancePolicy = reactive({
  suppressAlarms: true,
  minimumNotice: 24,
  maxDuration: 12,
  preNotification: '1h',
  postNotification: true,
  recipients: 'ops-team@company.com, manager@company.com',
})

function formatDateTime(date: Date): string {
  return date.toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function calculateDuration(start: Date, end: Date): string {
  const diffMs = end.getTime() - start.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)

  if (diffHours < 1) return `${diffMins}m`
  const remainingMins = diffMins % 60
  return `${diffHours}h ${remainingMins}m`
}

function getImpactBadgeClass(impact: string): string {
  const classes: Record<string, string> = {
    Critical: 'badge bg-danger',
    High: 'badge bg-warning text-dark',
    Medium: 'badge bg-info',
    Low: 'badge bg-secondary',
  }
  return classes[impact] || 'badge bg-secondary'
}

function getStatusBadgeClass(status: string): string {
  const classes: Record<string, string> = {
    Scheduled: 'badge bg-info',
    'In Progress': 'badge bg-warning text-dark',
    Completed: 'badge bg-success',
    Cancelled: 'badge bg-danger',
  }
  return classes[status] || 'badge bg-secondary'
}
</script>
