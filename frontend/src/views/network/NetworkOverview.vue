<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { networkLinksApi, type NetworkLink } from '../../services/api'

const loading = ref(true)
const links = ref<NetworkLink[]>([])

const showModal = ref(false)
const editing = ref<NetworkLink | null>(null)
const saving = ref(false)
const deleting = ref<number | null>(null)
const form = ref({ name: '', capacity: 0, usage: 0, latency: 0, loss: 0, status: 'active' })

async function loadData() {
  links.value = await networkLinksApi.getAll()
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
  form.value = { name: '', capacity: 0, usage: 0, latency: 0, loss: 0, status: 'active' }
  showModal.value = true
}

function openEdit(l: NetworkLink) {
  editing.value = l
  form.value = { name: l.name, capacity: l.capacity, usage: l.usage, latency: l.latency, loss: l.loss, status: l.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await networkLinksApi.update(editing.value.id, form.value)
    else await networkLinksApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(l: NetworkLink) {
  if (!confirm(`Delete link "${l.name}"?`)) return
  deleting.value = l.id
  try {
    await networkLinksApi.remove(l.id)
    await loadData()
  } finally {
    deleting.value = null
  }
}

const kpis = computed(() => {
  const totalBw = links.value.reduce((a, l) => a + l.capacity, 0)
  const totalUsage = links.value.reduce((a, l) => a + l.usage, 0)
  const activeCount = links.value.filter(l => l.status === 'active').length
  return [
    { label: 'Total Bandwidth', value: totalBw.toLocaleString(), unit: 'Gbps', color: 'primary' },
    { label: 'Current Throughput', value: totalUsage.toLocaleString(), unit: 'Gbps', color: 'info' },
    { label: 'Uptime', value: '99.97', unit: '%', color: 'success' },
    { label: 'Active IPs', value: '12,480', unit: '', color: 'dark' },
    { label: 'Redundancy', value: 'Dual', unit: '', color: 'secondary' },
    { label: 'Active Links', value: activeCount.toString(), unit: '', color: 'danger' },
  ]
})
</script>

<template>
  <div>
    <h4 class="mb-4">Network Overview</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="k in kpis" :key="k.label" class="col-sm-6 col-lg-4 col-xl-2">
          <div class="card border-0 shadow-sm h-100"><div class="card-body text-center">
            <h3 :class="`text-${k.color} mb-0`">{{ k.value }}<small class="text-muted fs-6">{{ k.unit }}</small></h3>
            <small class="text-muted">{{ k.label }}</small>
          </div></div>
        </div>
      </div>
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between">
          <span>Uplinks & Inter-DC Links</span>
          <button class="btn btn-sm btn-primary" @click="openCreate">Add Link</button>
        </div>
        <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
          <thead class="table-light"><tr><th>Link</th><th class="text-end">Capacity (Gbps)</th><th class="text-end">Usage (Gbps)</th><th style="width:18%">Utilization</th><th class="text-end">Latency (ms)</th><th class="text-end">Loss %</th><th>Status</th><th style="width:120px">Actions</th></tr></thead>
          <tbody><tr v-for="l in links" :key="l.id">
            <td class="fw-semibold">{{ l.name }}</td>
            <td class="text-end">{{ l.capacity }}</td><td class="text-end">{{ l.usage }}</td>
            <td><div class="d-flex align-items-center gap-2"><div class="progress flex-fill" style="height:8px"><div class="progress-bar bg-primary" :style="{width:(l.usage/l.capacity*100)+'%'}"></div></div><small>{{ (l.usage/l.capacity*100).toFixed(0) }}%</small></div></td>
            <td class="text-end">{{ l.latency }}</td><td class="text-end">{{ l.loss.toFixed(2) }}%</td>
            <td><span class="badge bg-success">{{ l.status }}</span></td>
            <td>
              <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(l)">Edit</button>
              <button class="btn btn-sm btn-outline-danger" @click="remove(l)" :disabled="deleting === l.id">
                <span v-if="deleting === l.id" class="spinner-border spinner-border-sm"></span>
                <span v-else>Delete</span>
              </button>
            </td>
          </tr></tbody>
        </table></div>
      </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Link</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Capacity (Gbps)</label>
              <input v-model.number="form.capacity" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Usage (Gbps)</label>
              <input v-model.number="form.usage" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Latency (ms)</label>
              <input v-model.number="form.latency" type="number" step="0.1" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Packet Loss %</label>
              <input v-model.number="form.loss" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="active">active</option>
                <option value="degraded">degraded</option>
                <option value="down">down</option>
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
