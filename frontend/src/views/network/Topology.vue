<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { networkLayersApi, type NetworkLayer } from '../../services/api'

const layers = ref<NetworkLayer[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<NetworkLayer | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { name: '', devices: '', connections: '' }
const form = ref({ ...defaultForm })

function parseDevices(devices: string): string[] {
  try { return JSON.parse(devices) } catch { return devices.split(',').map(d => d.trim()) }
}

async function loadData() {
  layers.value = await networkLayersApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: NetworkLayer) {
  editing.value = item
  form.value = { name: item.name, devices: item.devices, connections: item.connections }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await networkLayersApi.update(editing.value.id, form.value)
    else await networkLayersApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: NetworkLayer) {
  if (!confirm(`Delete layer "${item.name}"?`)) return
  deleting.value = item.id
  try { await networkLayersApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Network Topology</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-end mb-3"><button class="btn btn-sm btn-primary" @click="openCreate">+ Add Layer</button></div>
    <div class="text-center mb-4">
      <div class="d-inline-block border rounded p-3 bg-primary bg-opacity-10 mb-2"><strong>Internet / ISP</strong></div>
      <div class="text-muted">|</div>
      <div v-for="(layer, i) in layers" :key="layer.id" class="mb-2">
        <div class="card border-0 shadow-sm mx-auto" style="max-width:700px">
          <div class="card-body py-2">
            <div class="d-flex justify-content-between align-items-start">
              <div class="flex-fill text-center">
                <div class="fw-semibold small text-muted mb-1">{{ layer.name }}</div>
                <div class="d-flex flex-wrap justify-content-center gap-2 mb-1">
                  <span v-for="d in parseDevices(layer.devices)" :key="d" class="badge bg-dark">{{ d }}</span>
                </div>
                <div class="small text-muted">{{ layer.connections }}</div>
              </div>
              <div class="ms-2">
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(layer)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(layer)" :disabled="deleting === layer.id">
                  <span v-if="deleting === layer.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
                </button>
              </div>
            </div>
          </div>
        </div>
        <div v-if="i < layers.length - 1" class="text-muted">&#8597;</div>
      </div>
    </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Network Layer</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Devices (comma-separated or JSON array)</label><input v-model="form.devices" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Connections</label><input v-model="form.connections" type="text" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
