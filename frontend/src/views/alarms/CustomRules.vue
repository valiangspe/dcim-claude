<template>
  <div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Custom Alert Rules</h1>
      <button class="btn btn-primary">+ Create Rule</button>
    </div>

    <!-- Alert Rules Table -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Active Alert Rules</h5>
      </div>
      <div class="table-responsive">
        <table class="table table-sm mb-0">
          <thead class="table-light">
            <tr>
              <th>Rule Name</th>
              <th>Condition</th>
              <th>Threshold</th>
              <th>Action</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="rule in alertRules" :key="rule.id">
              <td>{{ rule.name }}</td>
              <td>{{ rule.condition }}</td>
              <td>{{ rule.threshold }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ rule.action }}</span>
              </td>
              <td>
                <span :class="rule.enabled ? 'badge bg-success' : 'badge bg-secondary'">
                  {{ rule.enabled ? 'Enabled' : 'Disabled' }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1">Edit</button>
                <button class="btn btn-sm btn-outline-warning me-1">Disable</button>
                <button class="btn btn-sm btn-outline-danger">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Rule Statistics -->
    <div class="row mt-4">
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Total Rules</h6>
            <p class="h4 mb-0">{{ alertRules.length }}</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Active Rules</h6>
            <p class="h4 mb-0">{{ alertRules.filter((r) => r.enabled).length }}</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h6 class="card-title">Triggered (24h)</h6>
            <p class="h4 mb-0">24</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface AlertRule {
  id: number
  name: string
  condition: string
  threshold: string
  action: string
  enabled: boolean
}

const alertRules: AlertRule[] = [
  {
    id: 1,
    name: 'High CPU Utilization',
    condition: 'CPU > Threshold',
    threshold: '85%',
    action: 'Send Critical Alert',
    enabled: true,
  },
  {
    id: 2,
    name: 'Temperature Alarm',
    condition: 'Temperature > Threshold',
    threshold: '40°C',
    action: 'Send Critical Alert, Trigger Cooling',
    enabled: true,
  },
  {
    id: 3,
    name: 'Memory Pressure',
    condition: 'Memory Usage > Threshold',
    threshold: '90%',
    action: 'Send High Alert',
    enabled: true,
  },
  {
    id: 4,
    name: 'Power Anomaly Detection',
    condition: 'Power Deviation > Threshold',
    threshold: '±20%',
    action: 'Send High Alert, Notify Ops Team',
    enabled: true,
  },
  {
    id: 5,
    name: 'Disk Space Low',
    condition: 'Free Disk Space < Threshold',
    threshold: '10%',
    action: 'Send Medium Alert',
    enabled: true,
  },
  {
    id: 6,
    name: 'Network Interface Down',
    condition: 'Link Status = Down',
    threshold: 'N/A',
    action: 'Send Critical Alert, Escalate',
    enabled: true,
  },
  {
    id: 7,
    name: 'UPS Battery Low',
    condition: 'Battery Level < Threshold',
    threshold: '25%',
    action: 'Send High Alert',
    enabled: false,
  },
  {
    id: 8,
    name: 'Consecutive Failed Health Checks',
    condition: 'Failed Checks >= Count',
    threshold: '3 consecutive',
    action: 'Send Critical Alert',
    enabled: true,
  },
]
</script>
