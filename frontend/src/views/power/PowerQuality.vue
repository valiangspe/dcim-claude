<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { powerPhasesApi, type PowerPhase, powerEventsApi, type PowerEvent } from '../../services/api'

const phases = ref<PowerPhase[]>([])

const harmonics = [
  { order: '3rd', l1: 2.1, l2: 2.4, l3: 2.2 },
  { order: '5th', l1: 1.8, l2: 2.0, l3: 1.9 },
  { order: '7th', l1: 0.9, l2: 1.1, l3: 1.0 },
  { order: '9th', l1: 0.4, l2: 0.5, l3: 0.4 },
  { order: '11th', l1: 0.3, l2: 0.3, l3: 0.3 },
]

const events = ref<PowerEvent[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PowerPhase | null>(null)

const defaultForm = { name: '', voltage: 0, current: 0, frequency: 0, thd: 0, pf: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  const [phasesData, eventsData] = await Promise.all([
    powerPhasesApi.getAll(),
    powerEventsApi.getAll(),
  ])
  phases.value = phasesData
  events.value = eventsData
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

function openEdit(item: PowerPhase) {
  editing.value = item
  form.value = { name: item.name, voltage: item.voltage, current: item.current, frequency: item.frequency, thd: item.thd, pf: item.pf }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await powerPhasesApi.update(editing.value.id, form.value)
    } else {
      await powerPhasesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await powerPhasesApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Power Quality</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Phase Measurements</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Phase</th><th class="text-end">Voltage (V)</th><th class="text-end">Current (A)</th><th class="text-end">Freq (Hz)</th><th class="text-end">THD %</th><th class="text-end">Power Factor</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="p in phases" :key="p.id">
                <td class="fw-semibold">{{ p.name }}</td>
                <td class="text-end">{{ p.voltage.toFixed(1) }}</td>
                <td class="text-end">{{ p.current }}</td>
                <td class="text-end">{{ p.frequency.toFixed(2) }}</td>
                <td class="text-end" :class="p.thd > 5 ? 'text-danger' : ''">{{ p.thd.toFixed(1) }}%</td>
                <td class="text-end">{{ p.pf.toFixed(2) }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(p)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(p.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="row g-3">
        <div class="col-lg-5">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Harmonics (%)</div>
            <div class="card-body p-0">
              <table class="table table-sm table-hover mb-0">
                <thead class="table-light">
                  <tr><th>Harmonic</th><th class="text-end">L1</th><th class="text-end">L2</th><th class="text-end">L3</th></tr>
                </thead>
                <tbody>
                  <tr v-for="h in harmonics" :key="h.order">
                    <td class="fw-semibold">{{ h.order }}</td>
                    <td class="text-end">{{ h.l1 }}</td>
                    <td class="text-end">{{ h.l2 }}</td>
                    <td class="text-end">{{ h.l3 }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
        <div class="col-lg-7">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Recent Sag/Swell Events</div>
            <div class="card-body p-0">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr><th>Time</th><th>Type</th><th>Phase</th><th>Duration</th><th>Magnitude</th></tr>
                </thead>
                <tbody>
                  <tr v-for="e in events" :key="e.time">
                    <td>{{ e.time }}</td>
                    <td><span class="badge" :class="e.type === 'Sag' ? 'bg-warning text-dark' : e.type === 'Swell' ? 'bg-info text-dark' : 'bg-danger'">{{ e.type }}</span></td>
                    <td>{{ e.phase }}</td>
                    <td>{{ e.duration }}</td>
                    <td class="fw-semibold">{{ e.magnitude }}</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Power Phase</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Voltage (V)</label>
              <input v-model.number="form.voltage" type="number" step="0.1" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Current (A)</label>
              <input v-model.number="form.current" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Frequency (Hz)</label>
              <input v-model.number="form.frequency" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">THD (%)</label>
              <input v-model.number="form.thd" type="number" step="0.1" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Power Factor</label>
              <input v-model.number="form.pf" type="number" step="0.01" class="form-control" />
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
