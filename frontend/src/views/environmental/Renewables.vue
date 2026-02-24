<template>
  <div class="renewables">
    <h2 class="mb-4">Renewable Energy</h2>

    <!-- Breakdown Charts -->
    <div class="row mb-4">
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Energy Source Breakdown</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <div class="d-flex justify-content-between mb-2">
                <span>Solar</span>
                <strong>18%</strong>
              </div>
              <div class="progress" style="height: 24px">
                <div class="progress-bar bg-warning" role="progressbar" style="width: 18%" aria-valuenow="18" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
            </div>
            <div class="mb-3">
              <div class="d-flex justify-content-between mb-2">
                <span>Wind</span>
                <strong>16%</strong>
              </div>
              <div class="progress" style="height: 24px">
                <div class="progress-bar bg-info" role="progressbar" style="width: 16%" aria-valuenow="16" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
            </div>
            <div>
              <div class="d-flex justify-content-between mb-2">
                <span>Grid (Renewable Credits)</span>
                <strong>66%</strong>
              </div>
              <div class="progress" style="height: 24px">
                <div class="progress-bar bg-success" role="progressbar" style="width: 66%" aria-valuenow="66" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Total Renewable Capacity</h5>
          </div>
          <div class="card-body">
            <div class="display-4 text-success mb-3">2,845 kW</div>
            <div class="row text-center">
              <div class="col-6">
                <small class="text-muted">Installed</small>
                <div class="h6">2,845 kW</div>
              </div>
              <div class="col-6">
                <small class="text-muted">Currently Generating</small>
                <div class="h6">1,246 kW</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Capacity vs Generation Table -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Renewable Assets - Capacity vs Generation</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Asset</th>
              <th>Type</th>
              <th>Capacity (kW)</th>
              <th>Current Output (kW)</th>
              <th>Efficiency</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Rooftop Solar Array A</td>
              <td>Solar</td>
              <td>400</td>
              <td>320</td>
              <td>80%</td>
              <td><span class="badge bg-success">Active</span></td>
            </tr>
            <tr>
              <td>Rooftop Solar Array B</td>
              <td>Solar</td>
              <td>350</td>
              <td>245</td>
              <td>70%</td>
              <td><span class="badge bg-success">Active</span></td>
            </tr>
            <tr>
              <td>Ground Mount Solar Farm</td>
              <td>Solar</td>
              <td>850</td>
              <td>612</td>
              <td>72%</td>
              <td><span class="badge bg-success">Active</span></td>
            </tr>
            <tr>
              <td>Wind Turbine 1</td>
              <td>Wind</td>
              <td>600</td>
              <td>42</td>
              <td>7%</td>
              <td><span class="badge bg-warning">Low Wind</span></td>
            </tr>
            <tr>
              <td>Wind Turbine 2</td>
              <td>Wind</td>
              <td>645</td>
              <td>27</td>
              <td>4%</td>
              <td><span class="badge bg-warning">Maintenance</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for Renewables component
// No imports needed
</script>

<style scoped>
.renewables {
  padding: 0;
}

.display-4 {
  font-weight: 700;
}
</style>
