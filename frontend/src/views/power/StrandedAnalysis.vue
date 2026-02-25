<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { strandedPowersApi, type StrandedPower } from '../../services/api'

const stranded = ref<StrandedPower[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<StrandedPower | null>(null)

const defaultForm = { location: '', allocated: 0, actual: 0, stranded: 0, pct: 0 }
const form = ref({ ...defaultForm })

const totalStranded = computed(() => stranded.value.reduce((a, s) => a + s.stranded, 0))
const totalAllocated = computed(() => stranded.value.reduce((a, s) => a + s.allocated, 0))

async function loadData() {
  stranded.value = await strandedPowersApi.getAll()
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

function openEdit(item: StrandedPower) {
  editing.value = item
  form.value = { location: item.location, allocated: item.allocated, actual: item.actual, stranded: item.stranded, pct: item.pct }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await strandedPowersApi.update(editing.value.id, form.value)
    } else {
      await strandedPowersApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await strandedPowersApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Stranded Power Analysis</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-danger bg-opacity-10">
            <div class="card-body py-4">
              <h2 class="text-danger mb-0">{{ totalStranded }} kW</h2>
              <small class="text-muted">Total Stranded Power</small>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
            <div class="card-body py-4">
              <h2 class="text-warning mb-0">{{ (totalStranded / totalAllocated * 100).toFixed(1) }}%</h2>
              <small class="text-muted">Stranded Ratio</small>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
            <div class="card-body py-4">
              <h2 class="text-info mb-0">${{ (totalStranded * 0.087 * 8760 / 1000).toFixed(0) }}k</h2>
              <small class="text-muted">Est. Annual Waste</small>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Stranded Power by Location</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Location</th><th class="text-end">Allocated (kW)</th><th class="text-end">Actual (kW)</th><th class="text-end">Stranded (kW)</th><th style="width:20%">Stranded %</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="s in stranded" :key="s.id">
                <td class="fw-semibold">{{ s.location }}</td>
                <td class="text-end">{{ s.allocated }}</td>
                <td class="text-end">{{ s.actual }}</td>
                <td class="text-end text-danger fw-semibold">{{ s.stranded }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 8px;">
                      <div class="progress-bar bg-danger" :style="{ width: s.pct + '%' }"></div>
                    </div>
                    <small>{{ s.pct.toFixed(1) }}%</small>
                  </div>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(s)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(s.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Stranded Power</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Location</label>
              <input v-model="form.location" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Allocated (kW)</label>
              <input v-model.number="form.allocated" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Actual (kW)</label>
              <input v-model.number="form.actual" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Stranded (kW)</label>
              <input v-model.number="form.stranded" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Pct (%)</label>
              <input v-model.number="form.pct" type="number" step="0.1" class="form-control" />
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
