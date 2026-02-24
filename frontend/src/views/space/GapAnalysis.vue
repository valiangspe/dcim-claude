<script setup lang="ts">
const racks = [
  { id: 'CAB-A01', totalU: 42, freeU: 4, gaps: [{ start: 15, size: 1 }, { start: 28, size: 2 }, { start: 40, size: 1 }] },
  { id: 'CAB-A03', totalU: 42, freeU: 14, gaps: [{ start: 5, size: 3 }, { start: 18, size: 4 }, { start: 30, size: 2 }, { start: 37, size: 5 }] },
  { id: 'CAB-B03', totalU: 42, freeU: 20, gaps: [{ start: 1, size: 6 }, { start: 15, size: 8 }, { start: 35, size: 6 }] },
  { id: 'CAB-C01', totalU: 42, freeU: 12, gaps: [{ start: 8, size: 4 }, { start: 22, size: 3 }, { start: 33, size: 5 }] },
  { id: 'CAB-C02', totalU: 42, freeU: 24, gaps: [{ start: 1, size: 10 }, { start: 19, size: 8 }, { start: 35, size: 6 }] },
  { id: 'CAB-D01', totalU: 48, freeU: 36, gaps: [{ start: 13, size: 36 }] },
]

const summary = {
  total1U: 8, total2U: 6, total3U: 4, total4UPlus: 9,
}

function sizeColor(size: number): string {
  if (size >= 4) return 'bg-success'
  if (size >= 2) return 'bg-info text-dark'
  return 'bg-secondary'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Gap Analysis</h4>

    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-secondary mb-0">{{ summary.total1U }}</h3><small class="text-muted">1U Gaps</small></div></div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-info mb-0">{{ summary.total2U }}</h3><small class="text-muted">2-3U Gaps</small></div></div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-success mb-0">{{ summary.total4UPlus }}</h3><small class="text-muted">4U+ Gaps</small></div></div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-primary mb-0">{{ racks.reduce((a, r) => a + r.freeU, 0) }}</h3><small class="text-muted">Total Free U</small></div></div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Racks with Gaps</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr><th>Rack</th><th class="text-end">Total U</th><th class="text-end">Free U</th><th>Gaps</th></tr>
          </thead>
          <tbody>
            <tr v-for="r in racks" :key="r.id">
              <td class="fw-semibold">{{ r.id }}</td>
              <td class="text-end">{{ r.totalU }}</td>
              <td class="text-end">{{ r.freeU }}</td>
              <td>
                <div class="d-flex flex-wrap gap-1">
                  <span v-for="(g, i) in r.gaps" :key="i" class="badge" :class="sizeColor(g.size)">
                    U{{ g.start }}–{{ g.start + g.size - 1 }} ({{ g.size }}U)
                  </span>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="d-flex gap-3 small mt-3">
      <span><span class="badge bg-secondary">&nbsp;&nbsp;</span> 1U</span>
      <span><span class="badge bg-info">&nbsp;&nbsp;</span> 2-3U</span>
      <span><span class="badge bg-success">&nbsp;&nbsp;</span> 4U+</span>
    </div>
  </div>
</template>
