<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { serverPowerRecordsApi, type ServerPowerRecord } from '../../services/api'

const servers = ref<ServerPowerRecord[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<ServerPowerRecord | null>(null)

const defaultForm = { name: '', rack: '', power: 0, cpu: 0, psu: '', status: 'online' }
const form = ref({ ...defaultForm })

const totalPower = computed(() => servers.value.reduce((a, s) => a + s.power, 0))

async function loadData() {
  servers.value = await serverPowerRecordsApi.getAll()
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

function openEdit(item: ServerPowerRecord) {
  editing.value = item
  form.value = { name: item.name, rack: item.rack, power: item.power, cpu: item.cpu, psu: item.psu, status: item.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await serverPowerRecordsApi.update(editing.value.id, form.value)
    } else {
      await serverPowerRecordsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await serverPowerRecordsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Server Power</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="alert alert-light border mb-4">
        <strong>Total Server Power:</strong> {{ totalPower.toFixed(2) }} kW across {{ servers.length }} servers
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Servers</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Server</th><th>Rack</th><th>PSU</th><th class="text-end">Power (kW)</th><th class="text-end">CPU %</th><th>Status</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="s in servers" :key="s.id">
                <td class="fw-semibold font-monospace">{{ s.name }}</td>
                <td>{{ s.rack }}</td>
                <td class="text-muted small">{{ s.psu }}</td>
                <td class="text-end">{{ s.power.toFixed(2) }}</td>
                <td class="text-end">
                  <span :class="s.cpu > 80 ? 'text-danger fw-semibold' : s.cpu > 60 ? 'text-warning' : ''">{{ s.cpu }}%</span>
                </td>
                <td><span class="badge" :class="s.status === 'online' ? 'bg-success' : 'bg-secondary'">{{ s.status }}</span></td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Server Power Record</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Rack</label>
              <input v-model="form.rack" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Power (kW)</label>
              <input v-model.number="form.power" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">CPU (%)</label>
              <input v-model.number="form.cpu" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">PSU</label>
              <input v-model="form.psu" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>online</option>
                <option>offline</option>
                <option>standby</option>
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
