<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">3D Visualization</h1>

    <div class="card">
      <div class="card-body text-center py-5">
        <div class="mb-4">
          <div style="font-size: 48px; color: #ccc;">
            <span>⚙</span>
          </div>
        </div>
        <h4 class="text-muted">3D View Requires WebGL Support</h4>
        <p class="text-muted mb-4">
          Your browser may not support WebGL, or it is disabled. Please enable it to view the 3D datacenter visualization.
        </p>
        <button class="btn btn-primary me-2" disabled>Load 3D Viewer</button>
        <button class="btn btn-outline-secondary">Use 2D Floor Plan Instead</button>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">Datacenter Statistics</h5>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-3 text-center">
            <h3 class="text-primary">{{ stats.totalRacks }}</h3>
            <p class="text-muted mb-0">Total Racks</p>
          </div>
          <div class="col-md-3 text-center">
            <h3 class="text-success">{{ stats.avgTemp }}°C</h3>
            <p class="text-muted mb-0">Average Temperature</p>
          </div>
          <div class="col-md-3 text-center">
            <h3 class="text-info">{{ stats.powerUsage }} kW</h3>
            <p class="text-muted mb-0">Current Power Usage</p>
          </div>
          <div class="col-md-3 text-center">
            <h3 class="text-warning">{{ stats.avgUtilization }}%</h3>
            <p class="text-muted mb-0">Average Utilization</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const stats = {
  totalRacks: 47,
  avgTemp: 24,
  powerUsage: 2845,
  avgUtilization: 62
};
</script>
