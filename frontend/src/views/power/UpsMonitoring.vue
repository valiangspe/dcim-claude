<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { upsUnitsApi, type UpsUnit } from '../../services/api'

const units = ref<UpsUnit[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<UpsUnit | null>(null)

const defaultForm = { name: '', capacity: 0, load: 0, loadPct: 0, input: 0, output: 0, battery: 0, runtime: 0, mode: 'online', status: 'normal' }
const form = ref({ ...defaultForm })

function loadColor(pct: number): string {
  if (pct >= 80) return 'danger'
  if (pct >= 60) return 'warning'
  return 'success'
}

async function loadData() {
  units.value = await upsUnitsApi.getAll()
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

function openEdit(item: UpsUnit) {
  editing.value = item
  form.value = { name: item.name, capacity: item.capacity, load: item.load, loadPct: item.loadPct, input: item.input, output: item.output, battery: item.battery, runtime: item.runtime, mode: item.mode, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await upsUnitsApi.update(editing.value.id, form.value)
    } else {
      await upsUnitsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await upsUnitsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">UPS Monitoring</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="u in units" :key="u.name" class="col-md-6 col-xl-4">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
              <span class="fw-semibold">{{ u.name }}</span>
              <div class="d-flex gap-1">
                <span class="badge" :class="u.mode === 'online' ? 'bg-primary' : 'bg-secondary'">{{ u.mode }}</span>
                <span class="badge" :class="u.status === 'normal' ? 'bg-success' : 'bg-warning text-dark'">{{ u.status }}</span>
              </div>
            </div>
            <div class="card-body">
              <div class="text-center mb-3">
                <div :class="`display-6 fw-bold text-${loadColor(u.loadPct)}`">{{ u.loadPct }}%</div>
                <small class="text-muted">{{ u.load }} / {{ u.capacity }} kW</small>
              </div>
              <div class="progress mb-3" style="height: 8px;">
                <div :class="`progress-bar bg-${loadColor(u.loadPct)}`" :style="{ width: u.loadPct + '%' }"></div>
              </div>
              <div class="row small text-muted">
                <div class="col-6">Input: <strong>{{ u.input }}V</strong></div>
                <div class="col-6">Output: <strong>{{ u.output > 0 ? u.output + 'V' : '---' }}</strong></div>
                <div class="col-6">Battery: <strong>{{ u.battery }}%</strong></div>
                <div class="col-6">Runtime: <strong>{{ u.runtime > 0 ? u.runtime + ' min' : '---' }}</strong></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- UPS Table with CRUD -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>UPS Units</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Name</th><th class="text-end">Capacity</th><th class="text-end">Load</th><th class="text-end">Load %</th><th class="text-end">Input</th><th class="text-end">Output</th><th class="text-end">Battery</th><th>Mode</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="u in units" :key="u.id">
                <td class="fw-semibold">{{ u.name }}</td>
                <td class="text-end">{{ u.capacity }} kW</td>
                <td class="text-end">{{ u.load }} kW</td>
                <td class="text-end">{{ u.loadPct }}%</td>
                <td class="text-end">{{ u.input }}V</td>
                <td class="text-end">{{ u.output > 0 ? u.output + 'V' : '---' }}</td>
                <td class="text-end">{{ u.battery }}%</td>
                <td><span class="badge" :class="u.mode === 'online' ? 'bg-primary' : 'bg-secondary'">{{ u.mode }}</span></td>
                <td><span class="badge" :class="u.status === 'normal' ? 'bg-success' : 'bg-warning text-dark'">{{ u.status }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(u)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(u.id)">Delete</button>
                </td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} UPS Unit</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Capacity (kW)</label>
                <input v-model.number="form.capacity" type="number" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">Load (kW)</label>
                <input v-model.number="form.load" type="number" class="form-control" />
              </div>
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Load %</label>
                <input v-model.number="form.loadPct" type="number" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">Input (V)</label>
                <input v-model.number="form.input" type="number" class="form-control" />
              </div>
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Output (V)</label>
                <input v-model.number="form.output" type="number" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">Battery (%)</label>
                <input v-model.number="form.battery" type="number" class="form-control" />
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Runtime (min)</label>
              <input v-model.number="form.runtime" type="number" class="form-control" />
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Mode</label>
                <select v-model="form.mode" class="form-select">
                  <option>online</option>
                  <option>battery</option>
                  <option>bypass</option>
                  <option>offline</option>
                </select>
              </div>
              <div class="col">
                <label class="form-label">Status</label>
                <select v-model="form.status" class="form-select">
                  <option>normal</option>
                  <option>warning</option>
                  <option>critical</option>
                  <option>offline</option>
                </select>
              </div>
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
