<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { batteryBanksApi, type BatteryBank } from '../../services/api'

const banks = ref<BatteryBank[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<BatteryBank | null>(null)

const defaultForm = { name: '', type: 'Li-ion', cells: 0, soc: 0, soh: 0, voltage: 0, temp: 0, runtime: 0, status: 'charged' }
const form = ref({ ...defaultForm })

function socColor(soc: number): string {
  if (soc >= 80) return 'success'
  if (soc >= 50) return 'warning'
  return 'danger'
}

function sohColor(soh: number): string {
  if (soh >= 90) return 'success'
  if (soh >= 75) return 'warning'
  return 'danger'
}

async function loadData() {
  banks.value = await batteryBanksApi.getAll()
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

function openEdit(item: BatteryBank) {
  editing.value = item
  form.value = { name: item.name, type: item.type, cells: item.cells, soc: item.soc, soh: item.soh, voltage: item.voltage, temp: item.temp, runtime: item.runtime, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await batteryBanksApi.update(editing.value.id, form.value)
    } else {
      await batteryBanksApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await batteryBanksApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Battery Management</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="b in banks" :key="b.name" class="col-md-6 col-xl-3">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
              <span class="fw-semibold">{{ b.name }}</span>
              <span class="badge" :class="b.status === 'charged' ? 'bg-success' : 'bg-warning text-dark'">{{ b.status }}</span>
            </div>
            <div class="card-body">
              <div class="text-center mb-3">
                <div :class="`display-5 fw-bold text-${socColor(b.soc)}`">{{ b.soc }}%</div>
                <small class="text-muted">State of Charge</small>
              </div>
              <div class="mb-2">
                <div class="d-flex justify-content-between small mb-1">
                  <span>SOH</span>
                  <span :class="`text-${sohColor(b.soh)}`">{{ b.soh }}%</span>
                </div>
                <div class="progress" style="height: 6px;">
                  <div :class="`progress-bar bg-${sohColor(b.soh)}`" :style="{ width: b.soh + '%' }"></div>
                </div>
              </div>
              <ul class="list-unstyled small text-muted mb-0 mt-3">
                <li>Type: <strong>{{ b.type }}</strong></li>
                <li>Cells: <strong>{{ b.cells }}</strong></li>
                <li>Voltage: <strong>{{ b.voltage.toFixed(1) }}V</strong></li>
                <li>Temp: <strong>{{ b.temp }}&deg;C</strong></li>
                <li>Runtime: <strong>{{ b.runtime }} min</strong></li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <!-- Battery Banks Table with CRUD -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Battery Banks</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Name</th><th>Type</th><th class="text-end">Cells</th><th class="text-end">SOC</th><th class="text-end">SOH</th><th class="text-end">Voltage</th><th class="text-end">Temp</th><th class="text-end">Runtime</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="b in banks" :key="b.id">
                <td class="fw-semibold">{{ b.name }}</td>
                <td>{{ b.type }}</td>
                <td class="text-end">{{ b.cells }}</td>
                <td class="text-end">{{ b.soc }}%</td>
                <td class="text-end">{{ b.soh }}%</td>
                <td class="text-end">{{ b.voltage.toFixed(1) }}V</td>
                <td class="text-end">{{ b.temp }}&deg;C</td>
                <td class="text-end">{{ b.runtime }} min</td>
                <td><span class="badge" :class="b.status === 'charged' ? 'bg-success' : 'bg-warning text-dark'">{{ b.status }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(b)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(b.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Battery Bank</h5>
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
                <option>Li-ion</option>
                <option>VRLA</option>
                <option>NiCd</option>
              </select>
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Cells</label>
                <input v-model.number="form.cells" type="number" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">SOC (%)</label>
                <input v-model.number="form.soc" type="number" class="form-control" />
              </div>
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">SOH (%)</label>
                <input v-model.number="form.soh" type="number" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">Voltage (V)</label>
                <input v-model.number="form.voltage" type="number" step="0.1" class="form-control" />
              </div>
            </div>
            <div class="row mb-3">
              <div class="col">
                <label class="form-label">Temp (C)</label>
                <input v-model.number="form.temp" type="number" class="form-control" />
              </div>
              <div class="col">
                <label class="form-label">Runtime (min)</label>
                <input v-model.number="form.runtime" type="number" class="form-control" />
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>charged</option>
                <option>charging</option>
                <option>discharging</option>
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
