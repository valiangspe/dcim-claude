<script setup lang="ts">
const overallScore = 87

const categories = [
  { name: 'Power Health', score: 92, color: 'success' },
  { name: 'Cooling Health', score: 84, color: 'warning' },
  { name: 'Network Health', score: 96, color: 'success' },
  { name: 'Security Health', score: 78, color: 'warning' },
]

function scoreColor(score: number): string {
  if (score >= 90) return 'success'
  if (score >= 70) return 'warning'
  return 'danger'
}

const history = [
  { date: '2026-02-20', score: 87, change: 0 },
  { date: '2026-02-19', score: 87, change: 1 },
  { date: '2026-02-18', score: 86, change: -2 },
  { date: '2026-02-17', score: 88, change: 3 },
  { date: '2026-02-16', score: 85, change: -1 },
  { date: '2026-02-15', score: 86, change: 2 },
  { date: '2026-02-14', score: 84, change: -3 },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Health Score</h4>

    <!-- Overall Score -->
    <div class="row justify-content-center mb-4">
      <div class="col-md-4">
        <div :class="`card border-0 shadow-sm text-center bg-${scoreColor(overallScore)} bg-opacity-10`">
          <div class="card-body py-5">
            <div :class="`display-1 fw-bold text-${scoreColor(overallScore)}`">{{ overallScore }}</div>
            <div class="text-muted fs-5">/ 100</div>
            <div class="mt-2 fw-semibold">Overall Health Score</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Category Breakdown -->
    <div class="row g-3 mb-4">
      <div v-for="cat in categories" :key="cat.name" class="col-sm-6 col-lg-3">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-body text-center">
            <h2 :class="`text-${cat.color} mb-1`">{{ cat.score }}</h2>
            <div class="progress mb-2" style="height: 8px;">
              <div
                :class="`progress-bar bg-${cat.color}`"
                :style="{ width: cat.score + '%' }"
              ></div>
            </div>
            <small class="text-muted">{{ cat.name }}</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Historical Trend -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">7-Day Trend</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Date</th>
              <th>Score</th>
              <th style="width: 40%;">Bar</th>
              <th class="text-end">Change</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in history" :key="row.date">
              <td>{{ row.date }}</td>
              <td class="fw-semibold">{{ row.score }}</td>
              <td>
                <div class="progress" style="height: 10px;">
                  <div
                    :class="`progress-bar bg-${scoreColor(row.score)}`"
                    :style="{ width: row.score + '%' }"
                  ></div>
                </div>
              </td>
              <td class="text-end">
                <span v-if="row.change > 0" class="text-success">+{{ row.change }}</span>
                <span v-else-if="row.change < 0" class="text-danger">{{ row.change }}</span>
                <span v-else class="text-muted">0</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
