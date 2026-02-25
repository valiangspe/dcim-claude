<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { rectifiersApi, type Rectifier, dcBusesApi, type DcBus } from '../../services/api'

const rectifiers = ref<Rectifier[]>([])
const buses = ref<DcBus[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Rectifier | null>(null)

const defaultForm = { rectifierId: '', voltage: 0, current: 0, power: 0, efficiency: 0, status: 'active' }
const form = ref({ ...defaultForm })

async function loadData() {
  const [rectData, busData] = await Promise.all([
    rectifiersApi.getAll(),
    dcBusesApi.getAll(),
  ])
  rectifiers.value = rectData
  buses.value = busData
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

function openEdit(item: Rectifier) {
  editing.value = item
  form.value = { rectifierId: item.rectifierId, voltage: item.voltage, current: item.current, power: item.power, efficiency: item.efficiency, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await rectifiersApi.update(editing.value.id, form.value)
    } else {
      await rectifiersApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await rectifiersApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">DC Power Control</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Rectifiers</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>ID</th><th class="text-end">Voltage (V)</th><th class="text-end">Current (A)</th><th class="text-end">Power (kW)</th><th class="text-end">Efficiency</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="r in rectifiers" :key="r.id">
                <td class="fw-semibold">{{ r.rectifierId }}</td>
                <td class="text-end">{{ r.voltage > 0 ? r.voltage.toFixed(1) : '---' }}</td>
                <td class="text-end">{{ r.current > 0 ? r.current : '---' }}</td>
                <td class="text-end">{{ r.power > 0 ? r.power.toFixed(2) : '---' }}</td>
                <td class="text-end">{{ r.efficiency > 0 ? r.efficiency.toFixed(1) + '%' : '---' }}</td>
                <td><span class="badge" :class="r.status === 'active' ? 'bg-success' : 'bg-secondary'">{{ r.status }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(r)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(r.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">DC Buses</div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Bus</th><th class="text-end">Voltage (V)</th><th class="text-end">Current (A)</th><th class="text-end">Power (kW)</th><th class="text-end">Temp (&deg;C)</th></tr>
            </thead>
            <tbody>
              <tr v-for="b in buses" :key="b.name">
                <td class="fw-semibold">{{ b.name }}</td>
                <td class="text-end">{{ b.voltage.toFixed(1) }}</td>
                <td class="text-end">{{ b.current }}</td>
                <td class="text-end">{{ b.power.toFixed(2) }}</td>
                <td class="text-end">{{ b.temp }}&deg;C</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Rectifier</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Rectifier ID</label>
              <input v-model="form.rectifierId" type="text" class="form-control" />
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
              <label class="form-label">Power (kW)</label>
              <input v-model.number="form.power" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Efficiency (%)</label>
              <input v-model.number="form.efficiency" type="number" step="0.1" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>active</option>
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
