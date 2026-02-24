<script setup lang="ts">
const systems = [
  { name: 'CRAH-A1', status: 'running', uptime: 99.98, lastMaint: '2026-01-15', nextMaint: '2026-04-15', downtime: 0.4 },
  { name: 'CRAH-A2', status: 'running', uptime: 99.95, lastMaint: '2026-01-20', nextMaint: '2026-04-20', downtime: 1.2 },
  { name: 'CRAH-B1', status: 'running', uptime: 99.92, lastMaint: '2025-12-10', nextMaint: '2026-03-10', downtime: 2.1 },
  { name: 'CRAH-B2', status: 'warning', uptime: 99.85, lastMaint: '2025-11-28', nextMaint: '2026-02-28', downtime: 3.8 },
  { name: 'CRAC-C1', status: 'running', uptime: 99.97, lastMaint: '2026-02-01', nextMaint: '2026-05-01', downtime: 0.6 },
  { name: 'CRAC-C2', status: 'running', uptime: 99.94, lastMaint: '2026-01-25', nextMaint: '2026-04-25', downtime: 1.5 },
  { name: 'CRAH-D1', status: 'standby', uptime: 100, lastMaint: '2026-02-10', nextMaint: '2026-05-10', downtime: 0 },
  { name: 'Chiller-1', status: 'running', uptime: 99.96, lastMaint: '2025-12-20', nextMaint: '2026-03-20', downtime: 1.0 },
  { name: 'Chiller-2', status: 'running', uptime: 99.90, lastMaint: '2025-11-15', nextMaint: '2026-02-25', downtime: 2.4 },
  { name: 'Cooling Tower', status: 'running', uptime: 99.88, lastMaint: '2025-12-05', nextMaint: '2026-03-05', downtime: 2.8 },
]

function maintUrgency(next: string): string {
  const days = Math.ceil((new Date(next).getTime() - new Date('2026-02-20').getTime()) / 86400000)
  if (days <= 7) return 'text-danger fw-semibold'
  if (days <= 30) return 'text-warning'
  return ''
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cooling System Status</h4>

    <div class="card border-0 shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>System</th><th>Status</th><th class="text-end">Uptime %</th><th class="text-end">Downtime (h/mo)</th><th>Last Maint.</th><th>Next Maint.</th></tr>
            </thead>
            <tbody>
              <tr v-for="s in systems" :key="s.name">
                <td class="fw-semibold">{{ s.name }}</td>
                <td>
                  <span class="badge" :class="{ 'bg-success': s.status === 'running', 'bg-warning text-dark': s.status === 'warning', 'bg-secondary': s.status === 'standby' }">{{ s.status }}</span>
                </td>
                <td class="text-end">{{ s.uptime.toFixed(2) }}%</td>
                <td class="text-end">{{ s.downtime.toFixed(1) }}</td>
                <td>{{ s.lastMaint }}</td>
                <td :class="maintUrgency(s.nextMaint)">{{ s.nextMaint }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
