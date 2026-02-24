<template>
  <div class="standard-reports">
    <div class="card">
      <div class="card-header bg-primary text-white">
        <h5 class="mb-0">Standard Reports</h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div v-for="report in reports" :key="report.id" class="col-md-6">
            <div class="card border-start border-4" :style="{ borderColor: report.color }">
              <div class="card-body">
                <h6 class="card-title fw-bold">{{ report.name }}</h6>
                <p class="card-text text-muted small">{{ report.description }}</p>
                <div class="mb-3">
                  <small class="d-block text-muted">
                    <span class="badge" :style="{ backgroundColor: report.color }">
                      Last Run: {{ report.lastRun }}
                    </span>
                  </small>
                </div>
                <div class="d-flex gap-2">
                  <button class="btn btn-sm btn-primary flex-grow-1">Generate Now</button>
                  <button class="btn btn-sm btn-outline-secondary">View History</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer text-muted">
        Available reports: {{ reports.length }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Report {
  id: string
  name: string
  description: string
  color: string
  lastRun: string
}

const reports: Report[] = [
  {
    id: '1',
    name: 'Power Consumption Report',
    description: 'Monthly power usage, costs, and efficiency metrics',
    color: '#ffc107',
    lastRun: '2026-02-18'
  },
  {
    id: '2',
    name: 'Cooling Performance Report',
    description: 'Temperature monitoring and cooling efficiency analysis',
    color: '#0dcaf0',
    lastRun: '2026-02-19'
  },
  {
    id: '3',
    name: 'Space Utilization Report',
    description: 'Rack and floor space capacity analysis',
    color: '#198754',
    lastRun: '2026-02-17'
  },
  {
    id: '4',
    name: 'Equipment Inventory Report',
    description: 'Complete asset listing and depreciation tracking',
    color: '#6c757d',
    lastRun: '2026-02-15'
  },
  {
    id: '5',
    name: 'Network Performance Report',
    description: 'Bandwidth utilization and network health metrics',
    color: '#0d6efd',
    lastRun: '2026-02-16'
  },
  {
    id: '6',
    name: 'Compliance Report',
    description: 'ISO 22237 and regulatory compliance summary',
    color: '#dc3545',
    lastRun: '2026-02-14'
  }
]
</script>

<style scoped>
.standard-reports {
  padding: 1rem;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-2px);
}
</style>
