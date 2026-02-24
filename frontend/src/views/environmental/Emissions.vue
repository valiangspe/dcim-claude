<template>
  <div class="emissions">
    <h2 class="mb-4">Emissions Breakdown</h2>

    <!-- Scope Cards -->
    <div class="row mb-4">
      <div class="col-md-4">
        <div class="card border-primary">
          <div class="card-header bg-primary text-white">
            <h5 class="mb-0">Scope 1 - Direct</h5>
          </div>
          <div class="card-body">
            <div class="display-5 text-primary">24.3</div>
            <p class="text-muted">Tonnes CO2e/year</p>
            <small class="text-muted">17% of total</small>
            <div class="mt-3">
              <div class="small mb-2">
                <div>Generators: 15.2 t</div>
                <div>Vehicles: 6.1 t</div>
                <div>Fugitive: 3.0 t</div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-warning">
          <div class="card-header bg-warning text-dark">
            <h5 class="mb-0">Scope 2 - Indirect Energy</h5>
          </div>
          <div class="card-body">
            <div class="display-5 text-warning">104.5</div>
            <p class="text-muted">Tonnes CO2e/year</p>
            <small class="text-muted">74% of total</small>
            <div class="mt-3">
              <div class="small mb-2">
                <div>Grid Electricity: 89.2 t</div>
                <div>Natural Gas: 15.3 t</div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-danger">
          <div class="card-header bg-danger text-white">
            <h5 class="mb-0">Scope 3 - Supply Chain</h5>
          </div>
          <div class="card-body">
            <div class="display-5 text-danger">13.2</div>
            <p class="text-muted">Tonnes CO2e/year</p>
            <small class="text-muted">9% of total</small>
            <div class="mt-3">
              <div class="small mb-2">
                <div>Business Travel: 5.8 t</div>
                <div>Waste: 4.2 t</div>
                <div>Supplies: 3.2 t</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Detailed Breakdown Table -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Complete Emissions Inventory</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Scope</th>
              <th>Category</th>
              <th>Activity</th>
              <th>CO2e (tonnes/year)</th>
              <th>% of Total</th>
              <th>Trend</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td><span class="badge bg-primary">1</span></td>
              <td>Combustion</td>
              <td>Backup Generators</td>
              <td>15.2</td>
              <td>10.7%</td>
              <td><span class="text-success">↓ 8%</span></td>
            </tr>
            <tr>
              <td><span class="badge bg-primary">1</span></td>
              <td>Mobile</td>
              <td>Fleet Vehicles</td>
              <td>6.1</td>
              <td>4.3%</td>
              <td><span class="text-success">↓ 12%</span></td>
            </tr>
            <tr>
              <td><span class="badge bg-primary">1</span></td>
              <td>Fugitive</td>
              <td>Refrigerant Leaks</td>
              <td>3.0</td>
              <td>2.1%</td>
              <td><span class="text-danger">↑ 5%</span></td>
            </tr>
            <tr>
              <td><span class="badge bg-warning">2</span></td>
              <td>Energy</td>
              <td>Grid Electricity</td>
              <td>89.2</td>
              <td>62.9%</td>
              <td><span class="text-success">↓ 6%</span></td>
            </tr>
            <tr>
              <td><span class="badge bg-warning">2</span></td>
              <td>Energy</td>
              <td>Natural Gas</td>
              <td>15.3</td>
              <td>10.8%</td>
              <td><span class="text-danger">↑ 3%</span></td>
            </tr>
            <tr>
              <td><span class="badge bg-danger">3</span></td>
              <td>Transport</td>
              <td>Business Travel</td>
              <td>5.8</td>
              <td>4.1%</td>
              <td><span class="text-success">↓ 15%</span></td>
            </tr>
            <tr>
              <td><span class="badge bg-danger">3</span></td>
              <td>Waste</td>
              <td>Landfill</td>
              <td>4.2</td>
              <td>3.0%</td>
              <td><span class="text-success">↓ 18%</span></td>
            </tr>
            <tr>
              <td><span class="badge bg-danger">3</span></td>
              <td>Procurement</td>
              <td>Equipment Supplies</td>
              <td>3.2</td>
              <td>2.3%</td>
              <td><span class="text-neutral">→ 0%</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for Emissions component
// No imports needed
</script>

<style scoped>
.emissions {
  padding: 0;
}

.display-5 {
  font-weight: 700;
}

.text-neutral {
  color: #6c757d;
}
</style>
