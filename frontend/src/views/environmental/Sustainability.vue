<template>
  <div class="sustainability">
    <h2 class="mb-4">Sustainability Metrics</h2>

    <!-- Key Metrics Cards -->
    <div class="row mb-4">
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Renewable Energy</h5>
            <div class="display-4 text-success">34%</div>
            <p class="text-muted">Of total energy consumption</p>
            <div class="progress" style="height: 20px">
              <div class="progress-bar bg-success" role="progressbar" style="width: 34%" aria-valuenow="34" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">PUE (Power Usage Effectiveness)</h5>
            <div class="display-4 text-info">1.47</div>
            <p class="text-muted">Lower is better (industry avg: 1.58)</p>
            <small class="text-success">↓ 0.05 improvement</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Additional Metrics -->
    <div class="row mb-4">
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">WUE</h5>
            <div class="display-5 text-primary">0.89</div>
            <p class="text-muted">L/kWh (Water Usage)</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Carbon Intensity</h5>
            <div class="display-5 text-warning">78</div>
            <p class="text-muted">gCO2e/kWh</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Energy Efficiency</h5>
            <div class="display-5 text-success">92%</div>
            <p class="text-muted">IT equipment utilization</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Monthly Trend Chart -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Monthly Trend</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Month</th>
              <th>Renewable %</th>
              <th>PUE</th>
              <th>WUE</th>
              <th>Carbon (gCO2/kWh)</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>January</td>
              <td>32%</td>
              <td>1.52</td>
              <td>0.92</td>
              <td>82</td>
            </tr>
            <tr>
              <td>February</td>
              <td>33%</td>
              <td>1.50</td>
              <td>0.91</td>
              <td>80</td>
            </tr>
            <tr>
              <td>March</td>
              <td>35%</td>
              <td>1.48</td>
              <td>0.89</td>
              <td>76</td>
            </tr>
            <tr>
              <td>April</td>
              <td>36%</td>
              <td>1.46</td>
              <td>0.88</td>
              <td>74</td>
            </tr>
            <tr>
              <td>May</td>
              <td>34%</td>
              <td>1.47</td>
              <td>0.89</td>
              <td>78</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for Sustainability Metrics component
// No imports needed
</script>

<style scoped>
.sustainability {
  padding: 0;
}

.display-4,
.display-5 {
  font-weight: 700;
}
</style>
