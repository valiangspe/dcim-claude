<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { supplyReturnUnitsApi, type SupplyReturnUnit } from '../../services/api'

const units = ref<SupplyReturnUnit[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<SupplyReturnUnit | null>(null)

const defaultForm = { name: '', supplyTemp: 0, returnTemp: 0, supplyFlow: 0, returnFlow: 0, deltaT: 0, status: 'normal' }
const form = ref({ ...defaultForm })

function deltaTColor(dt: number): string {
  if (dt > 15) return 'text-danger fw-semibold'
  if (dt > 14) return 'text-warning'
  return ''
}

async function loadData() {
  units.value = await supplyReturnUnitsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: SupplyReturnUnit) {
  editing.value = item
  form.value = { name: item.name, supplyTemp: item.supplyTemp, returnTemp: item.returnTemp, supplyFlow: item.supplyFlow, returnFlow: item.returnFlow, deltaT: item.deltaT, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await supplyReturnUnitsApi.update(editing.value.id, form.value)
    } else {
      await supplyReturnUnitsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await supplyReturnUnitsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Supply & Return Air</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Supply & Return Units</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr>
                  <th>Unit</th>
                  <th class="text-end">Supply (°C)</th><th class="text-end">Return (°C)</th>
                  <th class="text-end">Delta-T (°C)</th>
                  <th class="text-end">Supply CFM</th><th class="text-end">Return CFM</th>
                  <th>Status</th><th>Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="u in units" :key="u.id">
                  <td class="fw-semibold">{{ u.name }}</td>
                  <td class="text-end text-info">{{ u.supplyTemp.toFixed(1) }}</td>
                  <td class="text-end text-danger">{{ u.returnTemp.toFixed(1) }}</td>
                  <td class="text-end" :class="deltaTColor(u.deltaT)">{{ u.deltaT.toFixed(1) }}</td>
                  <td class="text-end">{{ u.supplyFlow.toLocaleString() }}</td>
                  <td class="text-end">{{ u.returnFlow.toLocaleString() }}</td>
                  <td>
                    <span class="badge" :class="u.status === 'normal' ? 'bg-success' : 'bg-warning text-dark'">{{ u.status }}</span>
                  </td>
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
              <label class="form-label">Supply Temp (°C)</label>
              <input v-model.number="form.supplyTemp" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Return Temp (°C)</label>
              <input v-model.number="form.returnTemp" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Supply Flow (CFM)</label>
              <input v-model.number="form.supplyFlow" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Return Flow (CFM)</label>
              <input v-model.number="form.returnFlow" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Delta-T (°C)</label>
              <input v-model.number="form.deltaT" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>normal</option>
                <option>warning</option>
                <option>critical</option>
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
