<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { cctvCamerasApi, type CctvCamera, recordingStatsApi, type RecordingStat } from '../../services/api'

const cameras = ref<CctvCamera[]>([])
const recordingStats = ref<RecordingStat[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<CctvCamera | null>(null)
const defaultForm = { name: '', location: '', status: 'online', recording: 'active', resolution: '', storage: '', lastFrame: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  const [c, r] = await Promise.all([cctvCamerasApi.getAll(), recordingStatsApi.getAll()])
  cameras.value = c
  recordingStats.value = r
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(camera: CctvCamera) {
  editing.value = camera
  form.value = { name: camera.name, location: camera.location, status: camera.status, recording: camera.recording, resolution: camera.resolution, storage: camera.storage, lastFrame: camera.lastFrame }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await cctvCamerasApi.update(editing.value.id, form.value)
    else await cctvCamerasApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await cctvCamerasApi.remove(id)
  await loadData()
}

const statusColor = {
  online: 'success',
  offline: 'danger',
} as const

const recordingColor = {
  active: 'success',
  paused: 'warning',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">CCTV Integration</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>

    <!-- System Stats -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-success mb-1">{{ cameras.filter(c => c.status === 'online').length }}/{{ cameras.length }}</h3>
            <small class="text-muted">Cameras Online</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-info mb-1">6.5 TB</h3>
            <small class="text-muted">Total Storage</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-warning mb-1">4.82 TB</h3>
            <small class="text-muted">Used Storage</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-primary mb-1">99.8%</h3>
            <small class="text-muted">System Uptime</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Cameras List -->
    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
        <span>Camera Status</span>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Camera</button>
      </div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Camera Name</th>
              <th>Location</th>
              <th>Status</th>
              <th>Recording</th>
              <th>Resolution</th>
              <th>Storage Used</th>
              <th>Last Frame</th>
              <th style="width:120px">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="camera in cameras" :key="camera.id">
              <td class="fw-semibold">{{ camera.name }}</td>
              <td class="text-muted">{{ camera.location }}</td>
              <td><span class="badge" :class="`bg-${statusColor[camera.status as keyof typeof statusColor]}`">{{ camera.status }}</span></td>
              <td><span class="badge" :class="`bg-${recordingColor[camera.recording as keyof typeof recordingColor]}`">{{ camera.recording }}</span></td>
              <td><span class="badge bg-light text-dark">{{ camera.resolution }}</span></td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <span style="width: 40px;">{{ camera.storage }}</span>
                  <div class="progress" style="width: 80px; height: 6px;">
                    <div class="progress-bar" :class="parseInt(camera.storage) > 70 ? 'bg-danger' : parseInt(camera.storage) > 50 ? 'bg-warning' : 'bg-success'" :style="{ width: camera.storage }"></div>
                  </div>
                </div>
              </td>
              <td class="text-muted">{{ camera.lastFrame }}</td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(camera)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(camera.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Storage Stats by Location -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Storage Statistics by Location</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Location</th>
              <th>Cameras</th>
              <th>Total Storage</th>
              <th>Used Storage</th>
              <th>Utilization</th>
              <th>Retention</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="stat in recordingStats" :key="stat.location">
              <td class="fw-semibold">{{ stat.location }}</td>
              <td><span class="badge bg-light text-dark">{{ stat.cameras }}</span></td>
              <td>{{ stat.totalStorage }}</td>
              <td>{{ stat.usedStorage }}</td>
              <td>
                <div class="progress" style="height: 8px;">
                  <div class="progress-bar bg-primary" :style="{ width: ((parseFloat(stat.usedStorage.replace('TB', '')) / parseFloat(stat.totalStorage.replace('TB', '')))) * 100 + '%' }"></div>
                </div>
              </td>
              <td class="text-muted">{{ stat.retention }}</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Camera</h5>
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
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="online">online</option>
                <option value="offline">offline</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Recording</label>
              <select v-model="form.recording" class="form-select">
                <option value="active">active</option>
                <option value="paused">paused</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Resolution</label>
              <input v-model="form.resolution" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Storage</label>
              <input v-model="form.storage" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Last Frame</label>
              <input v-model="form.lastFrame" type="text" class="form-control" />
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
