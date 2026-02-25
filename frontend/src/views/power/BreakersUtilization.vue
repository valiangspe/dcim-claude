<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { breakerRecordsApi, type BreakerRecord } from '../../services/api'

const breakers = ref<BreakerRecord[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<BreakerRecord | null>(null)

const defaultForm = { breakerId: '', pdu: '', rating: 0, load: 0, circuits: 0, trip: false }
const form = ref({ ...defaultForm })

function utilColor(pct: number): string {
  if (pct >= 90) return 'danger'
  if (pct >= 75) return 'warning'
  return 'success'
}

async function loadData() {
  breakers.value = await breakerRecordsApi.getAll()
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

function openEdit(item: BreakerRecord) {
  editing.value = item
  form.value = { breakerId: item.breakerId, pdu: item.pdu, rating: item.rating, load: item.load, circuits: item.circuits, trip: item.trip }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await breakerRecordsApi.update(editing.value.id, form.value)
    } else {
      await breakerRecordsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await breakerRecordsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Breakers Utilization</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Breakers</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Breaker</th>
                <th>PDU</th>
                <th class="text-end">Rating (A)</th>
                <th class="text-end">Load (A)</th>
                <th class="text-end">Circuits</th>
                <th style="width:18%">Utilization</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="b in breakers" :key="b.id" :class="b.trip ? 'table-danger' : ''">
                <td class="fw-semibold font-monospace">{{ b.breakerId }}</td>
                <td>{{ b.pdu }}</td>
                <td class="text-end">{{ b.rating }}</td>
                <td class="text-end">{{ b.load.toFixed(1) }}</td>
                <td class="text-end">{{ b.circuits }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 8px;">
                      <div :class="`progress-bar bg-${utilColor(b.load / b.rating * 100)}`" :style="{ width: (b.load / b.rating * 100) + '%' }"></div>
                    </div>
                    <small>{{ (b.load / b.rating * 100).toFixed(0) }}%</small>
                  </div>
                </td>
                <td>
                  <span v-if="b.trip" class="badge bg-danger">TRIPPED</span>
                  <span v-else class="badge bg-success">OK</span>
                </td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Breaker</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Breaker ID</label>
              <input v-model="form.breakerId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">PDU</label>
              <input v-model="form.pdu" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Rating (A)</label>
              <input v-model.number="form.rating" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Load (A)</label>
              <input v-model.number="form.load" type="number" step="0.1" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Circuits</label>
              <input v-model.number="form.circuits" type="number" class="form-control" />
            </div>
            <div class="mb-3 form-check">
              <input v-model="form.trip" type="checkbox" class="form-check-input" id="tripCheck" />
              <label class="form-check-label" for="tripCheck">Tripped</label>
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
