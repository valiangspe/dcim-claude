<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { coolingUnitsApi, type CoolingUnit } from '../../services/api'

const loading = ref(true)
const units = ref<CoolingUnit[]>([])
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CoolingUnit | null>(null)

const defaultForm = { name: '', type: 'CRAH', capacity: 0, load: 0, supplyTemp: 0, returnTemp: 0, fanSpeed: 0, status: 'running' }
const form = ref({ ...defaultForm })

async function loadData() {
  units.value = await coolingUnitsApi.getAll()
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

function openEdit(unit: CoolingUnit) {
  editing.value = unit
  form.value = { name: unit.name, type: unit.type, capacity: unit.capacity, load: unit.load, supplyTemp: unit.supplyTemp, returnTemp: unit.returnTemp, fanSpeed: unit.fanSpeed, status: unit.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await coolingUnitsApi.update(editing.value.id, form.value)
    } else {
      await coolingUnitsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await coolingUnitsApi.remove(id)
  await loadData()
}

const kpis = computed(() => {
  const totalCapacity = units.value.reduce((a, u) => a + u.capacity, 0)
  const currentLoad = units.value.reduce((a, u) => a + u.load, 0)
  const utilization = totalCapacity > 0 ? (currentLoad / totalCapacity * 100) : 0
  const running = units.value.filter(u => u.status === 'running')
  const avgSupply = running.length > 0 ? running.reduce((a, u) => a + u.supplyTemp, 0) / running.length : 0
  const avgReturn = running.length > 0 ? running.reduce((a, u) => a + u.returnTemp, 0) / running.length : 0
  return [
    { label: 'Total Capacity', value: totalCapacity.toLocaleString(), unit: 'kW', color: 'primary' },
    { label: 'Current Load', value: currentLoad.toLocaleString(), unit: 'kW', color: 'info' },
    { label: 'Utilization', value: utilization.toFixed(0), unit: '%', color: 'warning' },
    { label: 'Avg Supply Temp', value: avgSupply.toFixed(1), unit: '°C', color: 'success' },
    { label: 'Avg Return Temp', value: avgReturn.toFixed(1), unit: '°C', color: 'danger' },
    { label: 'Delta-T', value: (avgReturn - avgSupply).toFixed(1), unit: '°C', color: 'dark' },
  ]
})

function loadColor(pct: number): string {
  if (pct >= 85) return 'danger'
  if (pct >= 70) return 'warning'
  return 'success'
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cooling Overview</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="k in kpis" :key="k.label" class="col-sm-6 col-lg-4 col-xl-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body text-center">
              <h3 :class="`text-${k.color} mb-0`">{{ k.value }}<small class="text-muted fs-6">{{ k.unit }}</small></h3>
              <small class="text-muted">{{ k.label }}</small>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Cooling Units</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Unit</button>
        </div>
        <div class="card-body p-0">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr>
                  <th>Unit</th><th>Type</th><th class="text-end">Capacity (kW)</th><th class="text-end">Load (kW)</th>
                  <th style="width:15%">Utilization</th><th class="text-end">Supply (°C)</th><th class="text-end">Return (°C)</th>
                  <th class="text-end">Fan %</th><th>Status</th><th>Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="u in units" :key="u.id">
                  <td class="fw-semibold">{{ u.name }}</td>
                  <td><span class="badge bg-secondary">{{ u.type }}</span></td>
                  <td class="text-end">{{ u.capacity }}</td>
                  <td class="text-end">{{ u.load > 0 ? u.load : '---' }}</td>
                  <td>
                    <div v-if="u.load > 0" class="d-flex align-items-center gap-2">
                      <div class="progress flex-fill" style="height: 8px;">
                        <div :class="`progress-bar bg-${loadColor(u.load / u.capacity * 100)}`" :style="{ width: (u.load / u.capacity * 100) + '%' }"></div>
                      </div>
                      <small>{{ (u.load / u.capacity * 100).toFixed(0) }}%</small>
                    </div>
                    <span v-else class="text-muted">---</span>
                  </td>
                  <td class="text-end">{{ u.supplyTemp > 0 ? u.supplyTemp.toFixed(1) : '---' }}</td>
                  <td class="text-end">{{ u.returnTemp > 0 ? u.returnTemp.toFixed(1) : '---' }}</td>
                  <td class="text-end">{{ u.fanSpeed > 0 ? u.fanSpeed + '%' : '---' }}</td>
                  <td><span class="badge" :class="u.status === 'running' ? 'bg-success' : 'bg-secondary'">{{ u.status }}</span></td>
                  <td>
                    <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(u)">Edit</button>
                    <button class="btn btn-sm btn-outline-danger" @click="remove(u.id)">Delete</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Unit</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <select v-model="form.type" class="form-select">
                <option>CRAH</option>
                <option>CRAC</option>
              </select>
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
              <label class="form-label">Supply Temp (°C)</label>
              <input v-model.number="form.supplyTemp" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Return Temp (°C)</label>
              <input v-model.number="form.returnTemp" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Fan Speed %</label>
              <input v-model.number="form.fanSpeed" type="number" class="form-control" />
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
