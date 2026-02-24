<script setup lang="ts">
const scenarios = [
  {
    name: 'Add 20 GPU servers',
    impact: { racks: 5, power: 48, cooling: 52, space: '210U' },
    feasibility: 'possible',
    notes: 'Requires Hall B expansion. Cooling upgrade needed in Q3.',
  },
  {
    name: 'Consolidate Hall D into Hall C',
    impact: { racks: -8, power: -12, cooling: -8, space: '-336U freed' },
    feasibility: 'recommended',
    notes: 'Frees Hall D for decommission. No cooling changes needed.',
  },
  {
    name: 'Deploy 100 new 1U nodes',
    impact: { racks: 3, power: 24, cooling: 18, space: '100U' },
    feasibility: 'possible',
    notes: 'Gap analysis shows sufficient 1U slots in existing racks. Minimal impact.',
  },
  {
    name: 'Add redundant cooling to Hall A',
    impact: { racks: 0, power: 8, cooling: -20, space: '0U' },
    feasibility: 'recommended',
    notes: 'Improves Hall A cooling headroom from 15% to 35%.',
  },
  {
    name: 'Double storage capacity',
    impact: { racks: 10, power: 32, cooling: 25, space: '420U' },
    feasibility: 'constrained',
    notes: 'Exceeds current floor space in Hall C. Requires new hall or offsite.',
  },
]

function feasBadge(f: string): string {
  if (f === 'recommended') return 'bg-success'
  if (f === 'possible') return 'bg-info text-dark'
  return 'bg-danger'
}
</script>

<template>
  <div>
    <h4 class="mb-4">What-If Capacity Planning</h4>

    <div class="row g-3">
      <div v-for="s in scenarios" :key="s.name" class="col-12">
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
            <span class="fw-semibold">{{ s.name }}</span>
            <span class="badge" :class="feasBadge(s.feasibility)">{{ s.feasibility }}</span>
          </div>
          <div class="card-body">
            <div class="row g-3 mb-3">
              <div class="col-sm-3">
                <div class="border rounded p-2 text-center">
                  <div class="fw-semibold" :class="s.impact.racks > 0 ? 'text-danger' : s.impact.racks < 0 ? 'text-success' : ''">
                    {{ s.impact.racks > 0 ? '+' : '' }}{{ s.impact.racks }} racks
                  </div>
                  <small class="text-muted">Rack Impact</small>
                </div>
              </div>
              <div class="col-sm-3">
                <div class="border rounded p-2 text-center">
                  <div class="fw-semibold" :class="s.impact.power > 0 ? 'text-danger' : 'text-success'">
                    {{ s.impact.power > 0 ? '+' : '' }}{{ s.impact.power }} kW
                  </div>
                  <small class="text-muted">Power Impact</small>
                </div>
              </div>
              <div class="col-sm-3">
                <div class="border rounded p-2 text-center">
                  <div class="fw-semibold" :class="s.impact.cooling > 0 ? 'text-danger' : 'text-success'">
                    {{ s.impact.cooling > 0 ? '+' : '' }}{{ s.impact.cooling }} kW
                  </div>
                  <small class="text-muted">Cooling Impact</small>
                </div>
              </div>
              <div class="col-sm-3">
                <div class="border rounded p-2 text-center">
                  <div class="fw-semibold">{{ s.impact.space }}</div>
                  <small class="text-muted">Space Impact</small>
                </div>
              </div>
            </div>
            <p class="text-muted mb-0 small">{{ s.notes }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
