<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { sensorsApi, type Sensor } from '../../services/api'

const sensors = ref<Sensor[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Sensor | null>(null)

const defaultForm = { sensorId: '', name: '', type: 'Temperature', location: '', status: 'active', minThreshold: '', maxThreshold: '', pollingInterval: '30' }
const form = ref({ ...defaultForm })

const typeCounts = computed(() => {
  const counts: Record<string, number> = {}
  for (const s of sensors.value) {
    counts[s.type] = (counts[s.type] || 0) + 1
  }
  return Object.entries(counts).map(([type, count]) => ({ type, count }))
})

async function loadData() {
  sensors.value = await sensorsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
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
    if (editing.value) await sensorsApi.update(editing.value.id, form.value)
    else await sensorsApi.create(form.value)
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

<template>
  <div>
    <h4 class="mb-4">Sensor Management</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
    <div class="row g-3 mb-4">
      <div v-for="tc in typeCounts" :key="tc.type" class="col">
        <div class="card border-0 shadow-sm text-center h-100">
          <div class="card-body py-3">
            <h4 class="text-primary mb-0">{{ tc.count }}</h4>
            <small class="text-muted">{{ tc.type }}</small>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
        <span class="fw-semibold">Registered Sensors</span>
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Sensor</button>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Sensor ID</th><th>Name</th><th>Type</th><th>Location</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="s in sensors" :key="s.id">
                <td class="fw-semibold font-monospace small">{{ s.sensorId }}</td>
                <td>{{ s.name }}</td>
                <td>{{ s.type }}</td>
                <td class="text-muted">{{ s.location }}</td>
                <td>
                  <span class="badge" :class="{ 'bg-success': s.status === 'active', 'bg-warning text-dark': s.status === 'warning', 'bg-secondary': s.status === 'inactive' }">{{ s.status }}</span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(s)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(s.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    </template>

    <!-- Modal -->
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
                <option>Airflow</option>
                <option>Leak Detection</option>
                <option>Pressure</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Location</label>
              <input v-model="form.location" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>active</option>
                <option>warning</option>
                <option>inactive</option>
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
              <label class="form-label">Polling Interval (s)</label>
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
