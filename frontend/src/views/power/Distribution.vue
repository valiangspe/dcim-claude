<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { powerChainsApi, type PowerChain, pdusApi, type Pdu } from '../../services/api'

const chains = ref<PowerChain[]>([])
const pdus = ref<Pdu[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PowerChain | null>(null)

const defaultForm = { name: '', source: '', capacity: 0, load: 0, redundancy: 'N+1', status: 'active' }
const form = ref({ ...defaultForm })

async function loadData() {
  const [chainsData, pdusData] = await Promise.all([
    powerChainsApi.getAll(),
    pdusApi.getAll(),
  ])
  chains.value = chainsData
  pdus.value = pdusData
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

function openEdit(item: PowerChain) {
  editing.value = item
  form.value = { name: item.name, source: item.source, capacity: item.capacity, load: item.load, redundancy: item.redundancy, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await powerChainsApi.update(editing.value.id, form.value)
    } else {
      await powerChainsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await powerChainsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Distribution & Redundancy</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Power Chains</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Chain</th><th>Source</th><th class="text-end">Capacity</th><th class="text-end">Load</th><th>Redundancy</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="c in chains" :key="c.id">
                <td class="fw-semibold">{{ c.name }}</td>
                <td>{{ c.source }}</td>
                <td class="text-end">{{ c.capacity.toLocaleString() }} kW</td>
                <td class="text-end">{{ c.load > 0 ? c.load.toLocaleString() + ' kW' : '---' }}</td>
                <td><span class="badge bg-info text-dark">{{ c.redundancy }}</span></td>
                <td><span class="badge" :class="c.status === 'active' ? 'bg-success' : 'bg-secondary'">{{ c.status }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(c)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(c.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">PDU Details</div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>PDU</th><th>Chain</th><th class="text-end">Input (V)</th><th class="text-end">Output (V)</th><th class="text-end">Load (kW)</th><th class="text-end">Capacity (kW)</th><th class="text-end">Breakers</th></tr>
            </thead>
            <tbody>
              <tr v-for="p in pdus" :key="p.name">
                <td class="fw-semibold">{{ p.name }}</td>
                <td>{{ p.chain }}</td>
                <td class="text-end">{{ p.input }}</td>
                <td class="text-end">{{ p.output }}</td>
                <td class="text-end">{{ p.load > 0 ? p.load : '---' }}</td>
                <td class="text-end">{{ p.capacity }}</td>
                <td class="text-end">{{ p.breakers }}</td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Power Chain</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Source</label>
              <input v-model="form.source" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Capacity (kW)</label>
              <input v-model.number="form.capacity" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Load (kW)</label>
              <input v-model.number="form.load" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Redundancy</label>
              <select v-model="form.redundancy" class="form-select">
                <option>N+1</option>
                <option>2N</option>
                <option>2N+1</option>
                <option>N</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>active</option>
                <option>standby</option>
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
