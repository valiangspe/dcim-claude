<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1 class="h3 mb-0">Floor Plan Visualization</h1>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Rack</button>
    </div>

    <div class="row mb-4">
      <div class="col-auto">
        <div class="btn-group btn-group-sm" role="group">
          <button type="button" class="btn" :class="selectedHall === 'all' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedHall = 'all'">All Halls</button>
          <button type="button" class="btn" :class="selectedHall === 'A' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedHall = 'A'">Hall A</button>
          <button type="button" class="btn" :class="selectedHall === 'B' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedHall = 'B'">Hall B</button>
          <button type="button" class="btn" :class="selectedHall === 'C' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedHall = 'C'">Hall C</button>
        </div>
      </div>
      <div class="col-auto ms-auto">
        <select class="form-select form-select-sm" style="width: auto;">
          <option value="temp">Temperature</option>
          <option value="power">Power Usage</option>
          <option value="util">Utilization</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-body">
        <div class="floor-plan" style="background-color: #f8f9fa; padding: 20px; border-radius: 8px;">
          <div v-for="rack in filteredRacks" :key="rack.id" class="d-inline-block me-2 mb-2">
            <div class="rack-cell" :class="getRackColorClass(rack)"
                 :title="`${rack.rackId}: ${rack.status}`"
                 style="width: 60px; height: 80px; border: 1px solid #ccc; display: flex; align-items: center; justify-content: center; border-radius: 4px; cursor: pointer; font-weight: bold; color: white;">
              {{ rack.rackId }}
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer">
        <div class="row">
          <div class="col-auto">
            <span class="badge bg-success me-2">Optimal</span>
            <span class="badge bg-warning me-2">Warning</span>
            <span class="badge bg-danger">Critical</span>
          </div>
        </div>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header fw-semibold">Rack Data</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Rack ID</th><th>Hall</th><th>Status</th><th class="text-end">Utilization</th><th>Actions</th></tr>
          </thead>
          <tbody>
            <tr v-for="rack in filteredRacks" :key="rack.id">
              <td class="fw-medium">{{ rack.rackId }}</td>
              <td>{{ rack.hall }}</td>
              <td><span :class="['badge', rack.status === 'Optimal' ? 'bg-success' : rack.status === 'Warning' ? 'bg-warning' : 'bg-danger']">{{ rack.status }}</span></td>
              <td class="text-end">{{ rack.utilization }}%</td>
              <td>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(rack)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(rack.id)">Delete</button>
              </td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Rack</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Rack ID</label><input v-model="form.rackId" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Hall</label><input v-model="form.hall" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Status</label><input v-model="form.status" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Utilization</label><input v-model.number="form.utilization" type="number" class="form-control" /></div>
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

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { visRacksApi, type VisRack } from '../../services/api'

const selectedHall = ref('all')
const racks = ref<VisRack[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<VisRack | null>(null)
const saving = ref(false)

const defaultForm = { rackId: '', hall: '', status: '', utilization: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  racks.value = await visRacksApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})

const filteredRacks = computed(() => {
  if (selectedHall.value === 'all') {
    return racks.value
  }
  return racks.value.filter(rack => rack.hall === selectedHall.value)
})

function getRackColorClass(rack: { status: string }): string {
  if (rack.status === 'Optimal') return 'bg-success';
  if (rack.status === 'Warning') return 'bg-warning';
  return 'bg-danger';
}

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: VisRack) {
  editing.value = item
  form.value = { rackId: item.rackId, hall: item.hall, status: item.status, utilization: item.utilization }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await visRacksApi.update(editing.value.id, form.value)
    } else {
      await visRacksApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this rack?')) return
  await visRacksApi.remove(id)
  await loadData()
}
</script>
