<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { rackDevicesApi, type RackDevice, pendingAssetsApi, type PendingAsset } from '../../services/api'

const allDevices = ref<RackDevice[]>([])
const pendingAssets = ref<PendingAsset[]>([])
const loading = ref(true)
const selectedRack = ref('CAB-B03')
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PendingAsset | null>(null)

const defaultForm = { name: '', size: 1, type: '', targetRack: '' }
const form = ref({ ...defaultForm })

interface Slot {
  u: number
  device: string | null
  size: number
}

const rackSlots = computed(() => {
  const grouped: Record<string, Slot[]> = {}
  for (const d of allDevices.value) {
    if (!grouped[d.rackId]) {
      grouped[d.rackId] = Array.from({ length: 42 }, (_, i) => ({ u: 42 - i, device: null, size: 1 }))
    }
    const rack = grouped[d.rackId]
    for (let u = d.startU; u < d.startU + d.sizeU; u++) {
      const slot = rack.find(s => s.u === u)
      if (slot) {
        slot.device = d.name
        slot.size = d.sizeU
      }
    }
  }
  return grouped
})

const slots = computed(() => rackSlots.value[selectedRack.value] ?? [])
const freeU = computed(() => slots.value.filter(s => !s.device).length)

async function loadData() {
  const [devices, pending] = await Promise.all([rackDevicesApi.getAll(), pendingAssetsApi.getAll()])
  allDevices.value = devices
  pendingAssets.value = pending
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: PendingAsset) {
  editing.value = item
  form.value = { name: item.name, size: item.size, type: item.type, targetRack: item.targetRack }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await pendingAssetsApi.update(editing.value.id, form.value)
    } else {
      await pendingAssetsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await pendingAssetsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Asset Placement</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3">
        <div class="col-lg-4">
          <div class="card border-0 shadow-sm mb-3">
            <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
              <span>Pending Assets</span>
              <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
            </div>
            <div class="card-body">
              <div v-for="a in pendingAssets" :key="a.id" class="border rounded p-2 mb-2 d-flex justify-content-between align-items-center" style="cursor: grab;">
                <div>
                  <span class="fw-semibold">{{ a.name }}</span>
                  <br><small class="text-muted">{{ a.type }} &middot; {{ a.size }}U</small>
                </div>
                <div class="d-flex align-items-center gap-1">
                  <span class="badge bg-primary">{{ a.size }}U</span>
                  <button class="btn btn-sm btn-outline-secondary" @click="openEdit(a)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(a.id)">Delete</button>
                </div>
              </div>
              <small class="text-muted">Drag assets to an empty slot in the rack.</small>
            </div>
          </div>

          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <h4 class="text-success mb-0">{{ freeU }}U</h4>
              <small class="text-muted">Free slots in {{ selectedRack }}</small>
            </div>
          </div>
        </div>

        <div class="col-lg-8">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-transparent fw-semibold">{{ selectedRack }} — Rack View</div>
            <div class="card-body p-0">
              <table class="table table-bordered table-sm mb-0 text-center" style="table-layout: fixed;">
                <thead class="table-dark">
                  <tr><th style="width:50px">U</th><th>Device</th></tr>
                </thead>
                <tbody>
                  <tr v-for="slot in slots" :key="slot.u" :class="slot.device ? '' : 'table-success'" style="height: 26px;">
                    <td class="small fw-semibold align-middle">{{ slot.u }}</td>
                    <td v-if="slot.device" class="bg-primary bg-opacity-75 text-white small align-middle fw-semibold">{{ slot.device }}</td>
                    <td v-else class="small align-middle text-success">[ empty ]</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Pending Asset</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Size (U)</label>
              <input v-model.number="form.size" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Target Rack</label>
              <input v-model="form.targetRack" type="text" class="form-control" />
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
