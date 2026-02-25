<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { racksApi, type Rack } from '../../services/api'

const racks = ref<Rack[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Rack | null>(null)

const defaultForm = { rackId: '', hall: '', totalU: 42, usedU: 0, power: 0, weight: 0 }
const form = ref({ ...defaultForm })

function utilColor(pct: number): string {
  if (pct >= 95) return 'danger'
  if (pct >= 80) return 'warning'
  return 'success'
}

async function loadData() {
  racks.value = await racksApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: Rack) {
  editing.value = item
  form.value = { rackId: item.rackId, hall: item.hall, totalU: item.totalU, usedU: item.usedU, power: item.power, weight: item.weight }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await racksApi.update(editing.value.id, form.value)
    } else {
      await racksApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await racksApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Rack Utilization</h4>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Rack</button>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="rack in racks" :key="rack.id" class="col-sm-6 col-md-4 col-xl-3">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span class="fw-semibold">{{ rack.rackId }}</span>
                <small class="text-muted">{{ rack.hall }}</small>
              </div>
              <div class="text-center mb-2">
                <span :class="`display-6 fw-bold text-${utilColor(rack.usedU / rack.totalU * 100)}`">{{ (rack.usedU / rack.totalU * 100).toFixed(0) }}%</span>
              </div>
              <div class="progress mb-2" style="height: 12px;">
                <div :class="`progress-bar bg-${utilColor(rack.usedU / rack.totalU * 100)}`" :style="{ width: (rack.usedU / rack.totalU * 100) + '%' }"></div>
              </div>
              <div class="small text-muted d-flex justify-content-between">
                <span>{{ rack.usedU }}/{{ rack.totalU }}U</span>
                <span>{{ rack.power }} kW</span>
                <span>{{ rack.weight }} kg</span>
              </div>
              <div class="mt-2 text-center">
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(rack)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(rack.id)">Delete</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Rack</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Rack ID</label>
              <input v-model="form.rackId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Hall</label>
              <input v-model="form.hall" type="text" class="form-control" />
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
              <label class="form-label">Power (kW)</label>
              <input v-model.number="form.power" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Weight (kg)</label>
              <input v-model.number="form.weight" type="number" class="form-control" />
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
