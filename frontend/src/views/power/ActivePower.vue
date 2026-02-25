<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { cabinetsApi, type Cabinet } from '../../services/api'

const cabinets = ref<Cabinet[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Cabinet | null>(null)

const defaultForm = { cabinetId: '', location: '', power: 0, capacity: 0, phase: '3-phase' }
const form = ref({ ...defaultForm })

function utilColor(pct: number): string {
  if (pct >= 85) return 'danger'
  if (pct >= 70) return 'warning'
  return 'success'
}

async function loadData() {
  cabinets.value = await cabinetsApi.getAll()
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

function openEdit(item: Cabinet) {
  editing.value = item
  form.value = { cabinetId: item.cabinetId, location: item.location, power: item.power, capacity: item.capacity, phase: item.phase }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await cabinetsApi.update(editing.value.id, form.value)
    } else {
      await cabinetsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await cabinetsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Active Power</h4>
    <p class="text-muted">Actual active power per sub-location / cabinet.</p>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Cabinets</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Cabinet</th>
                <th>Location</th>
                <th>Phase</th>
                <th class="text-end">Active (kW)</th>
                <th class="text-end">Capacity (kW)</th>
                <th style="width:20%">Utilization</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="c in cabinets" :key="c.id">
                <td class="fw-semibold">{{ c.cabinetId }}</td>
                <td class="text-muted">{{ c.location }}</td>
                <td><span class="badge bg-secondary">{{ c.phase }}</span></td>
                <td class="text-end">{{ c.power.toFixed(1) }}</td>
                <td class="text-end">{{ c.capacity }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 8px;">
                      <div :class="`progress-bar bg-${utilColor(c.power / c.capacity * 100)}`" :style="{ width: (c.power / c.capacity * 100) + '%' }"></div>
                    </div>
                    <small>{{ (c.power / c.capacity * 100).toFixed(0) }}%</small>
                  </div>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(c)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(c.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Cabinet</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Cabinet ID</label>
              <input v-model="form.cabinetId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Location</label>
              <input v-model="form.location" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Power (kW)</label>
              <input v-model.number="form.power" type="number" step="0.1" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Capacity (kW)</label>
              <input v-model.number="form.capacity" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Phase</label>
              <select v-model="form.phase" class="form-select">
                <option>3-phase</option>
                <option>1-phase</option>
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
