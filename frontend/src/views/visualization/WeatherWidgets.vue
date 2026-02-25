<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1 class="h3 mb-0">Weather & Location Data</h1>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Location</button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="row">
      <div v-for="location in locations" :key="location.id" class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header bg-light">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h5 class="mb-0">{{ location.name }}</h5>
                <small class="text-muted">{{ location.region }}</small>
              </div>
              <div class="text-end">
                <div style="font-size: 32px;">{{ getWeatherIcon(location.condition) }}</div>
              </div>
            </div>
          </div>
          <div class="card-body">
            <div class="row mb-4">
              <div class="col-6">
                <div class="mb-3">
                  <div class="text-muted small">Current Temperature</div>
                  <div style="font-size: 28px; font-weight: bold;">{{ location.temp }}C</div>
                </div>
                <div class="mb-3">
                  <div class="text-muted small">Feels Like</div>
                  <div style="font-size: 20px;">{{ location.feelsLike }}C</div>
                </div>
                <div>
                  <div class="text-muted small">Wind Speed</div>
                  <div style="font-size: 20px;">{{ location.windSpeed }} km/h</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mb-3">
                  <div class="text-muted small">Condition</div>
                  <div style="font-size: 18px; font-weight: 500;">{{ location.condition }}</div>
                </div>
                <div class="mb-3">
                  <div class="text-muted small">Humidity</div>
                  <div style="font-size: 20px;">{{ location.humidity }}%</div>
                </div>
                <div>
                  <div class="text-muted small">Pressure</div>
                  <div style="font-size: 20px;">{{ location.pressure }} mb</div>
                </div>
              </div>
            </div>

            <div class="mb-3">
              <div class="text-muted small mb-1">Air Quality Index</div>
              <div class="progress" style="height: 20px;">
                <div :class="getAqiColor(location.aqi)" style="width: 60%;">60 - Moderate</div>
              </div>
            </div>

            <div class="alert" :class="getAlertClass(location.impact)" role="alert" style="margin-bottom: 0;">
              <strong>DC Impact:</strong> {{ location.impact }}
            </div>
          </div>
          <div class="card-footer bg-light d-flex justify-content-between align-items-center">
            <small class="text-muted">Last updated: {{ location.lastUpdated }}</small>
            <div>
              <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(location)">Edit</button>
              <button class="btn btn-sm btn-outline-danger" @click="remove(location.id)">Delete</button>
            </div>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Weather Location</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Region</label><input v-model="form.region" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Temperature</label><input v-model.number="form.temp" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Feels Like</label><input v-model.number="form.feelsLike" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Condition</label><input v-model="form.condition" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Wind Speed</label><input v-model.number="form.windSpeed" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Humidity</label><input v-model.number="form.humidity" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Pressure</label><input v-model.number="form.pressure" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">AQI</label><input v-model.number="form.aqi" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Impact</label><input v-model="form.impact" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Last Updated</label><input v-model="form.lastUpdated" type="text" class="form-control" /></div>
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
import { ref, onMounted } from 'vue'
import { weatherLocationsApi, type WeatherLocation } from '../../services/api'

const locations = ref<WeatherLocation[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<WeatherLocation | null>(null)
const saving = ref(false)

const defaultForm = { name: '', region: '', temp: 0, feelsLike: 0, condition: '', windSpeed: 0, humidity: 0, pressure: 0, aqi: 0, impact: '', lastUpdated: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  locations.value = await weatherLocationsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: WeatherLocation) {
  editing.value = item
  form.value = { name: item.name, region: item.region, temp: item.temp, feelsLike: item.feelsLike, condition: item.condition, windSpeed: item.windSpeed, humidity: item.humidity, pressure: item.pressure, aqi: item.aqi, impact: item.impact, lastUpdated: item.lastUpdated }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await weatherLocationsApi.update(editing.value.id, form.value)
    } else {
      await weatherLocationsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this location?')) return
  await weatherLocationsApi.remove(id)
  await loadData()
}

function getWeatherIcon(condition: string): string {
  const icons: Record<string, string> = {
    'Partly Cloudy': '⛅',
    'Cloudy': '☁️',
    'Thunderstorm': '⛈️',
    'Clear': '☀️',
    'Rainy': '🌧️',
    'Snowy': '🌨️'
  };
  return icons[condition] || '🌤️';
}

function getAqiColor(aqi: number): string {
  if (aqi < 50) return 'bg-success';
  if (aqi < 100) return 'bg-warning';
  return 'bg-danger';
}

function getAlertClass(impact: string): string {
  if (impact.startsWith('Low')) return 'alert-success';
  if (impact.startsWith('Good') || impact.startsWith('Optimal')) return 'alert-info';
  return 'alert-warning';
}
</script>
