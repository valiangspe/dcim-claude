<template>
  <div class="dcoi-compliance">
    <h2 class="mb-4">DCOI Compliance</h2>

    <!-- Compliance Score Card -->
    <div class="row mb-4">
      <div class="col-md-12">
        <div class="card">
          <div class="card-body text-center">
            <h5 class="card-title">Overall DCOI Compliance Score</h5>
            <div class="display-2 text-success mb-3">94/100</div>
            <p class="text-muted mb-3">Excellent compliance with data center optimization initiative</p>
            <div class="alert alert-success">
              Facility exceeds federal data center energy efficiency requirements
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Metrics Checklist -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">DCOI Metrics Assessment</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Metric</th>
              <th>Requirement</th>
              <th>Current Value</th>
              <th>Status</th>
              <th>Score</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>
                <strong>PUE (Power Usage Effectiveness)</strong>
                <br />
                <small class="text-muted">Facility energy efficiency</small>
              </td>
              <td>&lt; 1.50</td>
              <td>1.47</td>
              <td><span class="badge bg-success">PASS</span></td>
              <td><strong>20/20</strong></td>
            </tr>
            <tr>
              <td>
                <strong>Renewable Energy</strong>
                <br />
                <small class="text-muted">% of total consumption</small>
              </td>
              <td>&gt; 25%</td>
              <td>34%</td>
              <td><span class="badge bg-success">PASS</span></td>
              <td><strong>20/20</strong></td>
            </tr>
            <tr>
              <td>
                <strong>Water Usage Efficiency</strong>
                <br />
                <small class="text-muted">WUE - L/kWh</small>
              </td>
              <td>&lt; 1.2</td>
              <td>0.89</td>
              <td><span class="badge bg-success">PASS</span></td>
              <td><strong>18/20</strong></td>
            </tr>
            <tr>
              <td>
                <strong>Energy Reuse Effectiveness</strong>
                <br />
                <small class="text-muted">% of energy reused</small>
              </td>
              <td>&gt; 10%</td>
              <td>12%</td>
              <td><span class="badge bg-success">PASS</span></td>
              <td><strong>16/20</strong></td>
            </tr>
            <tr>
              <td>
                <strong>Metering & Monitoring</strong>
                <br />
                <small class="text-muted">Data center monitoring systems</small>
              </td>
              <td>Comprehensive</td>
              <td>Comprehensive</td>
              <td><span class="badge bg-success">PASS</span></td>
              <td><strong>20/20</strong></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Compliance Details -->
    <div class="row mt-4">
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Target Achievements</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span>PUE Target (1.50 max)</span>
                <span class="badge bg-success">Exceed</span>
              </div>
              <div class="progress" style="height: 20px">
                <div class="progress-bar bg-success" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
              <small class="text-success">1.47 (Beat target by 0.03)</small>
            </div>
            <div class="mb-3">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span>Renewable Energy (25% min)</span>
                <span class="badge bg-success">Exceed</span>
              </div>
              <div class="progress" style="height: 20px">
                <div class="progress-bar bg-success" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
              <small class="text-success">34% (Beat target by 9%)</small>
            </div>
            <div>
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span>WUE Target (1.20 max)</span>
                <span class="badge bg-success">Exceed</span>
              </div>
              <div class="progress" style="height: 20px">
                <div class="progress-bar bg-success" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
              </div>
              <small class="text-success">0.89 (Beat target by 0.31)</small>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Reporting Status</h5>
          </div>
          <div class="card-body">
            <table class="table table-sm">
              <tbody>
                <tr>
                  <td><strong>Last Reported</strong></td>
                  <td>2025-12-31</td>
                </tr>
                <tr>
                  <td><strong>Next Report Due</strong></td>
                  <td>2026-03-31</td>
                </tr>
                <tr>
                  <td><strong>Audit Status</strong></td>
                  <td><span class="badge bg-success">Current</span></td>
                </tr>
                <tr>
                  <td><strong>Certification</strong></td>
                  <td><span class="badge bg-success">Valid</span></td>
                </tr>
              </tbody>
            </table>
            <div class="alert alert-info mt-3">
              <small>Data center meets all DCOI goals and is positioned for continued compliance in 2026.</small>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for DCOI Compliance component
// No imports needed
</script>

<style scoped>
.dcoi-compliance {
  padding: 0;
}

.display-2 {
  font-weight: 700;
}
</style>
