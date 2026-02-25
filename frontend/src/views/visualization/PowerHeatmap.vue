<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1 class="h3 mb-0">Power Usage Heatmap</h1>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Data Point</button>
    </div>

    <div class="row mb-3">
      <div class="col-auto">
        <select class="form-select form-select-sm" style="width: auto;" v-model="selectedTimeRange">
          <option value="hour">Last Hour</option>
          <option value="day">Last 24 Hours</option>
          <option value="week">Last 7 Days</option>
          <option value="month">Last 30 Days</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table mb-0" style="font-size: 12px;">
            <thead>
              <tr>
                <th style="width: 100px;">Hall / Time</th>
                <th v-for="time in timeSlots" :key="time" style="width: 40px; text-align: center;">
                  {{ time }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="hall in halls" :key="hall">
                <td class="fw-medium">{{ hall }}</td>
                <td v-for="col in timeSlots" :key="col"
                    :style="{ backgroundColor: getHeatmapColor(getCell(hall, col)), color: getCell(hall, col) > 50 ? 'white' : 'black', textAlign: 'center', padding: '8px 0' }">
                  {{ getCell(hall, col) }} kW
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer">
        <div class="d-flex align-items-center gap-3">
          <span class="text-muted small">Scale:</span>
          <div style="width: 20px; height: 20px; background-color: rgb(0, 128, 0); border: 1px solid #ccc;"></div>
          <span class="small">Low (&lt;1000 kW)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 200, 0); border: 1px solid #ccc;"></div>
          <span class="small">Medium (1000-2000 kW)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 100, 0); border: 1px solid #ccc;"></div>
          <span class="small">High (2000-3000 kW)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(200, 0, 0); border: 1px solid #ccc;"></div>
          <span class="small">Critical (&gt;3000 kW)</span>
        </div>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header fw-semibold">Heatmap Data</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Category</th><th>Row Label</th><th>Col Label</th><th class="text-end">Value</th><th>Display Value</th><th>Actions</th></tr>
          </thead>
          <tbody>
            <tr v-for="item in powerData" :key="item.id">
              <td>{{ item.category }}</td>
              <td class="fw-medium">{{ item.rowLabel }}</td>
              <td>{{ item.colLabel }}</td>
              <td class="text-end">{{ item.value }}</td>
              <td>{{ item.displayValue }}</td>
              <td>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(item)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(item.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
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
            <div class="mb-3"><label class="form-label">Category</label><input v-model="form.category" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Row Label</label><input v-model="form.rowLabel" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Col Label</label><input v-model="form.colLabel" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Value</label><input v-model.number="form.value" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Display Value</label><input v-model="form.displayValue" type="text" class="form-control" /></div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save
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

const selectedTimeRange = ref('day')
const allData = ref<HeatmapData[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<HeatmapData | null>(null)
const saving = ref(false)

const defaultForm = { category: 'power', rowLabel: '', colLabel: '', value: 0, displayValue: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  allData.value = await heatmapDataApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const powerData = computed(() => allData.value.filter(d => d.category === 'power'))

const halls = computed(() => {
  const set = new Set<string>()
  powerData.value.forEach(d => set.add(d.rowLabel))
  return Array.from(set).sort()
})

const timeSlots = computed(() => {
  const set = new Set<string>()
  powerData.value.forEach(d => set.add(d.colLabel))
  return Array.from(set).sort()
})

const dataMap = computed(() => {
  const map: Record<string, Record<string, number>> = {}
  powerData.value.forEach(d => {
    if (!map[d.rowLabel]) map[d.rowLabel] = {}
    map[d.rowLabel][d.colLabel] = d.value
  })
  return map
})

function getCell(hall: string, col: string): number {
  return dataMap.value[hall]?.[col] || 0
}

function getHeatmapColor(value: number): string {
  if (value < 1000) return 'rgb(0, 128, 0)';
  if (value < 2000) return 'rgb(255, 200, 0)';
  if (value < 3000) return 'rgb(255, 100, 0)';
  return 'rgb(200, 0, 0)';
}

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
    if (editing.value) {
      await heatmapDataApi.update(editing.value.id, form.value)
    } else {
      await heatmapDataApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this data point?')) return
  await heatmapDataApi.remove(id)
  await loadData()
}
</script>
