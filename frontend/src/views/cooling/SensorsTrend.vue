<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { sensorTrendsApi, type SensorTrend } from '../../services/api'

const allTrends = ref<SensorTrend[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<SensorTrend | null>(null)

const defaultForm = { sensorId: '', cabinet: '', type: '', time: '', value: 0 }
const form = ref({ ...defaultForm })

const sensors = computed(() => {
  const seen = new Map<string, { id: string; cabinet: string; type: string }>()
  for (const t of allTrends.value) {
    if (!seen.has(t.sensorId)) {
      seen.set(t.sensorId, { id: t.sensorId, cabinet: t.cabinet, type: t.type })
    }
  }
  return Array.from(seen.values())
})

const trend = computed(() => {
  const grouped: Record<string, SensorTrend[]> = {}
  for (const t of allTrends.value) {
    if (!grouped[t.sensorId]) grouped[t.sensorId] = []
    grouped[t.sensorId].push(t)
  }
  return grouped
})

function tempColor(val: number): string {
  if (val >= 25) return 'bg-danger'
  if (val >= 23) return 'bg-warning'
  return 'bg-success'
}

async function loadData() {
  allTrends.value = await sensorTrendsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: SensorTrend) {
  editing.value = item
  form.value = { sensorId: item.sensorId, cabinet: item.cabinet, type: item.type, time: item.time, value: item.value }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await sensorTrendsApi.update(editing.value.id, form.value)
    } else {
      await sensorTrendsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await sensorTrendsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Sensors Trend</h4>
    <div class="d-flex justify-content-between align-items-center mb-3">
      <p class="text-muted mb-0">Historical temperature sensor readings (last 24h).</p>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Reading</button>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div v-for="sensor in sensors" :key="sensor.id" class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent d-flex justify-content-between">
          <span class="fw-semibold">{{ sensor.id }} <span class="text-muted fw-normal">— {{ sensor.cabinet }} ({{ sensor.type }})</span></span>
        </div>
        <div class="card-body p-0">
          <table class="table table-sm table-hover mb-0">
            <thead class="table-light">
              <tr><th>Time</th><th class="text-end">Temp (°C)</th><th style="width:45%">Bar</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="t in trend[sensor.id]" :key="t.id">
                <td>{{ t.time }}</td>
                <td class="text-end fw-semibold">{{ t.value.toFixed(1) }}</td>
                <td>
                  <div class="progress" style="height: 10px;">
                    <div :class="`progress-bar ${tempColor(t.value)}`" :style="{ width: ((t.value - 18) / 12 * 100) + '%' }"></div>
                  </div>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(t)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(t.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Sensor Reading</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Sensor ID</label>
              <input v-model="form.sensorId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Cabinet</label>
              <input v-model="form.cabinet" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Time</label>
              <input v-model="form.time" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Value</label>
              <input v-model.number="form.value" type="number" class="form-control" />
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
