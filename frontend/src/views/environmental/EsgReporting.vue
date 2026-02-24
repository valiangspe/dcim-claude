<template>
  <div class="esg-reporting">
    <h2 class="mb-4">ESG Reporting & Compliance</h2>

    <!-- ESG Score Cards -->
    <div class="row mb-4">
      <div class="col-md-4">
        <div class="card border-success">
          <div class="card-header bg-success text-white">
            <h5 class="mb-0">Environmental Score</h5>
          </div>
          <div class="card-body">
            <div class="display-4 text-success">87</div>
            <p class="text-muted">out of 100</p>
            <div class="progress" style="height: 20px">
              <div class="progress-bar bg-success" role="progressbar" style="width: 87%" aria-valuenow="87" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
            <small class="text-success mt-2 d-block">↑ 5 points YoY</small>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-info">
          <div class="card-header bg-info text-white">
            <h5 class="mb-0">Social Score</h5>
          </div>
          <div class="card-body">
            <div class="display-4 text-info">76</div>
            <p class="text-muted">out of 100</p>
            <div class="progress" style="height: 20px">
              <div class="progress-bar bg-info" role="progressbar" style="width: 76%" aria-valuenow="76" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
            <small class="text-info mt-2 d-block">↑ 3 points YoY</small>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-primary">
          <div class="card-header bg-primary text-white">
            <h5 class="mb-0">Governance Score</h5>
          </div>
          <div class="card-body">
            <div class="display-4 text-primary">82</div>
            <p class="text-muted">out of 100</p>
            <div class="progress" style="height: 20px">
              <div class="progress-bar bg-primary" role="progressbar" style="width: 82%" aria-valuenow="82" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
            <small class="text-primary mt-2 d-block">↑ 2 points YoY</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Combined ESG Score -->
    <div class="row mb-4">
      <div class="col-md-12">
        <div class="card">
          <div class="card-body text-center">
            <h5 class="card-title">Overall ESG Score</h5>
            <div class="display-3 text-primary mb-3">82</div>
            <p class="text-muted">Composite ESG Performance (out of 100)</p>
            <div class="alert alert-success">
              Rating: <strong>A-</strong> (Top 15% of data centers in ESG performance)
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Framework Compliance -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Framework Compliance & Certifications</h5>
      </div>
      <div class="card-body">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Framework / Standard</th>
              <th>Compliance Area</th>
              <th>Status</th>
              <th>Last Verified</th>
              <th>Next Review</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td><strong>GRI Standards</strong></td>
              <td>Sustainability Reporting</td>
              <td><span class="badge bg-success">Compliant</span></td>
              <td>2025-12-15</td>
              <td>2026-12-15</td>
            </tr>
            <tr>
              <td><strong>SASB</strong></td>
              <td>Data Center Operations</td>
              <td><span class="badge bg-success">Compliant</span></td>
              <td>2025-11-30</td>
              <td>2026-11-30</td>
            </tr>
            <tr>
              <td><strong>TCFD</strong></td>
              <td>Climate Risk Disclosure</td>
              <td><span class="badge bg-success">Compliant</span></td>
              <td>2025-10-20</td>
              <td>2026-10-20</td>
            </tr>
            <tr>
              <td><strong>ISO 14001</strong></td>
              <td>Environmental Management</td>
              <td><span class="badge bg-success">Certified</span></td>
              <td>2024-06-01</td>
              <td>2027-06-01</td>
            </tr>
            <tr>
              <td><strong>ISO 45001</strong></td>
              <td>Occupational Health & Safety</td>
              <td><span class="badge bg-success">Certified</span></td>
              <td>2024-08-15</td>
              <td>2027-08-15</td>
            </tr>
            <tr>
              <td><strong>DCOI</strong></td>
              <td>Federal Data Center Optimization</td>
              <td><span class="badge bg-success">Compliant</span></td>
              <td>2025-12-31</td>
              <td>2026-03-31</td>
            </tr>
            <tr>
              <td><strong>EU Taxonomy</strong></td>
              <td>Sustainable Finance</td>
              <td><span class="badge bg-info">In Progress</span></td>
              <td>2025-09-10</td>
              <td>2026-06-30</td>
            </tr>
            <tr>
              <td><strong>Science Based Targets</strong></td>
              <td>Carbon Reduction Goals</td>
              <td><span class="badge bg-success">Approved</span></td>
              <td>2024-03-01</td>
              <td>2027-03-01</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Environmental Initiatives -->
    <div class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">Active ESG Initiatives</h5>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-md-6">
            <h6>Environmental Initiatives</h6>
            <ul class="list-unstyled">
              <li class="mb-2">
                <span class="badge bg-success">Active</span>
                <strong>Renewable Energy Expansion</strong> - Adding 500 kW solar capacity
              </li>
              <li class="mb-2">
                <span class="badge bg-success">Active</span>
                <strong>Waste Reduction Program</strong> - 85% diversion rate
              </li>
              <li class="mb-2">
                <span class="badge bg-success">Active</span>
                <strong>Water Conservation</strong> - Cooling efficiency improvements
              </li>
              <li class="mb-2">
                <span class="badge bg-warning">Planned</span>
                <strong>Battery Recycling</strong> - Partnership established
              </li>
            </ul>
          </div>
          <div class="col-md-6">
            <h6>Social & Governance Initiatives</h6>
            <ul class="list-unstyled">
              <li class="mb-2">
                <span class="badge bg-success">Active</span>
                <strong>Employee Wellness</strong> - Comprehensive health program
              </li>
              <li class="mb-2">
                <span class="badge bg-success">Active</span>
                <strong>Community Outreach</strong> - STEM education partnerships
              </li>
              <li class="mb-2">
                <span class="badge bg-success">Active</span>
                <strong>Diversity & Inclusion</strong> - 42% women in leadership
              </li>
              <li class="mb-2">
                <span class="badge bg-success">Active</span>
                <strong>Supply Chain Ethics</strong> - Vendor assessment program
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Mock data for ESG Reporting component
// No imports needed
</script>

<style scoped>
.esg-reporting {
  padding: 0;
}

.display-3,
.display-4 {
  font-weight: 700;
}

h6 {
  font-weight: 600;
  margin-bottom: 1rem;
}
</style>
