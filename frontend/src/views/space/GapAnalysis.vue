<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { rackGapsApi, type RackGap } from '../../services/api'

const racks = ref<RackGap[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<RackGap | null>(null)

const defaultForm = { rackId: '', totalU: 42, freeU: 0, gapStart: 1, gapSize: 1 }
const form = ref({ ...defaultForm })

const summary = {
  total1U: 8, total2U: 6, total3U: 4, total4UPlus: 9,
}

function sizeColor(size: number): string {
  if (size >= 4) return 'bg-success'
  if (size >= 2) return 'bg-info text-dark'
  return 'bg-secondary'
}

async function loadData() {
  racks.value = await rackGapsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: RackGap) {
  editing.value = item
  form.value = { rackId: item.rackId, totalU: item.totalU, freeU: item.freeU, gapStart: item.gapStart, gapSize: item.gapSize }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await rackGapsApi.update(editing.value.id, form.value)
    } else {
      await rackGapsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await rackGapsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Gap Analysis</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-secondary mb-0">{{ summary.total1U }}</h3><small class="text-muted">1U Gaps</small></div></div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-info mb-0">{{ summary.total2U }}</h3><small class="text-muted">2-3U Gaps</small></div></div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-success mb-0">{{ summary.total4UPlus }}</h3><small class="text-muted">4U+ Gaps</small></div></div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center"><div class="card-body"><h3 class="text-primary mb-0">{{ racks.reduce((a, r) => a + r.freeU, 0) }}</h3><small class="text-muted">Total Free U</small></div></div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Racks with Gaps</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Rack</th><th class="text-end">Total U</th><th class="text-end">Free U</th><th>Gaps</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="r in racks" :key="r.id">
                <td class="fw-semibold">{{ r.rackId }}</td>
                <td class="text-end">{{ r.totalU }}</td>
                <td class="text-end">{{ r.freeU }}</td>
                <td>
                  <div class="d-flex flex-wrap gap-1">
                    <span class="badge" :class="sizeColor(r.gapSize)">
                      U{{ r.gapStart }}–{{ r.gapStart + r.gapSize - 1 }} ({{ r.gapSize }}U)
                    </span>
                  </div>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(r)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(r.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="d-flex gap-3 small mt-3">
        <span><span class="badge bg-secondary">&nbsp;&nbsp;</span> 1U</span>
        <span><span class="badge bg-info">&nbsp;&nbsp;</span> 2-3U</span>
        <span><span class="badge bg-success">&nbsp;&nbsp;</span> 4U+</span>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Rack Gap</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Rack ID</label>
              <input v-model="form.rackId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Total U</label>
              <input v-model.number="form.totalU" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Free U</label>
              <input v-model.number="form.freeU" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Gap Start (U)</label>
              <input v-model.number="form.gapStart" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Gap Size (U)</label>
              <input v-model.number="form.gapSize" type="number" class="form-control" />
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
