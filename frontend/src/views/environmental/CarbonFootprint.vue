<template>
  <div class="carbon-footprint">
    <h2 class="mb-4">Carbon Footprint</h2>

    <!-- Total CO2e Card -->
    <div class="row mb-4">
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Total CO2e Emissions</h5>
            <div class="display-4 text-danger">142</div>
            <p class="text-muted">Tonnes per year</p>
            <small class="text-success">↓ 12% vs last year</small>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Monthly Average</h5>
            <div class="display-4 text-info">11.8</div>
            <p class="text-muted">Tonnes per month</p>
            <small class="text-warning">Trending stable</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Monthly Breakdown Table -->
    <div class="card mb-4">
      <div class="card-header">
        <h5 class="mb-0">Monthly Breakdown</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Month</th>
              <th>CO2e (tonnes)</th>
              <th>Trend</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>January</td>
              <td>13.2</td>
              <td><span class="text-danger">↑ 5%</span></td>
              <td><span class="badge bg-warning">Above Avg</span></td>
            </tr>
            <tr>
              <td>February</td>
              <td>12.8</td>
              <td><span class="text-success">↓ 3%</span></td>
              <td><span class="badge bg-info">On Track</span></td>
            </tr>
            <tr>
              <td>March</td>
              <td>11.5</td>
              <td><span class="text-success">↓ 10%</span></td>
              <td><span class="badge bg-success">Good</span></td>
            </tr>
            <tr>
              <td>April</td>
              <td>10.9</td>
              <td><span class="text-success">↓ 5%</span></td>
              <td><span class="badge bg-success">Good</span></td>
            </tr>
            <tr>
              <td>May</td>
              <td>11.2</td>
              <td><span class="text-danger">↑ 2%</span></td>
              <td><span class="badge bg-info">On Track</span></td>
            </tr>
            <tr>
              <td>June</td>
              <td>10.8</td>
              <td><span class="text-success">↓ 4%</span></td>
              <td><span class="badge bg-success">Good</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Reduction Trend -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Reduction Trend (YoY)</h5>
      </div>
      <div class="card-body">
        <div class="mb-3">
          <div class="d-flex justify-content-between mb-2">
            <span>2023</span>
            <span class="fw-bold">162 tonnes</span>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-danger" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
          </div>
        </div>
        <div class="mb-3">
          <div class="d-flex justify-content-between mb-2">
            <span>2024</span>
            <span class="fw-bold">152 tonnes</span>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-warning" role="progressbar" style="width: 94%" aria-valuenow="94" aria-valuemin="0" aria-valuemax="100"></div>
          </div>
        </div>
        <div>
          <div class="d-flex justify-content-between mb-2">
            <span>2025</span>
            <span class="fw-bold">142 tonnes</span>
          </div>
          <div class="progress" style="height: 24px">
            <div class="progress-bar bg-success" role="progressbar" style="width: 88%" aria-valuenow="88" aria-valuemin="0" aria-valuemax="100"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for Carbon Footprint component
// No imports needed
</script>

<style scoped>
.carbon-footprint {
  padding: 0;
}

.display-4 {
  font-weight: 700;
}
</style>
