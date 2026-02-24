<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">Network Topology</h1>

    <div class="row mb-3">
      <div class="col-auto">
        <div class="btn-group btn-group-sm" role="group">
          <button type="button" class="btn" :class="selectedView === 'layer2' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedView = 'layer2'">Layer 2</button>
          <button type="button" class="btn" :class="selectedView === 'layer3' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedView = 'layer3'">Layer 3</button>
          <button type="button" class="btn" :class="selectedView === 'all' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedView = 'all'">All Layers</button>
        </div>
      </div>
    </div>

    <div class="card">
      <div class="card-body" style="background-color: #f8f9fa; min-height: 500px; position: relative;">
        <div class="topology-layer" style="margin-bottom: 40px;">
          <h6 class="text-muted mb-3">Core Layer</h6>
          <div class="d-flex gap-3 justify-content-center">
            <div v-for="device in coreDevices" :key="device.id" class="topology-node">
              <div class="node-box" :class="getNodeClass(device)"
                   style="width: 100px; padding: 10px; border: 2px solid #666; border-radius: 4px; text-align: center;">
                <div style="font-weight: bold; font-size: 12px;">{{ device.name }}</div>
                <div style="font-size: 10px; color: #666;">{{ device.status }}</div>
              </div>
            </div>
          </div>
        </div>

        <div class="topology-layer" style="margin-bottom: 40px;">
          <h6 class="text-muted mb-3">Distribution Layer</h6>
          <div class="d-flex gap-3 justify-content-center">
            <div v-for="device in distDevices" :key="device.id" class="topology-node">
              <div class="node-box" :class="getNodeClass(device)"
                   style="width: 90px; padding: 8px; border: 2px solid #999; border-radius: 4px; text-align: center; font-size: 11px;">
                {{ device.name }}
              </div>
            </div>
          </div>
        </div>

        <div class="topology-layer">
          <h6 class="text-muted mb-3">Access Layer</h6>
          <div class="d-flex gap-2 justify-content-center" style="flex-wrap: wrap;">
            <div v-for="device in accessDevices" :key="device.id" class="topology-node">
              <div class="node-box" :class="getNodeClass(device)"
                   style="width: 70px; padding: 6px; border: 1px solid #ccc; border-radius: 3px; text-align: center; font-size: 9px;">
                {{ device.name }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">Network Links</h5>
      </div>
      <div class="card-body">
        <table class="table table-sm mb-0">
          <thead class="table-light">
            <tr>
              <th>Source</th>
              <th>Destination</th>
              <th>Status</th>
              <th>Utilization</th>
              <th>Speed</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="link in links" :key="link.id">
              <td>{{ link.source }}</td>
              <td>{{ link.destination }}</td>
              <td>
                <span :class="['badge', link.status === 'Up' ? 'bg-success' : 'bg-danger']">
                  {{ link.status }}
                </span>
              </td>
              <td>{{ link.utilization }}%</td>
              <td>{{ link.speed }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const selectedView = ref('all');

const coreDevices = [
  { id: 1, name: 'CORE-01', status: 'UP', type: 'core' },
  { id: 2, name: 'CORE-02', status: 'UP', type: 'core' }
];

const distDevices = [
  { id: 3, name: 'DIST-01', status: 'UP', type: 'dist' },
  { id: 4, name: 'DIST-02', status: 'UP', type: 'dist' },
  { id: 5, name: 'DIST-03', status: 'UP', type: 'dist' },
  { id: 6, name: 'DIST-04', status: 'DOWN', type: 'dist' }
];

const accessDevices = [
  { id: 7, name: 'ACC-01', status: 'UP', type: 'access' },
  { id: 8, name: 'ACC-02', status: 'UP', type: 'access' },
  { id: 9, name: 'ACC-03', status: 'UP', type: 'access' },
  { id: 10, name: 'ACC-04', status: 'UP', type: 'access' },
  { id: 11, name: 'ACC-05', status: 'UP', type: 'access' },
  { id: 12, name: 'ACC-06', status: 'UP', type: 'access' },
  { id: 13, name: 'ACC-07', status: 'UP', type: 'access' },
  { id: 14, name: 'ACC-08', status: 'UP', type: 'access' }
];

const links = [
  { id: 1, source: 'CORE-01', destination: 'CORE-02', status: 'Up', utilization: 45, speed: '100 Gbps' },
  { id: 2, source: 'CORE-01', destination: 'DIST-01', status: 'Up', utilization: 62, speed: '100 Gbps' },
  { id: 3, source: 'CORE-01', destination: 'DIST-02', status: 'Up', utilization: 58, speed: '100 Gbps' },
  { id: 4, source: 'CORE-02', destination: 'DIST-03', status: 'Up', utilization: 71, speed: '100 Gbps' },
  { id: 5, source: 'CORE-02', destination: 'DIST-04', status: 'Down', utilization: 0, speed: '100 Gbps' },
  { id: 6, source: 'DIST-01', destination: 'ACC-01', status: 'Up', utilization: 35, speed: '40 Gbps' },
  { id: 7, source: 'DIST-02', destination: 'ACC-02', status: 'Up', utilization: 42, speed: '40 Gbps' }
];

function getNodeClass(device: { status: string }): string {
  if (device.status === 'UP') return 'bg-success';
  if (device.status === 'DOWN') return 'bg-danger';
  return 'bg-warning';
}
</script>
