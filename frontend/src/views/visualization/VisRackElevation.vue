<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Rack Elevation View</h1>
      </div>
      <div class="col-auto">
        <select class="form-select form-select-sm" style="width: auto;" v-model="selectedRack">
          <option v-for="rack in rackList" :key="rack" :value="rack">
            {{ rack }}
          </option>
        </select>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Rack</button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="row">
      <div class="col-lg-8">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">{{ selectedRack }} - U1 to U48</h5>
          </div>
          <div class="card-body" style="background-color: #f8f9fa;">
            <div class="elevation-view" style="border: 2px solid #333; width: 200px; margin: auto;">
              <div v-for="device in getRackDevices()" :key="device.u"
                   class="device-slot"
                   :class="getDeviceColor(device)"
                   style="height: 30px; border-bottom: 1px solid #999; padding: 4px; font-size: 11px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; color: white; font-weight: bold;">
                U{{ device.u }}: {{ device.name }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Rack Details</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label text-muted">Name</label>
              <p class="fw-medium">{{ selectedRack }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label text-muted">Total U</label>
              <p class="fw-medium">48U</p>
            </div>
            <div class="mb-3">
              <label class="form-label text-muted">Used U</label>
              <p class="fw-medium">38U (79%)</p>
            </div>
            <div class="mb-3">
              <label class="form-label text-muted">Power Supply</label>
              <p class="fw-medium">2x 32A 208V (6.6 kW)</p>
            </div>
            <div class="mb-3">
              <label class="form-label text-muted">Current Temperature</label>
              <p class="fw-medium">23.5C</p>
            </div>
            <div class="mb-0">
              <label class="form-label text-muted">Status</label>
              <div>
                <span class="badge bg-success">Operational</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header fw-semibold">Rack Data</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Rack ID</th><th>Hall</th><th>Status</th><th class="text-end">Utilization</th><th>Actions</th></tr>
          </thead>
          <tbody>
            <tr v-for="rack in visRacks" :key="rack.id">
              <td class="fw-medium">{{ rack.rackId }}</td>
              <td>{{ rack.hall }}</td>
              <td><span :class="['badge', rack.status === 'Optimal' ? 'bg-success' : rack.status === 'Warning' ? 'bg-warning' : 'bg-danger']">{{ rack.status }}</span></td>
              <td class="text-end">{{ rack.utilization }}%</td>
              <td>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(rack)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(rack.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Rack</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Rack ID</label><input v-model="form.rackId" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Hall</label><input v-model="form.hall" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Status</label><input v-model="form.status" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Utilization</label><input v-model.number="form.utilization" type="number" class="form-control" /></div>
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
import { rackDevicesApi, type RackDevice, visRacksApi, type VisRack } from '../../services/api'

const selectedRack = ref('R01-A')
const allDevices = ref<RackDevice[]>([])
const visRacks = ref<VisRack[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<VisRack | null>(null)
const saving = ref(false)

const defaultForm = { rackId: '', hall: '', status: '', utilization: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  const [devices, racks] = await Promise.all([
    rackDevicesApi.getAll(),
    visRacksApi.getAll()
  ])
  allDevices.value = devices
  visRacks.value = racks
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const rackList = computed(() => {
  const ids = new Set<string>()
  allDevices.value.forEach(d => ids.add(d.rackId))
  return Array.from(ids).sort()
})

const devicesByRack = computed(() => {
  const map: Record<string, RackDevice[]> = {}
  allDevices.value.forEach(d => {
    if (!map[d.rackId]) map[d.rackId] = []
    map[d.rackId].push(d)
  })
  return map
})

function getRackDevices() {
  const rackDevices = devicesByRack.value[selectedRack.value] || []
  const filledUs = new Set(rackDevices.map(d => d.startU))
  const allSlots = []
  for (let u = 48; u >= 1; u--) {
    if (filledUs.has(u)) {
      const device = rackDevices.find(d => d.startU === u)!
      allSlots.push({ u, name: device.name, type: device.type })
    } else {
      allSlots.push({ u, name: 'Empty', type: 'empty' })
    }
  }
  return allSlots
}

function getDeviceColor(device: { type: string }): string {
  if (device.type === 'empty') return 'bg-light';
  if (device.type === 'Server') return 'bg-primary';
  if (device.type === 'Storage') return 'bg-info';
  if (device.type === 'Network') return 'bg-warning text-dark';
  if (device.type === 'PDU') return 'bg-danger';
  return 'bg-secondary';
}

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: VisRack) {
  editing.value = item
  form.value = { rackId: item.rackId, hall: item.hall, status: item.status, utilization: item.utilization }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await visRacksApi.update(editing.value.id, form.value)
    } else {
      await visRacksApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this rack?')) return
  await visRacksApi.remove(id)
  await loadData()
}
</script>
