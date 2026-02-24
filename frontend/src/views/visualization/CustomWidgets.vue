<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Custom Widgets Gallery</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm">+ Add Widget</button>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <h5 class="mb-0">Real-Time Power Monitor</h5>
          </div>
          <div class="card-body">
            <div class="row text-center">
              <div class="col">
                <div style="font-size: 32px; color: #0d6efd; font-weight: bold;">2,850</div>
                <div class="text-muted">Current (kW)</div>
              </div>
              <div class="col">
                <div style="font-size: 32px; color: #198754; font-weight: bold;">2,780</div>
                <div class="text-muted">Average (kW)</div>
              </div>
              <div class="col">
                <div style="font-size: 32px; color: #dc3545; font-weight: bold;">3,120</div>
                <div class="text-muted">Peak (kW)</div>
              </div>
            </div>
            <div class="mt-3">
              <div class="progress" style="height: 25px;">
                <div class="progress-bar" style="width: 91%; background-color: #198754;">91% Capacity</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <h5 class="mb-0">Environmental Status</h5>
          </div>
          <div class="card-body">
            <div class="row text-center">
              <div class="col">
                <div style="font-size: 28px;">24.2°C</div>
                <div class="text-muted small">Inlet Temp</div>
                <span class="badge bg-success mt-1">Normal</span>
              </div>
              <div class="col">
                <div style="font-size: 28px;">52%</div>
                <div class="text-muted small">Humidity</div>
                <span class="badge bg-success mt-1">Optimal</span>
              </div>
              <div class="col">
                <div style="font-size: 28px;">+8 Pa</div>
                <div class="text-muted small">Pressure</div>
                <span class="badge bg-warning mt-1">Watch</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <h5 class="mb-0">Space Utilization</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <div class="d-flex justify-content-between mb-1">
                <span>Rack Density (Hall A)</span>
                <strong>85%</strong>
              </div>
              <div class="progress">
                <div class="progress-bar bg-warning" style="width: 85%;"></div>
              </div>
            </div>
            <div class="mb-3">
              <div class="d-flex justify-content-between mb-1">
                <span>Rack Density (Hall B)</span>
                <strong>62%</strong>
              </div>
              <div class="progress">
                <div class="progress-bar bg-info" style="width: 62%;"></div>
              </div>
            </div>
            <div>
              <div class="d-flex justify-content-between mb-1">
                <span>Rack Density (Hall C)</span>
                <strong>48%</strong>
              </div>
              <div class="progress">
                <div class="progress-bar bg-success" style="width: 48%;"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <h5 class="mb-0">Recent Alerts</h5>
          </div>
          <div class="card-body">
            <div v-for="alert in alerts" :key="alert.id" class="mb-2 pb-2 border-bottom">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                  <span :class="['badge', alert.severity === 'Critical' ? 'bg-danger' : alert.severity === 'Warning' ? 'bg-warning' : 'bg-info']">
                    {{ alert.severity }}
                  </span>
                  <div class="small mt-1">{{ alert.message }}</div>
                </div>
                <div class="text-muted small">{{ alert.time }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <h5 class="mb-0">Quick Stats</h5>
          </div>
          <div class="card-body">
            <div class="row text-center mb-3">
              <div class="col">
                <div style="font-size: 24px; color: #0d6efd; font-weight: bold;">47</div>
                <div class="text-muted small">Total Racks</div>
              </div>
              <div class="col">
                <div style="font-size: 24px; color: #198754; font-weight: bold;">892</div>
                <div class="text-muted small">Devices</div>
              </div>
              <div class="col">
                <div style="font-size: 24px; color: #ffc107; font-weight: bold;">12</div>
                <div class="text-muted small">Alerts</div>
              </div>
            </div>
            <button class="btn btn-sm btn-outline-primary w-100">View Details</button>
          </div>
        </div>
      </div>

      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <h5 class="mb-0">Capacity Forecast</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <div class="small text-muted mb-1">Power Capacity</div>
              <div class="progress" style="height: 20px;">
                <div class="progress-bar bg-success" style="width: 75%;">75% Used</div>
              </div>
              <div class="text-muted small mt-1">Est. Full: 6 months</div>
            </div>
            <div>
              <div class="small text-muted mb-1">Cooling Capacity</div>
              <div class="progress" style="height: 20px;">
                <div class="progress-bar bg-warning" style="width: 68%;">68% Used</div>
              </div>
              <div class="text-muted small mt-1">Est. Full: 8 months</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const alerts = [
  {
    id: 1,
    severity: 'Critical',
    message: 'High temperature detected in Hall B - CAB-B-02',
    time: '5 min ago'
  },
  {
    id: 2,
    severity: 'Warning',
    message: 'Power supply redundancy degraded on PDU-03',
    time: '15 min ago'
  },
  {
    id: 3,
    severity: 'Info',
    message: 'Sensor calibration completed for HVAC-01',
    time: '2 hours ago'
  },
  {
    id: 4,
    severity: 'Warning',
    message: 'Rack capacity utilization reaching 85% in Hall A',
    time: '4 hours ago'
  }
];
</script>
