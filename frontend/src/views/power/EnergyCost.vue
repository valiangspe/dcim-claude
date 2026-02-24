<script setup lang="ts">
const summary = [
  { label: 'Avg Cost / kWh', value: '$0.087', color: 'primary' },
  { label: 'Monthly Bill', value: '$178,420', color: 'danger' },
  { label: 'YTD Spend', value: '$354,810', color: 'warning' },
  { label: 'Budget Remaining', value: '$745,190', color: 'success' },
]

const monthly = [
  { month: 'Sep 2025', kwh: 2040000, cost: 177480, rate: 0.087 },
  { month: 'Oct 2025', kwh: 2060000, cost: 179220, rate: 0.087 },
  { month: 'Nov 2025', kwh: 2010000, cost: 174870, rate: 0.087 },
  { month: 'Dec 2025', kwh: 2080000, cost: 182520, rate: 0.0878 },
  { month: 'Jan 2026', kwh: 2030000, cost: 176390, rate: 0.0869 },
  { month: 'Feb 2026', kwh: 2050000, cost: 178420, rate: 0.087 },
]

const maxCost = 200000
</script>

<template>
  <div>
    <h4 class="mb-4">Energy Cost & Billing</h4>

    <div class="row g-3 mb-4">
      <div v-for="s in summary" :key="s.label" class="col-sm-6 col-lg-3">
        <div class="card border-0 shadow-sm text-center h-100">
          <div class="card-body">
            <h3 :class="`text-${s.color} mb-0`">{{ s.value }}</h3>
            <small class="text-muted">{{ s.label }}</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Monthly Billing</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Month</th><th class="text-end">Consumption (kWh)</th><th class="text-end">Rate ($/kWh)</th><th class="text-end">Cost ($)</th><th style="width:25%">Bar</th></tr>
          </thead>
          <tbody>
            <tr v-for="m in monthly" :key="m.month">
              <td class="fw-semibold">{{ m.month }}</td>
              <td class="text-end">{{ m.kwh.toLocaleString() }}</td>
              <td class="text-end">{{ m.rate.toFixed(4) }}</td>
              <td class="text-end fw-semibold">${{ m.cost.toLocaleString() }}</td>
              <td>
                <div class="progress" style="height: 10px;">
                  <div class="progress-bar bg-danger" :style="{ width: (m.cost / maxCost * 100) + '%' }"></div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
