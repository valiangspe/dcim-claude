<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js'
import { timeseriesApi, type TemperatureReading } from '../../services/api'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend)

const readings = ref<TemperatureReading[]>([])
const loading = ref(true)
const seeding = ref(false)
const writing = ref(false)
const selectedRange = ref('24h')
const showWriteModal = ref(false)

const ranges = ['1h', '6h', '24h', '7d']
const writeForm = ref({ sensorId: '', cabinet: '', inlet: 0, outlet: 0 })

const dtFmt = new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' })
function formatTime(iso: string) {
  return dtFmt.format(new Date(iso))
}

const bySensor = computed(() => {
  const map = new Map<string, TemperatureReading[]>()
  for (const r of readings.value) {
    if (!map.has(r.sensorId)) map.set(r.sensorId, [])
    map.get(r.sensorId)!.push(r)
  }
  return map
})

const sensorIds = computed(() => Array.from(bySensor.value.keys()))

function latestReading(id: string) {
  const rows = bySensor.value.get(id) ?? []
  return rows[rows.length - 1]
}

function inletColor(v: number) {
  if (v >= 25) return 'text-danger fw-semibold'
  if (v >= 23) return 'text-warning'
  return 'text-success'
}

function barColor(v: number) {
  if (v >= 25) return 'bg-danger'
  if (v >= 23) return 'bg-warning'
  return 'bg-success'
}

function barWidth(v: number) {
  return Math.min(100, Math.max(0, ((v - 18) / 12) * 100))
}

function buildChartData(rows: TemperatureReading[]) {
  const labels = rows.map(r => formatTime(r.time))
  return {
    labels,
    datasets: [
      {
        label: 'Inlet (°C)',
        data: rows.map(r => r.inlet),
        borderColor: '#0d6efd',
        backgroundColor: 'transparent',
        borderWidth: 1.5,
        pointRadius: 0,
        tension: 0.3,
      },
      {
        label: 'Outlet (°C)',
        data: rows.map(r => r.outlet),
        borderColor: '#dc3545',
        backgroundColor: 'transparent',
        borderWidth: 1.5,
        pointRadius: 0,
        tension: 0.3,
      },
    ],
  }
}

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  animation: false as const,
  plugins: {
    legend: { position: 'top' as const, labels: { boxWidth: 12, font: { size: 11 } } },
    tooltip: {
      mode: 'index' as const,
      intersect: false,
      callbacks: {
        title: (items: { label: string }[]) => items[0]?.label ?? '',
      },
    },
  },
  scales: {
    x: {
      ticks: {
        maxTicksLimit: 6,
        maxRotation: 0,
        font: { size: 10 },
      },
      grid: { display: false },
    },
    y: {
      ticks: { font: { size: 10 } },
      grid: { color: '#f0f0f0' },
    },
  },
}

async function load() {
  loading.value = true
  try {
    readings.value = await timeseriesApi.getTemperature(selectedRange.value)
  } finally {
    loading.value = false
  }
}

async function seed() {
  seeding.value = true
  try {
    await timeseriesApi.seedTemperature()
    await load()
  } finally {
    seeding.value = false
  }
}

async function writeReading() {
  writing.value = true
  try {
    await timeseriesApi.writeTemperature(writeForm.value)
    showWriteModal.value = false
    await load()
  } finally {
    writing.value = false
  }
}

onMounted(load)
</script>

