<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { chillersApi, type Chiller } from '../../services/api'

const chillers = ref<Chiller[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Chiller | null>(null)

const defaultForm = { name: '', type: '', capacity: 0, load: 0, cop: 0, eer: 0, supplyW: 0, returnW: 0, flow: 0, status: 'running' }
const form = ref({ ...defaultForm })

const hourly = [
  { time: '00:00', ch1Cop: 5.4, ch2Cop: 5.0 },
  { time: '04:00', ch1Cop: 5.5, ch2Cop: 5.1 },
  { time: '08:00', ch1Cop: 5.3, ch2Cop: 4.9 },
  { time: '12:00', ch1Cop: 5.0, ch2Cop: 4.6 },
  { time: '16:00', ch1Cop: 5.1, ch2Cop: 4.7 },
  { time: '20:00', ch1Cop: 5.3, ch2Cop: 4.9 },
]

async function loadData() {
  chillers.value = await chillersApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: Chiller) {
  editing.value = item
  form.value = { name: item.name, type: item.type, capacity: item.capacity, load: item.load, cop: item.cop, eer: item.eer, supplyW: item.supplyW, returnW: item.returnW, flow: item.flow, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await chillersApi.update(editing.value.id, form.value)
    } else {
      await chillersApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await chillersApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Chiller Efficiency</h4>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Chiller</button>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="c in chillers" :key="c.id" class="col-lg-6">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
              <span class="fw-semibold">{{ c.name }} <span class="text-muted fw-normal">({{ c.type }})</span></span>
              <div>
                <span class="badge bg-success me-2">{{ c.status }}</span>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(c)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(c.id)">Delete</button>
              </div>
            </div>
            <div class="card-body">
              <div class="row text-center mb-3">
                <div class="col-4">
                  <h3 class="text-primary mb-0">{{ c.cop.toFixed(1) }}</h3>
                  <small class="text-muted">COP</small>
                </div>
                <div class="col-4">
                  <h3 class="text-info mb-0">{{ c.eer.toFixed(1) }}</h3>
                  <small class="text-muted">EER</small>
                </div>
                <div class="col-4">
                  <h3 class="text-warning mb-0">{{ (c.load / c.capacity * 100).toFixed(0) }}%</h3>
                  <small class="text-muted">Load</small>
                </div>
              </div>
              <div class="progress mb-3" style="height: 8px;">
                <div class="progress-bar bg-primary" :style="{ width: (c.load / c.capacity * 100) + '%' }"></div>
              </div>
              <div class="row small text-muted">
                <div class="col-6">Capacity: <strong>{{ c.capacity }} kW</strong></div>
                <div class="col-6">Load: <strong>{{ c.load }} kW</strong></div>
                <div class="col-6">Supply Water: <strong>{{ c.supplyW }}°C</strong></div>
                <div class="col-6">Return Water: <strong>{{ c.returnW }}°C</strong></div>
                <div class="col-6">Flow Rate: <strong>{{ c.flow }} L/min</strong></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">COP Trend (24h)</div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Time</th><th class="text-end">Chiller-1 COP</th><th class="text-end">Chiller-2 COP</th></tr>
            </thead>
            <tbody>
              <tr v-for="h in hourly" :key="h.time">
                <td>{{ h.time }}</td>
                <td class="text-end fw-semibold">{{ h.ch1Cop.toFixed(1) }}</td>
                <td class="text-end fw-semibold">{{ h.ch2Cop.toFixed(1) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Chiller</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Capacity (kW)</label>
              <input v-model.number="form.capacity" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Load (kW)</label>
              <input v-model.number="form.load" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">COP</label>
              <input v-model.number="form.cop" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">EER</label>
              <input v-model.number="form.eer" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Supply Water (°C)</label>
              <input v-model.number="form.supplyW" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Return Water (°C)</label>
              <input v-model.number="form.returnW" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Flow (L/min)</label>
              <input v-model.number="form.flow" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>running</option>
                <option>standby</option>
                <option>offline</option>
              </select>
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
