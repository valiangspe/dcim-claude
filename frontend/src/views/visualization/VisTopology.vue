<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1 class="h3 mb-0">Network Topology</h1>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Device</button>
    </div>

    <div class="row mb-3">
      <div class="col-auto">
        <div class="btn-group btn-group-sm" role="group">
          <button type="button" class="btn" :class="selectedView === 'layer2' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedView = 'layer2'">Layer 2</button>
          <button type="button" class="btn" :class="selectedView === 'layer3' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedView = 'layer3'">Layer 3</button>
          <button type="button" class="btn" :class="selectedView === 'all' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedView = 'all'">All Layers</button>
        </div>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-body" style="background-color: #f8f9fa; min-height: 500px; position: relative;">
        <div class="topology-layer" style="margin-bottom: 40px;">
          <h6 class="text-muted mb-3">Core Layer</h6>
          <div class="d-flex gap-3 justify-content-center">
            <div v-for="device in coreDevices" :key="device.id" class="topology-node">
              <div class="node-box" :class="getNodeClass(device)"
                   style="width: 100px; padding: 10px; border: 2px solid #666; border-radius: 4px; text-align: center;">
                <div style="font-weight: bold; font-size: 12px;">{{ device.name }}</div>
                <div style="font-size: 10px; color: #666;">{{ device.status }}</div>
              </div>
            </div>
          </div>
        </div>

        <div class="topology-layer" style="margin-bottom: 40px;">
          <h6 class="text-muted mb-3">Distribution Layer</h6>
          <div class="d-flex gap-3 justify-content-center">
            <div v-for="device in distDevices" :key="device.id" class="topology-node">
              <div class="node-box" :class="getNodeClass(device)"
                   style="width: 90px; padding: 8px; border: 2px solid #999; border-radius: 4px; text-align: center; font-size: 11px;">
                {{ device.name }}
              </div>
            </div>
          </div>
        </div>

        <div class="topology-layer">
          <h6 class="text-muted mb-3">Access Layer</h6>
          <div class="d-flex gap-2 justify-content-center" style="flex-wrap: wrap;">
            <div v-for="device in accessDevices" :key="device.id" class="topology-node">
              <div class="node-box" :class="getNodeClass(device)"
                   style="width: 70px; padding: 6px; border: 1px solid #ccc; border-radius: 3px; text-align: center; font-size: 9px;">
                {{ device.name }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">Network Links</h5>
      </div>
      <div class="card-body">
        <table class="table table-sm mb-0">
          <thead class="table-light">
            <tr>
              <th>Source</th>
              <th>Destination</th>
              <th>Status</th>
              <th>Utilization</th>
              <th>Speed</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="link in links" :key="link.id">
              <td>{{ link.source }}</td>
              <td>{{ link.destination }}</td>
              <td>
                <span :class="['badge', link.status === 'Up' ? 'bg-success' : 'bg-danger']">
                  {{ link.status }}
                </span>
              </td>
              <td>{{ link.utilization }}%</td>
              <td>{{ link.speed }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header fw-semibold">Network Devices</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Name</th><th>Status</th><th>Type</th><th>Layer</th><th>Actions</th></tr>
          </thead>
          <tbody>
            <tr v-for="device in allDevices" :key="device.id">
              <td class="fw-medium">{{ device.name }}</td>
              <td><span :class="['badge', device.status === 'UP' ? 'bg-success' : device.status === 'DOWN' ? 'bg-danger' : 'bg-warning']">{{ device.status }}</span></td>
              <td>{{ device.type }}</td>
              <td>{{ device.layer }}</td>
              <td>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(device)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(device.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Network Device</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Status</label><input v-model="form.status" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Type</label><input v-model="form.type" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Layer</label><input v-model="form.layer" type="text" class="form-control" /></div>
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
import { networkDevicesApi, type NetworkDevice, visLinksApi, type VisLink } from '../../services/api'

const selectedView = ref('all')
const allDevices = ref<NetworkDevice[]>([])
const links = ref<VisLink[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<NetworkDevice | null>(null)
const saving = ref(false)

const defaultForm = { name: '', status: '', type: '', layer: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  const [devices, linksData] = await Promise.all([
    networkDevicesApi.getAll(),
    visLinksApi.getAll()
  ])
  allDevices.value = devices
  links.value = linksData
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const coreDevices = computed(() => allDevices.value.filter(d => d.layer === 'core'))
const distDevices = computed(() => allDevices.value.filter(d => d.layer === 'distribution'))
const accessDevices = computed(() => allDevices.value.filter(d => d.layer === 'access'))

function getNodeClass(device: { status: string }): string {
  if (device.status === 'UP') return 'bg-success';
  if (device.status === 'DOWN') return 'bg-danger';
  return 'bg-warning';
}

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: NetworkDevice) {
  editing.value = item
  form.value = { name: item.name, status: item.status, type: item.type, layer: item.layer }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await networkDevicesApi.update(editing.value.id, form.value)
    } else {
      await networkDevicesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this device?')) return
  await networkDevicesApi.remove(id)
  await loadData()
}
</script>
