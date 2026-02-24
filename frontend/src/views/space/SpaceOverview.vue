<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { hallsApi, type Hall } from '../../services/api'

const loading = ref(true)
const halls = ref<Hall[]>([])

const showModal = ref(false)
const editing = ref<Hall | null>(null)
const saving = ref(false)
const deleting = ref<number | null>(null)
const form = ref({ name: '', sqft: 0, racks: 0, occupied: 0, totalU: 0, usedU: 0, power: 0 })

async function loadData() {
  halls.value = await hallsApi.getAll()
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
  form.value = { name: '', sqft: 0, racks: 0, occupied: 0, totalU: 0, usedU: 0, power: 0 }
  showModal.value = true
}

function openEdit(h: Hall) {
  editing.value = h
  form.value = { name: h.name, sqft: h.sqft, racks: h.racks, occupied: h.occupied, totalU: h.totalU, usedU: h.usedU, power: h.power }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await hallsApi.update(editing.value.id, form.value)
    else await hallsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(h: Hall) {
  if (!confirm(`Delete hall "${h.name}"?`)) return
  deleting.value = h.id
  try {
    await hallsApi.remove(h.id)
    await loadData()
  } finally {
    deleting.value = null
  }
}

const kpis = computed(() => {
  const totalRacks = halls.value.reduce((a, h) => a + h.racks, 0)
  const occupiedRacks = halls.value.reduce((a, h) => a + h.occupied, 0)
  const rackUtil = totalRacks > 0 ? (occupiedRacks / totalRacks * 100) : 0
  const totalSqft = halls.value.reduce((a, h) => a + h.sqft, 0)
  const totalU = halls.value.reduce((a, h) => a + h.totalU, 0)
  const usedU = halls.value.reduce((a, h) => a + h.usedU, 0)
  const availSqft = totalSqft > 0 ? totalSqft - Math.round(totalSqft * rackUtil / 100) : 0
  return [
    { label: 'Total Racks', value: totalRacks.toLocaleString(), color: 'primary' },
    { label: 'Occupied Racks', value: occupiedRacks.toLocaleString(), color: 'info' },
    { label: 'Rack Utilization', value: rackUtil.toFixed(1) + '%', color: 'warning' },
    { label: 'Total Floor Space', value: totalSqft.toLocaleString() + ' sqft', color: 'dark' },
    { label: 'Available Space', value: availSqft.toLocaleString() + ' sqft', color: 'success' },
    { label: 'Total U Slots', value: totalU.toLocaleString(), color: 'secondary' },
  ]
})
</script>

<template>
  <div>
    <h4 class="mb-4">Space Overview</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="k in kpis" :key="k.label" class="col-sm-6 col-lg-4 col-xl-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body text-center">
              <h3 :class="`text-${k.color} mb-0`">{{ k.value }}</h3>
              <small class="text-muted">{{ k.label }}</small>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
          <span>Per-Hall Breakdown</span>
          <button class="btn btn-sm btn-primary" @click="openCreate">Add Hall</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Hall</th><th class="text-end">Floor (sqft)</th><th class="text-end">Racks</th>
                <th class="text-end">Occupied</th><th style="width:14%">Rack Util</th>
                <th class="text-end">Total U</th><th class="text-end">Used U</th><th style="width:14%">U Util</th>
                <th class="text-end">Power (kW)</th>
                <th style="width:120px">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="h in halls" :key="h.id">
                <td class="fw-semibold">{{ h.name }}</td>
                <td class="text-end">{{ h.sqft.toLocaleString() }}</td>
                <td class="text-end">{{ h.racks }}</td>
                <td class="text-end">{{ h.occupied }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 8px;">
                      <div class="progress-bar bg-primary" :style="{ width: (h.occupied / h.racks * 100) + '%' }"></div>
                    </div>
                    <small>{{ (h.occupied / h.racks * 100).toFixed(0) }}%</small>
                  </div>
                </td>
                <td class="text-end">{{ h.totalU.toLocaleString() }}</td>
                <td class="text-end">{{ h.usedU.toLocaleString() }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 8px;">
                      <div class="progress-bar bg-info" :style="{ width: (h.usedU / h.totalU * 100) + '%' }"></div>
                    </div>
                    <small>{{ (h.usedU / h.totalU * 100).toFixed(0) }}%</small>
                  </div>
                </td>
                <td class="text-end">{{ h.power.toLocaleString() }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(h)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(h)" :disabled="deleting === h.id">
                    <span v-if="deleting === h.id" class="spinner-border spinner-border-sm"></span>
                    <span v-else>Delete</span>
                  </button>
                </td>
              </tr>
            </tbody>
            <tfoot class="table-light">
              <tr class="fw-semibold">
                <td>Total</td>
                <td class="text-end">{{ halls.reduce((a, h) => a + h.sqft, 0).toLocaleString() }}</td>
                <td class="text-end">{{ halls.reduce((a, h) => a + h.racks, 0) }}</td>
                <td class="text-end">{{ halls.reduce((a, h) => a + h.occupied, 0) }}</td>
                <td></td>
                <td class="text-end">{{ halls.reduce((a, h) => a + h.totalU, 0).toLocaleString() }}</td>
                <td class="text-end">{{ halls.reduce((a, h) => a + h.usedU, 0).toLocaleString() }}</td>
                <td></td>
                <td class="text-end">{{ halls.reduce((a, h) => a + h.power, 0).toLocaleString() }}</td>
                <td></td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Hall</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Sqft</label>
              <input v-model.number="form.sqft" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Racks</label>
              <input v-model.number="form.racks" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Occupied</label>
              <input v-model.number="form.occupied" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Total U</label>
              <input v-model.number="form.totalU" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Used U</label>
              <input v-model.number="form.usedU" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Power in kW</label>
              <input v-model.number="form.power" type="number" class="form-control" />
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
