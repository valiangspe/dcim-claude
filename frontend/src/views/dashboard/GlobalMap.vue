<script setup lang="ts">
import { onMounted, onBeforeUnmount, ref } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { sitesApi, type Site } from '../../services/api'

const mapContainer = ref<HTMLDivElement>()
let map: L.Map | null = null

const sites = ref<Site[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Site | null>(null)

const defaultForm = { name: '', location: '', lat: 0, lng: 0, status: 'operational', power: 0, pue: 0, racks: 0, alarms: 0 }
const form = ref({ ...defaultForm })

function statusBadge(status: string): string {
  switch (status) {
    case 'operational': return 'bg-success'
    case 'degraded': return 'bg-warning text-dark'
    case 'maintenance': return 'bg-secondary'
    default: return 'bg-danger'
  }
}

function markerColor(status: string): string {
  switch (status) {
    case 'operational': return '#198754'
    case 'degraded': return '#ffc107'
    case 'maintenance': return '#6c757d'
    default: return '#dc3545'
  }
}

function createIcon(status: string) {
  const color = markerColor(status)
  const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="28" height="40" viewBox="0 0 28 40">
    <path d="M14 0C6.27 0 0 6.27 0 14c0 10.5 14 26 14 26s14-15.5 14-26C28 6.27 21.73 0 14 0z" fill="${color}" stroke="#fff" stroke-width="2"/>
    <circle cx="14" cy="14" r="6" fill="#fff"/>
  </svg>`
  return L.divIcon({
    html: svg,
    className: '',
    iconSize: [28, 40],
    iconAnchor: [14, 40],
    popupAnchor: [0, -36],
  })
}

function initMap() {
  if (!mapContainer.value) return

  map = L.map(mapContainer.value).setView([20, 0], 2)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    maxZoom: 18,
  }).addTo(map)

  for (const site of sites.value) {
    const popup = `
      <strong>${site.name}</strong><br>
      ${site.location}<br>
      <span style="color:${markerColor(site.status)}; font-weight:600;">${site.status.toUpperCase()}</span><br>
      Power: ${site.power > 0 ? site.power.toLocaleString() + ' kW' : 'N/A'}<br>
      PUE: ${site.pue > 0 ? site.pue.toFixed(2) : 'N/A'}<br>
      Racks: ${site.racks} &middot; Alarms: ${site.alarms}
    `
    L.marker([site.lat, site.lng], { icon: createIcon(site.status) })
      .addTo(map!)
      .bindPopup(popup)
  }
}

async function loadData() {
  sites.value = await sitesApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
  initMap()
})

onBeforeUnmount(() => {
  map?.remove()
  map = null
})

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: Site) {
  editing.value = item
  form.value = { name: item.name, location: item.location, lat: item.lat, lng: item.lng, status: item.status, power: item.power, pue: item.pue, racks: item.racks, alarms: item.alarms }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await sitesApi.update(editing.value.id, form.value)
    } else {
      await sitesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await sitesApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Global Map</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <!-- Map -->
      <div class="card border-0 shadow-sm mb-4">
        <div ref="mapContainer" style="height: 420px; z-index: 0;"></div>
      </div>

      <!-- Sites Table -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>All Sites ({{ sites.length }})</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr>
                  <th>Site</th>
                  <th>Location</th>
                  <th>Status</th>
                  <th class="text-end">Power (kW)</th>
                  <th class="text-end">PUE</th>
                  <th class="text-end">Racks</th>
                  <th class="text-end">Alarms</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="site in sites" :key="site.id">
                  <td class="fw-semibold">{{ site.name }}</td>
                  <td class="text-muted">{{ site.location }}</td>
                  <td><span class="badge" :class="statusBadge(site.status)">{{ site.status }}</span></td>
                  <td class="text-end">{{ site.power > 0 ? site.power.toLocaleString() : '---' }}</td>
                  <td class="text-end">{{ site.pue > 0 ? site.pue.toFixed(2) : '---' }}</td>
                  <td class="text-end">{{ site.racks }}</td>
                  <td class="text-end">
                    <span v-if="site.alarms > 0" class="badge bg-danger">{{ site.alarms }}</span>
                    <span v-else class="text-success">0</span>
                  </td>
                  <td>
                    <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(site)">Edit</button>
                    <button class="btn btn-sm btn-outline-danger" @click="remove(site.id)">Delete</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Site</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Location</label>
              <input v-model="form.location" type="text" class="form-control" />
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Latitude</label>
                <input v-model.number="form.lat" type="number" step="0.0001" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">Longitude</label>
                <input v-model.number="form.lng" type="number" step="0.0001" class="form-control" />
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>operational</option>
                <option>degraded</option>
                <option>maintenance</option>
                <option>offline</option>
              </select>
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Power (kW)</label>
                <input v-model.number="form.power" type="number" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">PUE</label>
                <input v-model.number="form.pue" type="number" step="0.01" class="form-control" />
              </div>
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Racks</label>
                <input v-model.number="form.racks" type="number" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">Alarms</label>
                <input v-model.number="form.alarms" type="number" class="form-control" />
              </div>
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
