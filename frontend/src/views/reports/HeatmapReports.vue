<template>
  <div class="heatmap-reports">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-secondary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-header bg-secondary text-white">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Heatmap Reports</h5>
          <div class="d-flex gap-2 align-items-center">
          <button class="btn btn-sm btn-light" @click="openCreate">+ Add</button>
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
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-bordered text-center">
            <thead>
              <tr class="table-light">
                <th>Rack</th>
                <th v-for="col in colLabels" :key="col">{{ col }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(row, rowLabel) in heatmapGrid" :key="rowLabel">
                <td class="fw-bold">{{ rowLabel }}</td>
                <td v-for="col in colLabels" :key="col">
                  <div
                    class="heatmap-cell"
                    :style="{ backgroundColor: getCellColor(row[col]) }"
                    :title="`${getMetricLabel()}: ${row[col]?.displayValue || ''}`"
                  >
                    <small class="text-white fw-bold">{{ row[col]?.displayValue || '' }}</small>
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
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Heatmap Data</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Category</label>
              <select v-model="form.category" class="form-select">
                <option value="power">Power</option>
                <option value="temp">Temperature</option>
                <option value="util">Utilization</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Row Label</label>
              <input v-model="form.rowLabel" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Column Label</label>
              <input v-model="form.colLabel" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Value</label>
              <input v-model.number="form.value" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Display Value</label>
              <input v-model="form.displayValue" type="text" class="form-control" />
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { heatmapDataApi, type HeatmapData } from '../../services/api'

const heatmapType = ref<string>('power')
const allHeatmapData = ref<HeatmapData[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<HeatmapData | null>(null)
const defaultForm = { category: 'power', rowLabel: '', colLabel: '', value: 0, displayValue: '' }
const form = ref({ ...defaultForm })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: HeatmapData) {
  editing.value = item
  form.value = { category: item.category, rowLabel: item.rowLabel, colLabel: item.colLabel, value: item.value, displayValue: item.displayValue }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await heatmapDataApi.update(editing.value.id, form.value)
    else await heatmapDataApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await heatmapDataApi.remove(id)
  await loadData()
}

async function loadData() {
  allHeatmapData.value = await heatmapDataApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const filteredData = computed(() => {
  return allHeatmapData.value.filter(d => d.category === heatmapType.value)
})

const colLabels = computed(() => {
  const cols = new Set<string>()
  filteredData.value.forEach(d => cols.add(d.colLabel))
  return Array.from(cols).sort()
})

const heatmapGrid = computed(() => {
  const grid: Record<string, Record<string, HeatmapData>> = {}
  filteredData.value.forEach(d => {
    if (!grid[d.rowLabel]) grid[d.rowLabel] = {}
    grid[d.rowLabel][d.colLabel] = d
  })
  return grid
})

const getMetricLabel = (): string => {
  switch (heatmapType.value) {
    case 'power':
      return 'Power (kW)'
    case 'temp':
      return 'Temperature (C)'
    case 'util':
      return 'Utilization (%)'
    default:
      return 'Value'
  }
}

const getCellColor = (cell: HeatmapData | undefined): string => {
  if (!cell) return '#6c757d'
  const value = cell.value

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
      { id: '1', color: '#0dcaf0', label: '< 21C (Cold)' },
      { id: '2', color: '#198754', label: '21-25C (Optimal)' },
      { id: '3', color: '#ffc107', label: '25-27C (Warm)' },
      { id: '4', color: '#dc3545', label: '> 27C (Hot)' }
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