<template>
  <div>
    <h4 class="mb-1">Sensors Trend</h4>
    <p class="text-muted mb-3">Live temperature history from InfluxDB — 5-minute bucket averages per cabinet sensor.</p>

    <div class="d-flex gap-2 mb-4 align-items-center flex-wrap">
      <div class="btn-group btn-group-sm">
        <button
          v-for="r in ranges" :key="r"
          class="btn"
          :class="selectedRange === r ? 'btn-primary' : 'btn-outline-secondary'"
          @click="selectedRange = r; load()"
        >{{ r }}</button>
      </div>
      <button class="btn btn-sm btn-outline-secondary" @click="load" :disabled="loading">
        <span v-if="loading" class="spinner-border spinner-border-sm"></span>
        <span v-else>Refresh</span>
      </button>
      <button class="btn btn-sm btn-outline-primary" @click="showWriteModal = true">+ Write Reading</button>
      <button class="btn btn-sm btn-outline-warning" @click="seed" :disabled="seeding">
        <span v-if="seeding" class="spinner-border spinner-border-sm me-1"></span>
        Seed Demo Data
      </button>
    </div>

    <div v-if="loading && readings.length === 0" class="text-center py-5">
      <div class="spinner-border"></div>
    </div>

    <div v-else-if="readings.length === 0" class="alert alert-info">
      No data in InfluxDB for the selected range.
      <button class="btn btn-sm btn-primary ms-2" @click="seed" :disabled="seeding">Seed 24h of demo data</button>
    </div>

    <template v-else>
      <!-- Summary cards -->
      <div class="row g-3 mb-4">
        <div v-for="id in sensorIds" :key="id" class="col-md-4">
          <div class="card border-0 shadow-sm">
            <div class="card-body">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                  <div class="fw-semibold">{{ id }}</div>
                  <small class="text-muted">{{ latestReading(id)?.cabinet }}</small>
                </div>
                <span class="badge bg-primary">Live</span>
              </div>
              <div class="mt-2 d-flex gap-3" v-if="latestReading(id)">
                <div>
                  <div class="small text-muted">Inlet</div>
                  <div :class="inletColor(latestReading(id)!.inlet)">{{ latestReading(id)!.inlet.toFixed(1) }}°C</div>
                </div>
                <div>
                  <div class="small text-muted">Outlet</div>
                  <div>{{ latestReading(id)!.outlet.toFixed(1) }}°C</div>
                </div>
                <div>
                  <div class="small text-muted">Delta-T</div>
                  <div>{{ latestReading(id)!.delta.toFixed(1) }}°C</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Per-sensor: table + line chart side by side -->
      <div v-for="id in sensorIds" :key="id" class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold">
          {{ id }}
          <span class="text-muted fw-normal">— {{ latestReading(id)?.cabinet }}</span>
          <span class="badge bg-secondary ms-2">{{ bySensor.get(id)?.length }} points</span>
        </div>
        <div class="card-body p-0 d-flex" style="min-height: 240px;">

          <!-- Table -->
          <div style="flex: 0 0 400px; max-height: 280px; overflow-y: auto; border-right: 1px solid #dee2e6;">
            <table class="table table-sm table-hover mb-0">
              <thead class="table-light sticky-top">
                <tr>
                  <th>Time</th>
                  <th class="text-end">Inlet</th>
                  <th class="text-end">Outlet</th>
                  <th class="text-end">ΔT</th>
                  <th style="width: 70px;">Bar</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in bySensor.get(id)" :key="r.time">
                  <td class="text-muted small" style="white-space: nowrap;">{{ formatTime(r.time) }}</td>
                  <td class="text-end fw-semibold" :class="inletColor(r.inlet)">{{ r.inlet.toFixed(1) }}</td>
                  <td class="text-end">{{ r.outlet.toFixed(1) }}</td>
                  <td class="text-end text-muted">{{ r.delta.toFixed(1) }}</td>
                  <td>
                    <div class="progress" style="height: 7px;">
                      <div class="progress-bar" :class="barColor(r.inlet)" :style="{ width: barWidth(r.inlet) + '%' }"></div>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Chart.js line chart -->
          <div class="flex-grow-1 p-3" style="min-width: 0; position: relative;">
            <Line
              :data="buildChartData(bySensor.get(id) ?? [])"
              :options="chartOptions"
              style="width: 100%; height: 220px;"
            />
          </div>

        </div>
      </div>
    </template>

    <!-- Write modal -->
    <div v-if="showWriteModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Write Temperature Reading</h5>
            <button type="button" class="btn-close" @click="showWriteModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Sensor ID</label>
              <input v-model="writeForm.sensorId" type="text" class="form-control" placeholder="e.g. S-01" />
            </div>
            <div class="mb-3">
              <label class="form-label">Cabinet</label>
              <input v-model="writeForm.cabinet" type="text" class="form-control" placeholder="e.g. Row-A / Cabinet 01" />
            </div>
            <div class="mb-3">
              <label class="form-label">Inlet (°C)</label>
              <input v-model.number="writeForm.inlet" type="number" step="0.1" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Outlet (°C)</label>
              <input v-model.number="writeForm.outlet" type="number" step="0.1" class="form-control" />
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showWriteModal = false">Cancel</button>
            <button class="btn btn-primary" @click="writeReading" :disabled="writing">
              <span v-if="writing" class="spinner-border spinner-border-sm me-1"></span>
              Write to InfluxDB
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
