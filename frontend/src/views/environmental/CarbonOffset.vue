<template>
  <div class="carbon-offset">
    <h2 class="mb-4">Carbon Offset Programs</h2>

    <!-- Summary Cards -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Purchased</h5>
            <div class="display-5 text-info">185</div>
            <p class="text-muted">Credits (tonnes)</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Retired</h5>
            <div class="display-5 text-success">142</div>
            <p class="text-muted">Credits (tonnes)</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Remaining</h5>
            <div class="display-5 text-warning">43</div>
            <p class="text-muted">Credits (tonnes)</p>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Offset Ratio</h5>
            <div class="display-5 text-primary">100%</div>
            <p class="text-muted">Of emissions retired</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Offset Progress -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Annual Offset Progress</h5>
      </div>
      <div class="card-body">
        <div class="mb-3">
          <div class="d-flex justify-content-between mb-2">
            <span>Target Emissions</span>
            <strong>142 tonnes CO2e</strong>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-danger" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
          </div>
        </div>
        <div>
          <div class="d-flex justify-content-between mb-2">
            <span>Offset Coverage</span>
            <strong>142 tonnes CO2e</strong>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-success" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Offset Projects List -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Active Offset Projects</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Project</th>
              <th>Type</th>
              <th>Credits Purchased</th>
              <th>Credits Retired</th>
              <th>Location</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Reforestation Initiative</td>
              <td>Carbon Sequestration</td>
              <td>65 tonnes</td>
              <td>65 tonnes</td>
              <td>Brazil</td>
              <td><span class="badge bg-success">Verified</span></td>
            </tr>
            <tr>
              <td>Methane Capture - Landfill</td>
              <td>Methane Reduction</td>
              <td>48 tonnes</td>
              <td>48 tonnes</td>
              <td>USA</td>
              <td><span class="badge bg-success">Verified</span></td>
            </tr>
            <tr>
              <td>Wind Farm Development</td>
              <td>Renewable Energy</td>
              <td>55 tonnes</td>
              <td>29 tonnes</td>
              <td>India</td>
              <td><span class="badge bg-info">Active</span></td>
            </tr>
            <tr>
              <td>Solar Installation - Schools</td>
              <td>Renewable Energy</td>
              <td>17 tonnes</td>
              <td>0 tonnes</td>
              <td>Kenya</td>
              <td><span class="badge bg-warning">Pending</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for Carbon Offset component
// No imports needed
</script>

<style scoped>
.carbon-offset {
  padding: 0;
}

.display-5 {
  font-weight: 700;
}
</style>
