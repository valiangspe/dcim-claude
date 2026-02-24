<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Sensor Configuration</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm">+ Add Sensor</button>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Sensor ID</th>
                <th>Type</th>
                <th>Location</th>
                <th>Min Threshold</th>
                <th>Max Threshold</th>
                <th>Polling Interval</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="sensor in sensors" :key="sensor.id">
                <td class="font-monospace small">{{ sensor.id }}</td>
                <td>
                  <span class="badge bg-info">{{ sensor.type }}</span>
                </td>
                <td class="small">{{ sensor.location }}</td>
                <td>{{ sensor.minThreshold }}</td>
                <td>{{ sensor.maxThreshold }}</td>
                <td class="small">{{ sensor.pollingInterval }}</td>
                <td>
                  <span :class="['badge', sensor.status === 'Online' ? 'bg-success' : 'bg-secondary']">
                    {{ sensor.status }}
                  </span>
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
const sensors = [
  {
    id: 'TEMP-HALL-A-01',
    type: 'Temperature',
    location: 'Hall A Inlet',
    minThreshold: '18°C',
    maxThreshold: '27°C',
    pollingInterval: '30 seconds',
    status: 'Online'
  },
  {
    id: 'TEMP-HALL-B-01',
    type: 'Temperature',
    location: 'Hall B Outlet',
    minThreshold: '18°C',
    maxThreshold: '35°C',
    pollingInterval: '30 seconds',
    status: 'Online'
  },
  {
    id: 'HUMID-HALL-A-01',
    type: 'Humidity',
    location: 'Hall A',
    minThreshold: '20%',
    maxThreshold: '80%',
    pollingInterval: '60 seconds',
    status: 'Online'
  },
  {
    id: 'HUMID-HALL-B-01',
    type: 'Humidity',
    location: 'Hall B',
    minThreshold: '20%',
    maxThreshold: '80%',
    pollingInterval: '60 seconds',
    status: 'Online'
  },
  {
    id: 'POWER-PDU-01',
    type: 'Power',
    location: 'PDU Row 1',
    minThreshold: '0 kW',
    maxThreshold: '120 kW',
    pollingInterval: '5 seconds',
    status: 'Online'
  },
  {
    id: 'AIRFLOW-A-01',
    type: 'Air Flow',
    location: 'CRAC Unit A',
    minThreshold: '1000 CFM',
    maxThreshold: '5000 CFM',
    pollingInterval: '30 seconds',
    status: 'Online'
  },
  {
    id: 'PRESSURE-ROOM-01',
    type: 'Pressure',
    location: 'Cold Aisle 1',
    minThreshold: '-12 Pa',
    maxThreshold: '12 Pa',
    pollingInterval: '120 seconds',
    status: 'Online'
  },
  {
    id: 'WATER-LEAK-01',
    type: 'Water Leak Detection',
    location: 'Cooling Loop A',
    minThreshold: 'None',
    maxThreshold: 'Alert',
    pollingInterval: '30 seconds',
    status: 'Online'
  }
];
</script>
