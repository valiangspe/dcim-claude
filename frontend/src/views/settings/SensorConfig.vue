<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Sensor Configuration</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Sensor</button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>
    <div v-else class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Sensor ID</th>
                <th>Name</th>
                <th>Type</th>
                <th>Location</th>
                <th>Min Threshold</th>
                <th>Max Threshold</th>
                <th>Polling Interval</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="sensor in sensors" :key="sensor.id">
                <td class="font-monospace small">{{ sensor.sensorId }}</td>
                <td>{{ sensor.name }}</td>
                <td>
                  <span class="badge bg-info">{{ sensor.type }}</span>
                </td>
                <td class="small">{{ sensor.location }}</td>
                <td>{{ sensor.minThreshold }}</td>
                <td>{{ sensor.maxThreshold }}</td>
                <td class="small">{{ sensor.pollingInterval }}</td>
                <td>
                  <span :class="['badge', sensor.status === 'Online' ? 'bg-success' : 'bg-secondary']">
                    {{ sensor.status }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(sensor)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(sensor.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Sensor</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Sensor ID</label>
              <input v-model="form.sensorId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <select v-model="form.type" class="form-select">
                <option>Temperature</option>
                <option>Humidity</option>
                <option>Power</option>
                <option>Air Flow</option>
                <option>Pressure</option>
                <option>Water Leak Detection</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Location</label>
              <input v-model="form.location" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>Online</option>
                <option>Offline</option>
                <option>Maintenance</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Min Threshold</label>
              <input v-model="form.minThreshold" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Max Threshold</label>
              <input v-model="form.maxThreshold" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Polling Interval</label>
              <input v-model="form.pollingInterval" type="text" class="form-control" />
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

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { sensorsApi, type Sensor } from '../../services/api'

const sensors = ref<Sensor[]>([])
const loading = ref(true)
const error = ref('')
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Sensor | null>(null)

const defaultForm = { sensorId: '', name: '', type: 'Temperature', location: '', status: 'Online', minThreshold: '', maxThreshold: '', pollingInterval: '' }
const form = ref({ ...defaultForm })

async function loadData() {
  sensors.value = await sensorsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'Failed to load sensors'
  } finally {
    loading.value = false
  }
})

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(sensor: Sensor) {
  editing.value = sensor
  form.value = { sensorId: sensor.sensorId, name: sensor.name, type: sensor.type, location: sensor.location, status: sensor.status, minThreshold: sensor.minThreshold, maxThreshold: sensor.maxThreshold, pollingInterval: sensor.pollingInterval }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await sensorsApi.update(editing.value.id, form.value)
    } else {
      await sensorsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await sensorsApi.remove(id)
  await loadData()
}
</script>
