<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h1 class="h3 mb-0">
          <span v-if="selectedContainer" role="button" @click="selectedContainer = null" class="text-primary me-2">&larr;</span>
          {{ selectedContainer ? selectedContainer.name : 'Container Map' }}
        </h1>
        <small class="text-muted" v-if="!selectedContainer">Room Layout — Click a container to inspect</small>
        <small class="text-muted" v-else>Click on any component for details</small>
      </div>
      <div>
        <select v-if="!selectedContainer" class="form-select form-select-sm" style="width:auto" v-model="selectedRoom">
          <option value="all">All Rooms</option>
          <option v-for="r in rooms" :key="r" :value="r">{{ r }}</option>
        </select>
      </div>
    </div>

    <!-- Room Overview -->
    <div v-if="!selectedContainer">
      <div class="row g-3">
        <div v-for="c in filteredContainers" :key="c.id" class="col-lg-4 col-md-6">
          <div class="card h-100 border-0 shadow-sm container-card" @click="selectedContainer = c" role="button">
            <div class="card-body">
              <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                  <h5 class="mb-0">{{ c.name }}</h5>
                  <small class="text-muted">{{ c.room }} &middot; {{ c.type }}</small>
                </div>
                <span :class="['badge', c.status === 'Healthy' ? 'bg-success' : c.status === 'Warning' ? 'bg-warning' : 'bg-danger']">
                  {{ c.status }}
                </span>
              </div>
              <!-- Container visual preview using actual image -->
              <div class="container-preview mb-3" :style="{ borderColor: c.status === 'Healthy' ? '#198754' : c.status === 'Warning' ? '#ffc107' : '#dc3545' }">
                <img src="/dc.png" alt="Container" class="w-100 h-100" style="object-fit:cover; border-radius:4px;" />
              </div>
              <!-- Vital stats -->
              <div class="row g-2 text-center">
                <div class="col-3">
                  <div class="small text-muted">Temp</div>
                  <div class="fw-bold" :class="c.temp > 28 ? 'text-danger' : c.temp > 25 ? 'text-warning' : 'text-success'">{{ c.temp }}°C</div>
                </div>
                <div class="col-3">
                  <div class="small text-muted">Power</div>
                  <div class="fw-bold">{{ c.powerKw }} kW</div>
                </div>
                <div class="col-3">
                  <div class="small text-muted">Network</div>
                  <div class="fw-bold" :class="c.networkHealth >= 95 ? 'text-success' : c.networkHealth >= 80 ? 'text-warning' : 'text-danger'">{{ c.networkHealth }}%</div>
                </div>
                <div class="col-3">
                  <div class="small text-muted">Util</div>
                  <div class="fw-bold">{{ c.utilization }}%</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Container Detail View -->
    <div v-else>
      <!-- Summary cards -->
      <div class="row g-3 mb-4">
        <div class="col-md-3" v-for="stat in containerStats" :key="stat.label">
          <div class="card border-0 shadow-sm">
            <div class="card-body py-2 text-center">
              <div class="small text-muted">{{ stat.label }}</div>
              <div class="h5 mb-0" :class="stat.color">{{ stat.value }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Interactive image with hotspot overlays -->
      <div class="card border-0 shadow-sm">
        <div class="card-body p-3">
          <div class="image-map-wrapper">
            <img src="/dc.png" alt="Data Center Container" class="container-img" />
            <!-- Clickable hotspot zones (positioned by % relative to image) -->
            <div v-for="zone in hotspotZones" :key="zone.id"
                 class="hotspot-zone"
                 :class="{ 'hotspot-hover': hoverZone === zone.id }"
                 :style="{ left: zone.x + '%', top: zone.y + '%', width: zone.w + '%', height: zone.h + '%' }"
                 @mouseenter="hoverZone = zone.id"
                 @mouseleave="hoverZone = null"
                 @click="showPopup(zone, $event)">
              <span class="hotspot-label">{{ zone.label }}</span>
            </div>
          </div>
        </div>
        <div class="card-footer bg-white">
          <small class="text-muted">Hover or click on highlighted areas to view component details</small>
        </div>
      </div>
    </div>

    <!-- Popup Overlay -->
    <div v-if="popup" class="popup-overlay" @click.self="popup = null">
      <div class="popup-card card shadow-lg border-0" :style="popupStyle">
        <div class="card-header d-flex justify-content-between align-items-center py-2" :style="{ background: popup.headerColor }">
          <span class="fw-bold text-white">{{ popup.title }}</span>
          <button class="btn btn-sm btn-close btn-close-white" @click="popup = null"></button>
        </div>
        <div class="card-body py-2">
          <table class="table table-sm table-borderless mb-0">
            <tr v-for="row in popup.rows" :key="row.label">
              <td class="text-muted py-1" style="width:45%">{{ row.label }}</td>
              <td class="py-1 fw-medium" :class="row.color || ''">{{ row.value }}</td>
            </tr>
          </table>
          <div v-if="popup.bars" class="mt-2">
            <div v-for="bar in popup.bars" :key="bar.label" class="mb-2">
              <div class="d-flex justify-content-between" style="font-size:0.75rem">
                <span>{{ bar.label }}</span>
                <span class="fw-bold">{{ bar.value }}%</span>
              </div>
              <div class="progress" style="height:8px">
                <div class="progress-bar" :class="bar.value > 85 ? 'bg-danger' : bar.value > 60 ? 'bg-warning' : 'bg-success'" :style="{ width: bar.value + '%' }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

// --- Room / Container overview ---
const rooms = ['Room A', 'Room B', 'Room C']
const selectedRoom = ref('all')

interface Container {
  id: number
  name: string
  room: string
  type: string
  status: 'Healthy' | 'Warning' | 'Critical'
  temp: number
  powerKw: number
  networkHealth: number
  utilization: number
}

const containers = ref<Container[]>([
  { id: 1, name: 'CTR-A01', room: 'Room A', type: '40ft High-Density', status: 'Healthy', temp: 22, powerKw: 85, networkHealth: 99, utilization: 72 },
  { id: 2, name: 'CTR-A02', room: 'Room A', type: '40ft Standard', status: 'Warning', temp: 27, powerKw: 92, networkHealth: 94, utilization: 88 },
  { id: 3, name: 'CTR-A03', room: 'Room A', type: '20ft Edge', status: 'Healthy', temp: 21, powerKw: 45, networkHealth: 100, utilization: 55 },
  { id: 4, name: 'CTR-B01', room: 'Room B', type: '40ft High-Density', status: 'Critical', temp: 31, powerKw: 110, networkHealth: 78, utilization: 95 },
  { id: 5, name: 'CTR-B02', room: 'Room B', type: '40ft Standard', status: 'Healthy', temp: 23, powerKw: 78, networkHealth: 98, utilization: 65 },
  { id: 6, name: 'CTR-C01', room: 'Room C', type: '20ft Edge', status: 'Healthy', temp: 20, powerKw: 38, networkHealth: 100, utilization: 42 },
])

const filteredContainers = computed(() =>
  selectedRoom.value === 'all' ? containers.value : containers.value.filter(c => c.room === selectedRoom.value)
)

const selectedContainer = ref<Container | null>(null)

const containerStats = computed(() => {
  const c = selectedContainer.value
  if (!c) return []
  return [
    { label: 'Temperature', value: c.temp + '°C', color: c.temp > 28 ? 'text-danger' : c.temp > 25 ? 'text-warning' : 'text-success' },
    { label: 'Power Draw', value: c.powerKw + ' kW', color: '' },
    { label: 'Network Health', value: c.networkHealth + '%', color: c.networkHealth >= 95 ? 'text-success' : 'text-warning' },
    { label: 'Utilization', value: c.utilization + '%', color: c.utilization > 85 ? 'text-danger' : '' },
  ]
})

// --- Hotspot zones (% positions mapped to the dc.png image) ---
// Image layout: door/panels on left ~0-18%, server racks in middle ~18-78%, cooling pipes top ~18-78%, battery/UPS right ~78-100%
interface HotspotZone {
  id: string
  label: string
  x: number; y: number; w: number; h: number
  type: 'door' | 'fire-panel' | 'electrical' | 'server' | 'cooling' | 'battery'
  rackIndex?: number
}

const hotspotZones: HotspotZone[] = [
  // Door
  { id: 'door', label: 'Door', x: 1, y: 10, w: 8, h: 85, type: 'door' },
  // Fire suppression panel (red box)
  { id: 'fire', label: 'Fire Panel', x: 9, y: 12, w: 5, h: 40, type: 'fire-panel' },
  // Electrical panel
  { id: 'electrical', label: 'Electrical', x: 9, y: 55, w: 8, h: 40, type: 'electrical' },
  // Server racks (7 racks spanning the middle)
  { id: 'rack1', label: 'Rack 1', x: 19, y: 10, w: 7.5, h: 85, type: 'server', rackIndex: 0 },
  { id: 'rack2', label: 'Rack 2', x: 27, y: 10, w: 7.5, h: 85, type: 'server', rackIndex: 1 },
  { id: 'rack3', label: 'Rack 3', x: 35, y: 10, w: 7.5, h: 85, type: 'server', rackIndex: 2 },
  { id: 'rack4', label: 'Rack 4', x: 43, y: 10, w: 7.5, h: 85, type: 'server', rackIndex: 3 },
  { id: 'rack5', label: 'Rack 5', x: 51, y: 10, w: 7.5, h: 85, type: 'server', rackIndex: 4 },
  { id: 'rack6', label: 'Rack 6', x: 59, y: 10, w: 7.5, h: 85, type: 'server', rackIndex: 5 },
  { id: 'rack7', label: 'Rack 7', x: 67, y: 10, w: 7.5, h: 85, type: 'server', rackIndex: 6 },
  // Cooling (top pipes area)
  { id: 'cooling', label: 'Cooling Pipes', x: 19, y: 0, w: 56, h: 10, type: 'cooling' },
  // Battery / UPS (right section - yellow modules)
  { id: 'battery', label: 'UPS / Battery', x: 78, y: 5, w: 21, h: 90, type: 'battery' },
]

const hoverZone = ref<string | null>(null)

// --- Rack mock data ---
interface RackUnit { slot: number; type: 'server' | 'network' | 'storage'; active: boolean }
interface Rack { label: string; units: RackUnit[] }

function makeRack(label: string): Rack {
  const units: RackUnit[] = []
  const types: RackUnit['type'][] = ['server', 'server', 'server', 'network', 'server', 'server', 'storage', 'server', 'server', 'server']
  for (let i = 0; i < 10; i++) {
    units.push({ slot: i, type: types[i], active: Math.random() > 0.15 })
  }
  return { label, units }
}

const containerRacks: Rack[] = [
  makeRack('Rack 1'), makeRack('Rack 2'), makeRack('Rack 3'),
  makeRack('Rack 4'), makeRack('Rack 5'), makeRack('Rack 6'),
  makeRack('Rack 7'),
]

// --- Popup system ---
interface PopupRow { label: string; value: string; color?: string }
interface PopupBar { label: string; value: number }
interface PopupData { title: string; headerColor: string; rows: PopupRow[]; bars?: PopupBar[] }

const popup = ref<PopupData | null>(null)
const popupPos = ref({ x: 0, y: 0 })

const popupStyle = computed(() => ({
  position: 'fixed' as const,
  left: popupPos.value.x + 'px',
  top: popupPos.value.y + 'px',
  zIndex: 9999,
  minWidth: '280px',
  maxWidth: '340px',
}))

const accessNames = ['Budi', 'Andi', 'Sari', 'Dewi', 'Riko', 'Fajar']

function showPopup(zone: HotspotZone, event: MouseEvent) {
  const rect = (event.currentTarget as HTMLElement).getBoundingClientRect()
  popupPos.value = {
    x: Math.min(rect.right + 8, window.innerWidth - 360),
    y: Math.max(rect.top, 10),
  }

  if (zone.type === 'door') {
    const mins = Math.floor(Math.random() * 120) + 5
    const person = accessNames[Math.floor(Math.random() * accessNames.length)]
    popup.value = {
      title: 'Container Door',
      headerColor: '#6c757d',
      rows: [
        { label: 'Status', value: 'Locked', color: 'text-success' },
        { label: 'Last Access', value: `${mins}m ago by ${person}` },
        { label: 'Access Method', value: 'RFID Badge' },
        { label: 'Lock Type', value: 'Electromagnetic + PIN' },
        { label: 'Access Today', value: `${Math.floor(Math.random() * 8) + 1} entries` },
        { label: 'Camera', value: 'Active — Recording', color: 'text-success' },
      ],
    }
  } else if (zone.type === 'fire-panel') {
    popup.value = {
      title: 'Fire Suppression Panel',
      headerColor: '#dc3545',
      rows: [
        { label: 'System', value: 'FM-200 (HFC-227ea)' },
        { label: 'Status', value: 'Armed — Standby', color: 'text-success' },
        { label: 'Smoke Detector', value: 'Normal — No Alarm', color: 'text-success' },
        { label: 'VESDA', value: 'Alert Level 0 (Clear)' },
        { label: 'Gas Pressure', value: '42 bar (OK)', color: 'text-success' },
        { label: 'Last Inspection', value: '2026-01-15' },
        { label: 'Last Discharge Test', value: '2025-11-20' },
      ],
    }
  } else if (zone.type === 'electrical') {
    popup.value = {
      title: 'Electrical Distribution Panel',
      headerColor: '#495057',
      rows: [
        { label: 'Supply', value: '3-Phase 400V / 63A' },
        { label: 'Main Breaker', value: 'ON', color: 'text-success' },
        { label: 'Phase A Current', value: '48.2 A' },
        { label: 'Phase B Current', value: '51.7 A' },
        { label: 'Phase C Current', value: '49.8 A' },
        { label: 'Power Factor', value: '0.97' },
        { label: 'Frequency', value: '50.01 Hz' },
        { label: 'Total Load', value: `${selectedContainer.value?.powerKw ?? 0} kW` },
      ],
    }
  } else if (zone.type === 'server' && zone.rackIndex !== undefined) {
    const rack = containerRacks[zone.rackIndex]
    const cpu = Math.floor(Math.random() * 60) + 20
    const mem = Math.floor(Math.random() * 40) + 40
    const disk = Math.floor(Math.random() * 50) + 30
    const net = Math.floor(Math.random() * 70) + 10
    const activeCount = rack.units.filter(u => u.active).length
    popup.value = {
      title: `${rack.label} — ${activeCount}/${rack.units.length} Active`,
      headerColor: '#1565C0',
      rows: [
        { label: 'Servers', value: `${rack.units.filter(u => u.type === 'server').length} units` },
        { label: 'Network Switches', value: `${rack.units.filter(u => u.type === 'network').length} units` },
        { label: 'Storage Arrays', value: `${rack.units.filter(u => u.type === 'storage').length} units` },
        { label: 'Inlet Temp', value: `${21 + Math.floor(Math.random() * 5)}°C` },
        { label: 'Power Draw', value: `${(8 + Math.random() * 7).toFixed(1)} kW` },
        { label: 'Network Throughput', value: `${(Math.random() * 40 + 5).toFixed(1)} Gbps` },
      ],
      bars: [
        { label: 'CPU Usage', value: cpu },
        { label: 'Memory Usage', value: mem },
        { label: 'Disk I/O', value: disk },
        { label: 'Network I/O', value: net },
      ],
    }
  } else if (zone.type === 'cooling') {
    popup.value = {
      title: 'Cooling System',
      headerColor: '#0288D1',
      rows: [
        { label: 'Type', value: 'In-Row Cooling (DX)' },
        { label: 'Supply Temp', value: '16°C', color: 'text-primary' },
        { label: 'Return Temp', value: '28°C', color: 'text-warning' },
        { label: 'Delta T', value: '12°C' },
        { label: 'Capacity', value: '120 kW' },
        { label: 'Current Load', value: '87 kW (73%)' },
        { label: 'Refrigerant', value: 'R410A — Normal', color: 'text-success' },
        { label: 'Airflow', value: '4,200 CFM' },
      ],
    }
  } else if (zone.type === 'battery') {
    popup.value = {
      title: 'UPS / Battery Bank',
      headerColor: '#F9A825',
      rows: [
        { label: 'UPS Model', value: 'Eaton 9395P 200kVA' },
        { label: 'Status', value: 'Online — Normal', color: 'text-success' },
        { label: 'Load', value: '78 kW (39%)' },
        { label: 'Battery SOC', value: '98%', color: 'text-success' },
        { label: 'Battery SOH', value: '94%' },
        { label: 'Runtime Remaining', value: '18 min at full load' },
        { label: 'Last Test', value: '2026-02-20 03:00' },
        { label: 'Modules', value: '12 / 12 Online', color: 'text-success' },
      ],
      bars: [
        { label: 'Battery Charge', value: 98 },
        { label: 'UPS Load', value: 39 },
      ],
    }
  }
}
</script>

<style scoped>
.container-card {
  transition: transform 0.15s, box-shadow 0.15s;
}
.container-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 6px 20px rgba(0,0,0,0.12) !important;
}

