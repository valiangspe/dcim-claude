<script setup lang="ts">
import { onMounted, onBeforeUnmount, ref } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

const mapContainer = ref<HTMLDivElement>()
let map: L.Map | null = null

const sites = [
  { name: 'DC-East-01', location: 'Virginia, US', lat: 38.95, lng: -77.45, status: 'operational', power: 1240, pue: 1.32, racks: 86, alarms: 2 },
  { name: 'DC-West-01', location: 'Oregon, US', lat: 45.59, lng: -121.18, status: 'operational', power: 980, pue: 1.28, racks: 62, alarms: 0 },
  { name: 'DC-EU-01', location: 'Frankfurt, DE', lat: 50.11, lng: 8.68, status: 'operational', power: 627, pue: 1.45, racks: 38, alarms: 5 },
  { name: 'DC-APAC-01', location: 'Singapore, SG', lat: 1.35, lng: 103.82, status: 'maintenance', power: 0, pue: 0, racks: 24, alarms: 1 },
  { name: 'DC-EU-02', location: 'Amsterdam, NL', lat: 52.37, lng: 4.90, status: 'operational', power: 412, pue: 1.35, racks: 28, alarms: 3 },
  { name: 'DC-South-01', location: 'Sao Paulo, BR', lat: -23.55, lng: -46.63, status: 'degraded', power: 215, pue: 1.52, racks: 16, alarms: 8 },
]

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

onMounted(() => {
  if (!mapContainer.value) return

  map = L.map(mapContainer.value).setView([20, 0], 2)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    maxZoom: 18,
  }).addTo(map)

  for (const site of sites) {
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
})

onBeforeUnmount(() => {
  map?.remove()
  map = null
})
</script>

<template>
  <div>
    <h4 class="mb-4">Global Map</h4>

    <!-- Map -->
    <div class="card border-0 shadow-sm mb-4">
      <div ref="mapContainer" style="height: 420px; z-index: 0;"></div>
    </div>

    <!-- Sites Table -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold d-flex justify-content-between">
        <span>All Sites</span>
        <span class="badge bg-dark">{{ sites.length }} sites</span>
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
              </tr>
            </thead>
            <tbody>
              <tr v-for="site in sites" :key="site.name">
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
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
