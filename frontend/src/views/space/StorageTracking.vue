<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { storageSystemsApi, type StorageSystem } from '../../services/api'

const systems = ref<StorageSystem[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<StorageSystem | null>(null)

const defaultForm = { name: '', type: '', totalTB: 0, usedTB: 0, iops: 0, throughput: '', status: 'online' }
const form = ref({ ...defaultForm })

const totalCap = computed(() => systems.value.reduce((a, s) => a + s.totalTB, 0))
const totalUsed = computed(() => systems.value.reduce((a, s) => a + s.usedTB, 0))

function utilColor(pct: number): string {
  if (pct >= 85) return 'danger'
  if (pct >= 70) return 'warning'
  return 'success'
}

async function loadData() {
  systems.value = await storageSystemsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: StorageSystem) {
  editing.value = item
  form.value = { name: item.name, type: item.type, totalTB: item.totalTB, usedTB: item.usedTB, iops: item.iops, throughput: item.throughput, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await storageSystemsApi.update(editing.value.id, form.value)
    } else {
      await storageSystemsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await storageSystemsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Storage Tracking</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
            <div class="card-body"><h3 class="text-primary mb-0">{{ totalCap }} TB</h3><small class="text-muted">Total Capacity</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-warning bg-opacity-10">
            <div class="card-body"><h3 class="text-warning mb-0">{{ totalUsed }} TB</h3><small class="text-muted">Used</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-success bg-opacity-10">
            <div class="card-body"><h3 class="text-success mb-0">{{ totalCap - totalUsed }} TB</h3><small class="text-muted">Available</small></div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
            <div class="card-body"><h3 class="text-info mb-0">{{ (totalUsed / totalCap * 100).toFixed(1) }}%</h3><small class="text-muted">Overall Utilization</small></div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Storage Systems</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>System</th><th>Type</th><th class="text-end">Total (TB)</th><th class="text-end">Used (TB)</th><th style="width:18%">Utilization</th><th class="text-end">IOPS</th><th class="text-end">Throughput</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="s in systems" :key="s.id">
                <td class="fw-semibold">{{ s.name }}</td>
                <td><span class="badge bg-secondary">{{ s.type }}</span></td>
                <td class="text-end">{{ s.totalTB }}</td>
                <td class="text-end">{{ s.usedTB }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 8px;">
                      <div :class="`progress-bar bg-${utilColor(s.usedTB / s.totalTB * 100)}`" :style="{ width: (s.usedTB / s.totalTB * 100) + '%' }"></div>
                    </div>
                    <small>{{ (s.usedTB / s.totalTB * 100).toFixed(0) }}%</small>
                  </div>
                </td>
                <td class="text-end">{{ s.iops.toLocaleString() }}</td>
                <td class="text-end">{{ s.throughput }}</td>
                <td><span class="badge bg-success">{{ s.status }}</span></td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Storage System</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <input v-model="form.type" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Total (TB)</label>
              <input v-model.number="form.totalTB" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Used (TB)</label>
              <input v-model.number="form.usedTB" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">IOPS</label>
              <input v-model.number="form.iops" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Throughput</label>
              <input v-model="form.throughput" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>online</option>
                <option>degraded</option>
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
