<script setup lang="ts">
const breakers = [
  { id: 'BRK-A1-01', pdu: 'PDU-A1', rating: 20, load: 16.8, circuits: 3, trip: false },
  { id: 'BRK-A1-02', pdu: 'PDU-A1', rating: 20, load: 14.2, circuits: 3, trip: false },
  { id: 'BRK-A1-03', pdu: 'PDU-A1', rating: 30, load: 27.5, circuits: 4, trip: false },
  { id: 'BRK-A2-01', pdu: 'PDU-A2', rating: 20, load: 18.9, circuits: 3, trip: true },
  { id: 'BRK-B1-01', pdu: 'PDU-B1', rating: 30, load: 22.1, circuits: 4, trip: false },
  { id: 'BRK-B1-02', pdu: 'PDU-B1', rating: 20, load: 15.4, circuits: 3, trip: false },
  { id: 'BRK-B2-01', pdu: 'PDU-B2', rating: 20, load: 12.8, circuits: 3, trip: false },
  { id: 'BRK-C1-01', pdu: 'PDU-C1', rating: 20, load: 8.2, circuits: 2, trip: false },
]

function utilColor(pct: number): string {
  if (pct >= 90) return 'danger'
  if (pct >= 75) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Breakers Utilization</h4>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Breaker</th>
              <th>PDU</th>
              <th class="text-end">Rating (A)</th>
              <th class="text-end">Load (A)</th>
              <th class="text-end">Circuits</th>
              <th style="width:22%">Utilization</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="b in breakers" :key="b.id" :class="b.trip ? 'table-danger' : ''">
              <td class="fw-semibold font-monospace">{{ b.id }}</td>
              <td>{{ b.pdu }}</td>
              <td class="text-end">{{ b.rating }}</td>
              <td class="text-end">{{ b.load.toFixed(1) }}</td>
              <td class="text-end">{{ b.circuits }}</td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="progress flex-fill" style="height: 8px;">
                    <div :class="`progress-bar bg-${utilColor(b.load / b.rating * 100)}`" :style="{ width: (b.load / b.rating * 100) + '%' }"></div>
                  </div>
                  <small>{{ (b.load / b.rating * 100).toFixed(0) }}%</small>
                </div>
              </td>
              <td>
                <span v-if="b.trip" class="badge bg-danger">TRIPPED</span>
                <span v-else class="badge bg-success">OK</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
