<template>
  <div class="energy-storage">
    <h2 class="mb-4">Energy Storage Systems</h2>

    <!-- Summary Cards -->
    <div class="row mb-4">
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Total Capacity</h5>
            <div class="display-5 text-primary">8,500</div>
            <p class="text-muted">kWh</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Available Energy</h5>
            <div class="display-5 text-success">6,835</div>
            <p class="text-muted">kWh (80.4%)</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Cycle Efficiency</h5>
            <div class="display-5 text-info">91%</div>
            <p class="text-muted">Round-trip efficiency</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Storage Systems Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Storage Systems - Capacity & Status</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>System</th>
              <th>Type</th>
              <th>Capacity (kWh)</th>
              <th>SOC (%)</th>
              <th>Status</th>
              <th>Response Time</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Battery Pack A</td>
              <td>Lithium-ion</td>
              <td>3,000</td>
              <td>
                <div class="progress" style="height: 20px">
                  <div class="progress-bar bg-success" role="progressbar" style="width: 85%" aria-valuenow="85" aria-valuemin="0" aria-valuemax="100"></div>
                </div>
                <small>85%</small>
              </td>
              <td><span class="badge bg-success">Ready</span></td>
              <td>12ms</td>
            </tr>
            <tr>
              <td>Battery Pack B</td>
              <td>Lithium-ion</td>
              <td>2,500</td>
              <td>
                <div class="progress" style="height: 20px">
                  <div class="progress-bar bg-info" role="progressbar" style="width: 72%" aria-valuenow="72" aria-valuemin="0" aria-valuemax="100"></div>
                </div>
                <small>72%</small>
              </td>
              <td><span class="badge bg-success">Ready</span></td>
              <td>15ms</td>
            </tr>
            <tr>
              <td>Flywheel System 1</td>
              <td>Mechanical</td>
              <td>1,500</td>
              <td>
                <div class="progress" style="height: 20px">
                  <div class="progress-bar bg-warning" role="progressbar" style="width: 92%" aria-valuenow="92" aria-valuemin="0" aria-valuemax="100"></div>
                </div>
                <small>92%</small>
              </td>
              <td><span class="badge bg-success">Ready</span></td>
              <td>8ms</td>
            </tr>
            <tr>
              <td>Flywheel System 2</td>
              <td>Mechanical</td>
              <td>1,500</td>
              <td>
                <div class="progress" style="height: 20px">
                  <div class="progress-bar bg-warning" role="progressbar" style="width: 68%" aria-valuenow="68" aria-valuemin="0" aria-valuemax="100"></div>
                </div>
                <small>68%</small>
              </td>
              <td><span class="badge bg-warning">Charging</span></td>
              <td>8ms</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Performance Details -->
    <div class="row">
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Monthly Energy Discharged</h5>
          </div>
          <div class="card-body">
            <table class="table table-sm">
              <thead>
                <tr>
                  <th>Month</th>
                  <th>Discharged (kWh)</th>
                  <th>Cycles</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>January</td>
                  <td>1,240</td>
                  <td>3</td>
                </tr>
                <tr>
                  <td>February</td>
                  <td>985</td>
                  <td>2</td>
                </tr>
                <tr>
                  <td>March</td>
                  <td>1,560</td>
                  <td>4</td>
                </tr>
                <tr>
                  <td>April</td>
                  <td>2,140</td>
                  <td>5</td>
                </tr>
                <tr>
                  <td>May</td>
                  <td>1,820</td>
                  <td>4</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Health & Maintenance</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <div class="d-flex justify-content-between mb-2">
                <span>Battery Pack A - Health</span>
                <strong>96%</strong>
              </div>
              <div class="progress" style="height: 20px">
                <div class="progress-bar bg-success" role="progressbar" style="width: 96%" aria-valuenow="96" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
            </div>
            <div class="mb-3">
              <div class="d-flex justify-content-between mb-2">
                <span>Battery Pack B - Health</span>
                <strong>89%</strong>
              </div>
              <div class="progress" style="height: 20px">
                <div class="progress-bar bg-warning" role="progressbar" style="width: 89%" aria-valuenow="89" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
            </div>
            <div class="mb-3">
              <div class="d-flex justify-content-between mb-2">
                <span>Flywheel 1 - Health</span>
                <strong>98%</strong>
              </div>
              <div class="progress" style="height: 20px">
                <div class="progress-bar bg-success" role="progressbar" style="width: 98%" aria-valuenow="98" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
            </div>
            <div>
              <div class="d-flex justify-content-between mb-2">
                <span>Flywheel 2 - Health</span>
                <strong>94%</strong>
              </div>
              <div class="progress" style="height: 20px">
                <div class="progress-bar bg-success" role="progressbar" style="width: 94%" aria-valuenow="94" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for Energy Storage component
// No imports needed
</script>

<style scoped>
.energy-storage {
  padding: 0;
}

.display-5 {
  font-weight: 700;
}
</style>
