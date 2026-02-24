<script setup lang="ts">
interface RackCell {
  id: string
  util: number
}

const rows: { label: string; racks: (RackCell | null)[] }[] = [
  { label: 'Row 1', racks: [
    { id: 'A01', util: 90 }, { id: 'A02', util: 100 }, { id: 'A03', util: 67 }, { id: 'A04', util: 83 },
    null,
    { id: 'B01', util: 95 }, { id: 'B02', util: 98 }, { id: 'B03', util: 52 },
  ]},
  { label: 'Row 2', racks: [
    { id: 'A05', util: 78 }, { id: 'A06', util: 85 }, { id: 'A07', util: 72 }, { id: 'A08', util: 60 },
    null,
    { id: 'B04', util: 88 }, { id: 'B05', util: 45 }, { id: 'B06', util: 30 },
  ]},
  { label: 'Row 3', racks: [
    { id: 'C01', util: 71 }, { id: 'C02', util: 43 }, null, null,
    null,
    { id: 'D01', util: 25 }, { id: 'D02', util: 18 }, { id: 'D03', util: 10 },
  ]},
]

function cellBg(util: number): string {
  if (util >= 90) return 'bg-danger text-white'
  if (util >= 70) return 'bg-warning text-dark'
  if (util >= 40) return 'bg-info text-dark'
  return 'bg-success text-white'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Floor Plan</h4>

    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">Hall A & B — Floor Layout</div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-bordered text-center mb-0" style="table-layout: fixed;">
            <thead>
              <tr class="table-light">
                <th style="width:70px"></th>
                <th v-for="i in 8" :key="i">Pos {{ i }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in rows" :key="row.label">
                <td class="fw-semibold table-light align-middle">{{ row.label }}</td>
                <td v-for="(rack, i) in row.racks" :key="i" :class="rack ? cellBg(rack.util) : 'bg-light'" style="height: 60px; vertical-align: middle;">
                  <template v-if="rack">
                    <div class="fw-semibold small">{{ rack.id }}</div>
                    <div class="small">{{ rack.util }}%</div>
                  </template>
                  <template v-else>
                    <span class="text-muted small">—</span>
                  </template>
                </td>
              </tr>
              <tr>
                <td class="table-secondary fw-semibold" colspan="5">Cold Aisle</td>
                <td class="table-secondary fw-semibold" colspan="4">Cold Aisle</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div class="d-flex gap-3 small">
      <span><span class="badge bg-success">&nbsp;&nbsp;</span> &lt;40%</span>
      <span><span class="badge bg-info">&nbsp;&nbsp;</span> 40-70%</span>
      <span><span class="badge bg-warning">&nbsp;&nbsp;</span> 70-90%</span>
      <span><span class="badge bg-danger">&nbsp;&nbsp;</span> &ge;90%</span>
      <span><span class="badge bg-light text-muted border">&nbsp;&nbsp;</span> Empty</span>
    </div>
  </div>
</template>
