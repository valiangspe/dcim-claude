<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { isoClassRatingsApi, type IsoClassRating } from '../../services/api'

const ratings = ref<IsoClassRating[]>([])
const loading = ref(true)
const error = ref('')
const showModal = ref(false)
const editing = ref<IsoClassRating | null>(null)
const saving = ref(false)
const form = ref({ site: '', system: '', classRating: 1, justification: '', assessedBy: '', assessedDate: '', status: 'active' })

const classDescriptions: Record<number, { label: string; availability: string; color: string; description: string }> = {
  1: { label: 'Class 1', availability: '≥ 99.671%', color: 'secondary', description: 'Basic — single path, no redundancy' },
  2: { label: 'Class 2', availability: '≥ 99.741%', color: 'info', description: 'Redundant components — scheduled maintenance possible' },
  3: { label: 'Class 3', availability: '≥ 99.982%', color: 'warning', description: 'Concurrently maintainable — dual paths, all active' },
  4: { label: 'Class 4', availability: '≥ 99.995%', color: 'success', description: 'Fault tolerant — continuous operation during any failure' },
}

const kpis = computed(() => {
  const counts = [0, 0, 0, 0]
  ratings.value.forEach(r => { counts[r.classRating - 1]++ })
  return [
    { label: 'Class 4 Systems', value: String(counts[3]), color: 'success', trend: 'Fault tolerant' },
    { label: 'Class 3 Systems', value: String(counts[2]), color: 'warning', trend: 'Concurrently maintainable' },
    { label: 'Class 2 Systems', value: String(counts[1]), color: 'info', trend: 'Redundant components' },
    { label: 'Class 1 Systems', value: String(counts[0]), color: 'secondary', trend: 'Basic infrastructure' },
    { label: 'Total Assessed', value: String(ratings.value.length), color: 'primary', trend: 'ISO/IEC 22237' },
    { label: 'Avg Class', value: ratings.value.length ? (ratings.value.reduce((s, r) => s + r.classRating, 0) / ratings.value.length).toFixed(1) : '—', color: 'dark', trend: 'Weighted average' },
  ]
})

async function loadData() {
  ratings.value = await isoClassRatingsApi.getAll()
}

onMounted(async () => {
  try { await loadData() }
  catch (e: unknown) { error.value = e instanceof Error ? e.message : 'Failed to load ratings' }
  finally { loading.value = false }
})

function openCreate() {
  editing.value = null
  form.value = { site: '', system: '', classRating: 1, justification: '', assessedBy: '', assessedDate: '', status: 'active' }
  showModal.value = true
}

function openEdit(r: IsoClassRating) {
  editing.value = r
  form.value = { site: r.site, system: r.system, classRating: r.classRating, justification: r.justification, assessedBy: r.assessedBy, assessedDate: r.assessedDate, status: r.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await isoClassRatingsApi.update(editing.value.id, form.value)
    else await isoClassRatingsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(id: number) {
  if (!confirm('Delete this rating?')) return
  await isoClassRatingsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-1">ISO Class 1–4 Availability System</h4>
    <p class="text-muted small mb-4">ISO/IEC 22237 — Availability classification for data centre systems and infrastructure</p>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>
    <template v-else>

      <!-- Class Reference Cards -->
      <div class="row g-3 mb-4">
        <div v-for="(desc, cls) in classDescriptions" :key="cls" class="col-sm-6 col-xl-3">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <div class="d-flex align-items-center mb-2">
                <span class="badge me-2" :class="`bg-${desc.color}`">{{ desc.label }}</span>
                <span class="fw-semibold small">{{ desc.availability }}</span>
              </div>
              <p class="text-muted small mb-0">{{ desc.description }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- KPI Cards -->
      <div class="row g-3 mb-4">
        <div v-for="kpi in kpis" :key="kpi.label" class="col-sm-6 col-lg-4 col-xl-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <h3 :class="`text-${kpi.color} mb-1`">{{ kpi.value }}</h3>
              <small class="text-muted d-block">{{ kpi.label }}</small>
              <small class="text-secondary">{{ kpi.trend }}</small>
            </div>
          </div>
        </div>
      </div>

      <!-- Ratings Table -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
          <span>System Availability Ratings</span>
          <button class="btn btn-sm btn-primary" @click="openCreate">Add Rating</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Site</th>
                <th>System / Subsystem</th>
                <th>Class Rating</th>
                <th>Availability</th>
                <th>Assessed By</th>
                <th>Date</th>
                <th>Status</th>
                <th style="width:80px">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in ratings" :key="r.id">
                <td class="fw-semibold">{{ r.site }}</td>
                <td>{{ r.system }}</td>
                <td>
                  <span class="badge" :class="`bg-${classDescriptions[r.classRating]?.color}`">
                    Class {{ r.classRating }}
                  </span>
                </td>
                <td class="text-muted small">{{ classDescriptions[r.classRating]?.availability }}</td>
                <td>{{ r.assessedBy }}</td>
                <td class="text-muted">{{ r.assessedDate }}</td>
                <td><span class="badge" :class="r.status === 'active' ? 'bg-success' : 'bg-secondary'">{{ r.status }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(r)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(r.id)">Delete</button>
                </td>
              </tr>
              <tr v-if="ratings.length === 0">
                <td colspan="8" class="text-center text-muted py-4">No ratings recorded yet. Add a system rating to begin ISO 22237 classification.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Class Rating</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Site</label>
              <input v-model="form.site" type="text" class="form-control" placeholder="e.g. DC-EAST-01" />
            </div>
            <div class="mb-3">
              <label class="form-label">System / Subsystem</label>
              <input v-model="form.system" type="text" class="form-control" placeholder="e.g. Power Distribution, Cooling Plant" />
            </div>
            <div class="mb-3">
              <label class="form-label">ISO Class Rating</label>
              <select v-model.number="form.classRating" class="form-select">
                <option :value="1">Class 1 — Basic (≥ 99.671%)</option>
                <option :value="2">Class 2 — Redundant (≥ 99.741%)</option>
                <option :value="3">Class 3 — Concurrently Maintainable (≥ 99.982%)</option>
                <option :value="4">Class 4 — Fault Tolerant (≥ 99.995%)</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Justification</label>
              <textarea v-model="form.justification" class="form-control" rows="3" placeholder="Evidence and rationale for this classification"></textarea>
            </div>
            <div class="mb-3">
              <label class="form-label">Assessed By</label>
              <input v-model="form.assessedBy" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Assessment Date</label>
              <input v-model="form.assessedDate" type="date" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="active">Active</option>
                <option value="review">Under Review</option>
                <option value="archived">Archived</option>
              </select>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