.container-preview {
  border: 2px solid;
  border-radius: 6px;
  height: 70px;
  overflow: hidden;
  background: #000;
}

/* Image map wrapper */
.image-map-wrapper {
  position: relative;
  display: inline-block;
  width: 100%;
}
.container-img {
  width: 100%;
  height: auto;
  display: block;
  border-radius: 8px;
}
.hotspot-zone {
  position: absolute;
  border: 2px solid transparent;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: flex-end;
  justify-content: center;
  padding-bottom: 2px;
}
.hotspot-zone:hover,
.hotspot-hover {
  border-color: rgba(255, 255, 255, 0.8);
  background: rgba(255, 255, 255, 0.12);
  box-shadow: 0 0 12px rgba(255, 255, 255, 0.3), inset 0 0 20px rgba(255, 255, 255, 0.05);
}
.hotspot-label {
  font-size: 0.65rem;
  font-weight: 600;
  color: transparent;
  text-shadow: none;
  background: transparent;
  padding: 1px 4px;
  border-radius: 3px;
  transition: all 0.2s;
  white-space: nowrap;
  pointer-events: none;
}
.hotspot-zone:hover .hotspot-label,
.hotspot-hover .hotspot-label {
  color: #fff;
  background: rgba(0,0,0,0.6);
  text-shadow: 0 1px 2px rgba(0,0,0,0.5);
}

/* Popup */
.popup-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  z-index: 9998;
}
.popup-card {
  animation: popIn 0.15s ease-out;
}
@keyframes popIn {
  from { opacity: 0; transform: scale(0.95); }
  to { opacity: 1; transform: scale(1); }
}
</style>
