<script setup lang="ts">
const ports = [
  { pdu: 'PDU-A1', total: 42, used: 36, reserved: 2 },
  { pdu: 'PDU-A2', total: 42, used: 33, reserved: 3 },
  { pdu: 'PDU-B1', total: 42, used: 38, reserved: 1 },
  { pdu: 'PDU-B2', total: 42, used: 29, reserved: 4 },
  { pdu: 'PDU-C1', total: 42, used: 18, reserved: 0 },
]

const trend = [
  { month: 'Sep 2025', used: 138 }, { month: 'Oct 2025', used: 141 },
  { month: 'Nov 2025', used: 145 }, { month: 'Dec 2025', used: 148 },
  { month: 'Jan 2026', used: 151 }, { month: 'Feb 2026', used: 154 },
]

const totalPorts = ports.reduce((a, p) => a + p.total, 0)
const maxTrend = 210
</script>

<template>
  <div>
    <h4 class="mb-4">Port Capacity</h4>

    <div class="row g-3 mb-4">
      <div class="col-lg-7">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">Power Port Allocation</div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>PDU</th><th class="text-end">Total</th><th class="text-end">Used</th><th class="text-end">Reserved</th><th class="text-end">Free</th><th style="width:25%">Utilization</th></tr>
              </thead>
              <tbody>
                <tr v-for="p in ports" :key="p.pdu">
                  <td class="fw-semibold">{{ p.pdu }}</td>
                  <td class="text-end">{{ p.total }}</td>
                  <td class="text-end">{{ p.used }}</td>
                  <td class="text-end">{{ p.reserved }}</td>
                  <td class="text-end">{{ p.total - p.used - p.reserved }}</td>
                  <td>
                    <div class="progress" style="height: 8px;">
                      <div class="progress-bar bg-primary" :style="{ width: (p.used / p.total * 100) + '%' }"></div>
                      <div class="progress-bar bg-warning" :style="{ width: (p.reserved / p.total * 100) + '%' }"></div>
                    </div>
                  </td>
                </tr>
              </tbody>
              <tfoot class="table-light">
                <tr class="fw-semibold">
                  <td>Total</td>
                  <td class="text-end">{{ totalPorts }}</td>
                  <td class="text-end">{{ ports.reduce((a, p) => a + p.used, 0) }}</td>
                  <td class="text-end">{{ ports.reduce((a, p) => a + p.reserved, 0) }}</td>
                  <td class="text-end">{{ ports.reduce((a, p) => a + p.total - p.used - p.reserved, 0) }}</td>
                  <td></td>
                </tr>
              </tfoot>
            </table>
          </div>
        </div>
      </div>
      <div class="col-lg-5">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">Usage Trend</div>
          <div class="card-body p-0">
            <table class="table table-sm table-hover mb-0">
              <thead class="table-light">
                <tr><th>Month</th><th>Ports Used</th><th style="width:45%">Bar</th></tr>
              </thead>
              <tbody>
                <tr v-for="t in trend" :key="t.month">
                  <td>{{ t.month }}</td>
                  <td>{{ t.used }}</td>
                  <td>
                    <div class="progress" style="height: 10px;">
                      <div class="progress-bar bg-info" :style="{ width: (t.used / maxTrend * 100) + '%' }"></div>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
