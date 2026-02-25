<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { coolingCapacitiesApi, type CoolingCapacity } from '../../services/api'

const halls = ref<CoolingCapacity[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CoolingCapacity | null>(null)

const defaultForm = { name: '', current: 0, max: 0, planned: 0, available: 0, growth: 0 }
const form = ref({ ...defaultForm })

const forecast = [
  { quarter: 'Q2 2026', projectedLoad: 2580, capacity: 3200, headroom: 620 },
  { quarter: 'Q3 2026', projectedLoad: 2720, capacity: 3200, headroom: 480 },
  { quarter: 'Q4 2026', projectedLoad: 2850, capacity: 3200, headroom: 350 },
  { quarter: 'Q1 2027', projectedLoad: 2990, capacity: 3600, headroom: 610 },
]

async function loadData() {
  halls.value = await coolingCapacitiesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: CoolingCapacity) {
  editing.value = item
  form.value = { name: item.name, current: item.current, max: item.max, planned: item.planned, available: item.available, growth: item.growth }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await coolingCapacitiesApi.update(editing.value.id, form.value)
    } else {
      await coolingCapacitiesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await coolingCapacitiesApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cooling Capacity Planning</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
            <div class="card-body"><h3 class="text-primary mb-0">3,200 kW</h3><small class="text-muted">Total Capacity</small></div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
            <div class="card-body"><h3 class="text-warning mb-0">2,432 kW</h3><small class="text-muted">Current Load</small></div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-success bg-opacity-10">
            <div class="card-body"><h3 class="text-success mb-0">768 kW</h3><small class="text-muted">Available Headroom</small></div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Per-Hall Capacity</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Hall</th><th class="text-end">Current (kW)</th><th class="text-end">Max (kW)</th><th class="text-end">Planned (kW)</th><th class="text-end">Available (kW)</th><th class="text-end">Growth %/yr</th><th style="width:15%">Utilization</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="h in halls" :key="h.id">
                <td class="fw-semibold">{{ h.name }}</td>
                <td class="text-end">{{ h.current.toLocaleString() }}</td>
                <td class="text-end">{{ h.max.toLocaleString() }}</td>
                <td class="text-end">{{ h.planned }}</td>
                <td class="text-end text-success fw-semibold">{{ h.available }}</td>
                <td class="text-end">{{ h.growth }}%</td>
                <td>
                  <div class="progress" style="height: 8px;">
                    <div class="progress-bar bg-primary" :style="{ width: (h.current / h.max * 100) + '%' }"></div>
                    <div class="progress-bar bg-warning" :style="{ width: (h.planned / h.max * 100) + '%' }"></div>
                  </div>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(h)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(h.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">Quarterly Forecast</div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Quarter</th><th class="text-end">Projected Load (kW)</th><th class="text-end">Capacity (kW)</th><th class="text-end">Headroom (kW)</th></tr>
            </thead>
            <tbody>
              <tr v-for="f in forecast" :key="f.quarter">
                <td class="fw-semibold">{{ f.quarter }}</td>
                <td class="text-end">{{ f.projectedLoad.toLocaleString() }}</td>
                <td class="text-end">{{ f.capacity.toLocaleString() }}</td>
                <td class="text-end" :class="f.headroom < 400 ? 'text-danger fw-semibold' : 'text-success'">{{ f.headroom }}</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Capacity</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Current (kW)</label>
              <input v-model.number="form.current" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Max (kW)</label>
              <input v-model.number="form.max" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Planned (kW)</label>
              <input v-model.number="form.planned" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Available (kW)</label>
              <input v-model.number="form.available" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Growth (%/yr)</label>
              <input v-model.number="form.growth" type="number" class="form-control" />
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
