<template>
  <div class="heatmap-reports">
    <div class="card">
      <div class="card-header bg-secondary text-white">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Heatmap Reports</h5>
          <div class="btn-group btn-group-sm" role="group">
            <input v-model="heatmapType" type="radio" class="btn-check" id="power" value="power" />
            <label class="btn btn-light" for="power">Power</label>

            <input v-model="heatmapType" type="radio" class="btn-check" id="temp" value="temp" />
            <label class="btn btn-light" for="temp">Temperature</label>

            <input v-model="heatmapType" type="radio" class="btn-check" id="util" value="util" />
            <label class="btn btn-light" for="util">Utilization</label>
          </div>
        </div>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-bordered text-center">
            <thead>
              <tr class="table-light">
                <th>Rack</th>
                <th v-for="col in 12" :key="col">U{{ col }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(row, rowIdx) in heatmapData" :key="rowIdx">
                <td class="fw-bold">Row {{ rowIdx + 1 }}</td>
                <td v-for="(cell, cellIdx) in row" :key="cellIdx">
                  <div
                    class="heatmap-cell"
                    :style="{ backgroundColor: getCellColor(cell) }"
                    :title="`${getMetricLabel()}: ${cell.value}`"
                  >
                    <small class="text-white fw-bold">{{ cell.value }}</small>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <hr class="my-4" />

        <h6 class="fw-bold mb-3">Color Scale Legend</h6>
        <div class="d-flex gap-2 flex-wrap">
          <div v-for="legend in getColorLegend()" :key="legend.id" class="d-flex align-items-center">
            <div
              class="me-2"
              :style="{ width: '20px', height: '20px', backgroundColor: legend.color, borderRadius: '3px' }"
            ></div>
            <small>{{ legend.label }}</small>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface HeatmapCell {
  value: string
  numericValue: number
}

interface HeatmapRow {
  [key: number]: HeatmapCell
}

const heatmapType = ref<string>('power')

const powerHeatmapData: HeatmapRow[] = [
  {
    0: { value: '8.2', numericValue: 82 },
    1: { value: '7.9', numericValue: 79 },
    2: { value: '9.1', numericValue: 91 },
    3: { value: '6.5', numericValue: 65 },
    4: { value: '8.4', numericValue: 84 },
    5: { value: '7.8', numericValue: 78 },
    6: { value: '9.3', numericValue: 93 },
    7: { value: '6.2', numericValue: 62 },
    8: { value: '8.7', numericValue: 87 },
    9: { value: '7.5', numericValue: 75 },
    10: { value: '8.9', numericValue: 89 },
    11: { value: '6.8', numericValue: 68 }
  },
  {
    0: { value: '7.5', numericValue: 75 },
    1: { value: '8.1', numericValue: 81 },
    2: { value: '6.9', numericValue: 69 },
    3: { value: '9.0', numericValue: 90 },
    4: { value: '7.3', numericValue: 73 },
    5: { value: '8.8', numericValue: 88 },
    6: { value: '6.4', numericValue: 64 },
    7: { value: '9.2', numericValue: 92 },
    8: { value: '7.7', numericValue: 77 },
    9: { value: '8.3', numericValue: 83 },
    10: { value: '6.6', numericValue: 66 },
    11: { value: '9.5', numericValue: 95 }
  },
  {
    0: { value: '8.6', numericValue: 86 },
    1: { value: '6.7', numericValue: 67 },
    2: { value: '8.4', numericValue: 84 },
    3: { value: '7.2', numericValue: 72 },
    4: { value: '9.1', numericValue: 91 },
    5: { value: '6.9', numericValue: 69 },
    6: { value: '8.5', numericValue: 85 },
    7: { value: '7.4', numericValue: 74 },
    8: { value: '8.2', numericValue: 82 },
    9: { value: '9.0', numericValue: 90 },
    10: { value: '7.0', numericValue: 70 },
    11: { value: '8.3', numericValue: 83 }
  }
]

const tempHeatmapData: HeatmapRow[] = [
  {
    0: { value: '24', numericValue: 24 },
    1: { value: '23', numericValue: 23 },
    2: { value: '26', numericValue: 26 },
    3: { value: '22', numericValue: 22 },
    4: { value: '25', numericValue: 25 },
    5: { value: '23', numericValue: 23 },
    6: { value: '27', numericValue: 27 },
    7: { value: '21', numericValue: 21 },
    8: { value: '25', numericValue: 25 },
    9: { value: '24', numericValue: 24 },
    10: { value: '26', numericValue: 26 },
    11: { value: '23', numericValue: 23 }
  },
  {
    0: { value: '23', numericValue: 23 },
    1: { value: '24', numericValue: 24 },
    2: { value: '22', numericValue: 22 },
    3: { value: '25', numericValue: 25 },
    4: { value: '23', numericValue: 23 },
    5: { value: '26', numericValue: 26 },
    6: { value: '21', numericValue: 21 },
    7: { value: '27', numericValue: 27 },
    8: { value: '24', numericValue: 24 },
    9: { value: '25', numericValue: 25 },
    10: { value: '22', numericValue: 22 },
    11: { value: '28', numericValue: 28 }
  },
  {
    0: { value: '25', numericValue: 25 },
    1: { value: '22', numericValue: 22 },
    2: { value: '24', numericValue: 24 },
    3: { value: '23', numericValue: 23 },
    4: { value: '26', numericValue: 26 },
    5: { value: '22', numericValue: 22 },
    6: { value: '25', numericValue: 25 },
    7: { value: '23', numericValue: 23 },
    8: { value: '24', numericValue: 24 },
    9: { value: '27', numericValue: 27 },
    10: { value: '21', numericValue: 21 },
    11: { value: '25', numericValue: 25 }
  }
]

const utilHeatmapData: HeatmapRow[] = [
  {
    0: { value: '92%', numericValue: 92 },
    1: { value: '78%', numericValue: 78 },
    2: { value: '85%', numericValue: 85 },
    3: { value: '65%', numericValue: 65 },
    4: { value: '88%', numericValue: 88 },
    5: { value: '72%', numericValue: 72 },
    6: { value: '95%', numericValue: 95 },
    7: { value: '60%', numericValue: 60 },
    8: { value: '89%', numericValue: 89 },
    9: { value: '76%', numericValue: 76 },
    10: { value: '91%', numericValue: 91 },
    11: { value: '68%', numericValue: 68 }
  },
  {
    0: { value: '74%', numericValue: 74 },
    1: { value: '86%', numericValue: 86 },
    2: { value: '69%', numericValue: 69 },
    3: { value: '93%', numericValue: 93 },
    4: { value: '71%', numericValue: 71 },
    5: { value: '87%', numericValue: 87 },
    6: { value: '62%', numericValue: 62 },
    7: { value: '94%', numericValue: 94 },
    8: { value: '75%', numericValue: 75 },
    9: { value: '84%', numericValue: 84 },
    10: { value: '64%', numericValue: 64 },
    11: { value: '96%', numericValue: 96 }
  },
  {
    0: { value: '88%', numericValue: 88 },
    1: { value: '65%', numericValue: 65 },
    2: { value: '82%', numericValue: 82 },
    3: { value: '70%', numericValue: 70 },
    4: { value: '91%', numericValue: 91 },
    5: { value: '68%', numericValue: 68 },
    6: { value: '85%', numericValue: 85 },
    7: { value: '72%', numericValue: 72 },
    8: { value: '80%', numericValue: 80 },
    9: { value: '93%', numericValue: 93 },
    10: { value: '66%', numericValue: 66 },
    11: { value: '87%', numericValue: 87 }
  }
]

const heatmapData = ref<HeatmapRow[]>(powerHeatmapData)

const getMetricLabel = (): string => {
  switch (heatmapType.value) {
    case 'power':
      return 'Power (kW)'
    case 'temp':
      return 'Temperature (°C)'
    case 'util':
      return 'Utilization (%)'
    default:
      return 'Value'
  }
}

const getCellColor = (cell: HeatmapCell): string => {
  const value = cell.numericValue

  if (heatmapType.value === 'power') {
    if (value >= 90) return '#dc3545'
    if (value >= 80) return '#ffc107'
    if (value >= 65) return '#198754'
    return '#0dcaf0'
  }

  if (heatmapType.value === 'temp') {
    if (value >= 27) return '#dc3545'
    if (value >= 25) return '#ffc107'
    if (value >= 21) return '#198754'
    return '#0dcaf0'
  }

  if (heatmapType.value === 'util') {
    if (value >= 90) return '#dc3545'
    if (value >= 80) return '#ffc107'
    if (value >= 65) return '#198754'
    return '#0dcaf0'
  }

  return '#6c757d'
}

const getColorLegend = () => {
  if (heatmapType.value === 'temp') {
    return [
      { id: '1', color: '#0dcaf0', label: '< 21°C (Cold)' },
      { id: '2', color: '#198754', label: '21-25°C (Optimal)' },
      { id: '3', color: '#ffc107', label: '25-27°C (Warm)' },
      { id: '4', color: '#dc3545', label: '> 27°C (Hot)' }
    ]
  }

  return [
    { id: '1', color: '#0dcaf0', label: 'Low' },
    { id: '2', color: '#198754', label: 'Medium' },
    { id: '3', color: '#ffc107', label: 'High' },
    { id: '4', color: '#dc3545', label: 'Critical' }
  ]
}
</script>

<style scoped>
.heatmap-reports {
  padding: 1rem;
}

.heatmap-cell {
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 60px;
  min-height: 40px;
  border-radius: 4px;
  cursor: pointer;
  transition: transform 0.2s;
}

.heatmap-cell:hover {
  transform: scale(1.05);
}

.table-responsive {
  max-height: 500px;
  overflow-y: auto;
}
</style>
