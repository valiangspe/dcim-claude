<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { rackDevicesApi, type RackDevice } from '../../services/api'

const allDevices = ref<RackDevice[]>([])
const loading = ref(true)
const selectedRack = ref('CAB-A01')
const showModal = ref(false)
const saving = ref(false)
const editing = ref<RackDevice | null>(null)

const defaultForm = { rackId: '', name: '', startU: 1, sizeU: 1, type: '', color: 'bg-primary' }
const form = ref({ ...defaultForm })

const rackData = computed(() => {
  const grouped: Record<string, { totalU: number; devices: RackDevice[] }> = {}
  for (const d of allDevices.value) {
    if (!grouped[d.rackId]) grouped[d.rackId] = { totalU: 42, devices: [] }
    grouped[d.rackId].devices.push(d)
  }
  return grouped
})

const rackOptions = computed(() => Object.keys(rackData.value))

interface SlotRow {
  u: number
  device: RackDevice | null
  isTop: boolean
}

const slots = computed((): SlotRow[] => {
  const data = rackData.value[selectedRack.value]
  if (!data) return []
  const result: SlotRow[] = []
  for (let u = data.totalU; u >= 1; u--) {
    const dev = data.devices.find(d => u >= d.startU && u < d.startU + d.sizeU) ?? null
    const isTop = dev ? u === dev.startU + dev.sizeU - 1 : false
    result.push({ u, device: dev, isTop })
  }
  return result
})

const devices = computed(() => rackData.value[selectedRack.value]?.devices ?? [])

async function loadData() {
  allDevices.value = await rackDevicesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm, rackId: selectedRack.value }
  showModal.value = true
}

function openEdit(item: RackDevice) {
  editing.value = item
  form.value = { rackId: item.rackId, name: item.name, startU: item.startU, sizeU: item.sizeU, type: item.type, color: item.color }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await rackDevicesApi.update(editing.value.id, form.value)
    } else {
      await rackDevicesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await rackDevicesApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Rack Elevation</h4>
      <div class="d-flex gap-2">
        <select v-model="selectedRack" class="form-select" style="width: 200px;">
          <option v-for="r in rackOptions" :key="r" :value="r">{{ r }}</option>
        </select>
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Device</button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row">
        <div class="col-lg-6">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-transparent fw-semibold">{{ selectedRack }} — Front View</div>
            <div class="card-body p-0">
              <table class="table table-bordered table-sm mb-0" style="table-layout: fixed;">
                <thead class="table-dark">
                  <tr><th style="width:50px" class="text-center">U</th><th>Device</th></tr>
                </thead>
                <tbody>
                  <tr v-for="slot in slots" :key="slot.u" style="height: 24px;">
                    <td class="align-middle text-center small fw-semibold">{{ slot.u }}</td>
                    <td
                      v-if="slot.device"
                      :class="[slot.device.color, 'text-white', 'align-middle', 'small', 'fw-semibold']"
                      :style="slot.isTop ? {} : { borderTop: 'none' }"
                    >
                      <template v-if="slot.isTop">{{ slot.device.name }} ({{ slot.device.sizeU }}U)</template>
                    </td>
                    <td v-else class="bg-light text-muted small align-middle text-center">—</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div class="col-lg-6">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-transparent fw-semibold">Device List</div>
            <div class="card-body p-0">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr><th>Device</th><th>Type</th><th class="text-end">Start U</th><th class="text-end">Size</th><th>Actions</th></tr>
                </thead>
                <tbody>
                  <tr v-for="d in devices" :key="d.id">
                    <td class="fw-semibold">{{ d.name }}</td>
                    <td><span class="badge" :class="d.color">{{ d.type }}</span></td>
                    <td class="text-end">U{{ d.startU }}</td>
                    <td class="text-end">{{ d.sizeU }}U</td>
                    <td>
                      <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(d)">Edit</button>
                      <button class="btn btn-sm btn-outline-danger" @click="remove(d.id)">Delete</button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Device</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Rack ID</label>
              <input v-model="form.rackId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Start U</label>
              <input v-model.number="form.startU" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Size (U)</label>
              <input v-model.number="form.sizeU" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Color (CSS class)</label>
              <input v-model="form.color" type="text" class="form-control" />
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
