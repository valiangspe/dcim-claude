<template>
  <div class="pue-trends">
    <div class="card">
      <div class="card-header bg-info text-white">
        <h5 class="mb-0">PUE Trends - Monthly Analysis</h5>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover">
            <thead>
              <tr class="table-light">
                <th>Month</th>
                <th>PUE Value</th>
                <th>Trend</th>
                <th>Progress</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="trend in pueTrends" :key="trend.month">
                <td><strong>{{ trend.month }}</strong></td>
                <td>{{ trend.pueValue }}</td>
                <td>
                  <span :class="getTrendBadgeClass(trend.trend)">
                    {{ trend.trend }}
                  </span>
                </td>
                <td>
                  <div class="progress" style="height: 20px">
                    <div
                      class="progress-bar"
                      :style="{ width: trend.progressPercent + '%', backgroundColor: getProgressColor(trend.pueValue) }"
                      role="progressbar"
                    >
                      <small class="text-white">{{ trend.progressPercent }}%</small>
                    </div>
                  </div>
                </td>
                <td>
                  <span :class="getStatusBadgeClass(trend.status)">
                    {{ trend.status }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer text-muted">
        Target PUE: 1.5 | Current: {{ pueTrends[0].pueValue }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface PueTrend {
  month: string
  pueValue: number
  trend: string
  progressPercent: number
  status: string
}

const pueTrends: PueTrend[] = [
  { month: 'February 2026', pueValue: 1.62, trend: 'Down', progressPercent: 92, status: 'On Track' },
  { month: 'January 2026', pueValue: 1.68, trend: 'Down', progressPercent: 89, status: 'Good' },
  { month: 'December 2025', pueValue: 1.75, trend: 'Up', progressPercent: 85, status: 'Fair' },
  { month: 'November 2025', pueValue: 1.72, trend: 'Down', progressPercent: 86, status: 'Fair' },
  { month: 'October 2025', pueValue: 1.81, trend: 'Up', progressPercent: 82, status: 'Poor' },
  { month: 'September 2025', pueValue: 1.78, trend: 'Down', progressPercent: 84, status: 'Fair' },
  { month: 'August 2025', pueValue: 1.85, trend: 'Up', progressPercent: 80, status: 'Poor' },
  { month: 'July 2025', pueValue: 1.82, trend: 'Up', progressPercent: 81, status: 'Poor' }
]

const getTrendBadgeClass = (trend: string): string => {
  const classes = 'badge'
  if (trend === 'Down') return `${classes} bg-success`
  if (trend === 'Up') return `${classes} bg-danger`
  return `${classes} bg-warning text-dark`
}

const getStatusBadgeClass = (status: string): string => {
  const classes = 'badge'
  switch (status) {
    case 'On Track':
      return `${classes} bg-success`
    case 'Good':
      return `${classes} bg-info`
    case 'Fair':
      return `${classes} bg-warning text-dark`
    case 'Poor':
      return `${classes} bg-danger`
    default:
      return `${classes} bg-secondary`
  }
}

const getProgressColor = (pueValue: number): string => {
  if (pueValue <= 1.5) return '#198754'
  if (pueValue <= 1.6) return '#0dcaf0'
  if (pueValue <= 1.75) return '#ffc107'
  return '#dc3545'
}
</script>

<style scoped>
.pue-trends {
  padding: 1rem;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}
</style>
