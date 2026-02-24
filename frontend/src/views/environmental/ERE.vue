<template>
  <div class="ere">
    <h2 class="mb-4">Energy Reuse Effectiveness (ERE)</h2>

    <!-- ERE Main Card -->
    <div class="row mb-4">
      <div class="col-md-8">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">ERE Metric</h5>
            <div class="display-3 text-success">0.12</div>
            <p class="text-muted mb-3">Lower is better; represents % of energy reused</p>
            <div class="alert alert-info">
              <strong>Interpretation:</strong> 12% of the energy that would normally be lost is being captured and reused in facility operations.
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Energy Reuse Summary</h5>
            <div class="row text-center">
              <div class="col-6">
                <small class="text-muted">Total Captured</small>
                <div class="h6">42.4 MWh</div>
              </div>
              <div class="col-6">
                <small class="text-muted">Annual Value</small>
                <div class="h6">$3,821</div>
              </div>
            </div>
            <hr />
            <div class="text-center">
              <small class="text-muted">YoY Growth</small>
              <div class="h6 text-success">↑ 18%</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Reuse Sources Table -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Energy Reuse Sources</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Source</th>
              <th>Energy Recovered (MWh/year)</th>
              <th>Reuse Application</th>
              <th>Efficiency</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Hot Aisle Containment</td>
              <td>18.6</td>
              <td>Building Heating</td>
              <td>87%</td>
              <td><span class="badge bg-success">Active</span></td>
            </tr>
            <tr>
              <td>Chiller Waste Heat</td>
              <td>14.2</td>
              <td>Hot Water Supply</td>
              <td>82%</td>
              <td><span class="badge bg-success">Active</span></td>
            </tr>
            <tr>
              <td>Server Fan Exhaust</td>
              <td>6.8</td>
              <td>Facility Ventilation Preheat</td>
              <td>76%</td>
              <td><span class="badge bg-success">Active</span></td>
            </tr>
            <tr>
              <td>UPS Cooling Loop</td>
              <td>2.8</td>
              <td>Standby Area Heating</td>
              <td>71%</td>
              <td><span class="badge bg-warning">Monitoring</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Monthly Trend -->
    <div class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">Monthly Recovery Trend</h5>
      </div>
      <div class="card-body">
        <table class="table table-sm">
          <thead>
            <tr>
              <th>Month</th>
              <th>Energy Recovered (MWh)</th>
              <th>Reuse Rate (%)</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>January</td>
              <td>4.2</td>
              <td>14%</td>
            </tr>
            <tr>
              <td>February</td>
              <td>3.8</td>
              <td>12%</td>
            </tr>
            <tr>
              <td>March</td>
              <td>3.5</td>
              <td>11%</td>
            </tr>
            <tr>
              <td>April</td>
              <td>3.1</td>
              <td>10%</td>
            </tr>
            <tr>
              <td>May</td>
              <td>3.3</td>
              <td>10%</td>
            </tr>
            <tr>
              <td>June</td>
              <td>3.2</td>
              <td>10%</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for Energy Reuse Effectiveness component
// No imports needed
</script>

<style scoped>
.ere {
  padding: 0;
}

.display-3 {
  font-weight: 700;
}
</style>
