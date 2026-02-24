<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { powerFeedsApi, type PowerFeed } from '../../services/api'

const loading = ref(true)
const feeds = ref<PowerFeed[]>([])
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PowerFeed | null>(null)

const defaultForm = { name: '', capacity: 0, load: 0, status: 'active' }
const form = ref({ ...defaultForm })

async function loadData() {
  feeds.value = await powerFeedsApi.getAll()
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

function openEdit(feed: PowerFeed) {
  editing.value = feed
  form.value = { name: feed.name, capacity: feed.capacity, load: feed.load, status: feed.status }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await powerFeedsApi.update(editing.value.id, form.value)
    } else {
      await powerFeedsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await powerFeedsApi.remove(id)
  await loadData()
}

const summary = computed(() => {
  const totalCapacity = feeds.value.reduce((a, f) => a + f.capacity, 0)
  const currentLoad = feeds.value.reduce((a, f) => a + f.load, 0)
  const loadPct = totalCapacity > 0 ? (currentLoad / totalCapacity * 100) : 0
  return [
    { label: 'Total Capacity', value: totalCapacity.toLocaleString(), unit: 'kW', color: 'primary' },
    { label: 'Current Load', value: currentLoad.toLocaleString(), unit: 'kW', color: 'info' },
    { label: 'Load %', value: loadPct.toFixed(1), unit: '%', color: 'warning' },
    { label: 'PUE', value: '1.38', unit: '', color: 'success' },
    { label: 'Redundancy', value: 'N+1', unit: '', color: 'secondary' },
    { label: 'Cost / kWh', value: '$0.087', unit: '', color: 'dark' },
  ]
})

const hourly = [
  { time: '00:00', load: 2650 }, { time: '02:00', load: 2610 },
  { time: '04:00', load: 2580 }, { time: '06:00', load: 2640 },
  { time: '08:00', load: 2720 }, { time: '10:00', load: 2790 },
  { time: '12:00', load: 2860 }, { time: '14:00', load: 2880 },
  { time: '16:00', load: 2850 }, { time: '18:00', load: 2810 },
  { time: '20:00', load: 2760 }, { time: '22:00', load: 2700 },
]

const maxLoad = 3000
</script>

<template>
  <div>
    <h4 class="mb-4">Power Overview</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="s in summary" :key="s.label" class="col-sm-6 col-lg-4 col-xl-2">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body text-center">
              <h3 :class="`text-${s.color} mb-0`">{{ s.value }}<small class="text-muted fs-6">{{ s.unit }}</small></h3>
              <small class="text-muted">{{ s.label }}</small>
            </div>
          </div>
        </div>
      </div>

      <div class="row g-3 mb-4">
        <div class="col-lg-6">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
              <span>Power Feeds</span>
              <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Feed</button>
            </div>
            <div class="card-body p-0">
              <table class="table table-hover align-middle mb-0">
                <thead class="table-light">
                  <tr><th>Feed</th><th>Capacity</th><th>Load</th><th>Util</th><th>Status</th><th>Actions</th></tr>
                </thead>
                <tbody>
                  <tr v-for="f in feeds" :key="f.id">
                    <td class="fw-semibold">{{ f.name }}</td>
                    <td>{{ f.capacity.toLocaleString() }} kW</td>
                    <td>{{ f.load > 0 ? f.load.toLocaleString() + ' kW' : '---' }}</td>
                    <td>
                      <div v-if="f.load > 0" class="progress" style="height: 8px; width: 80px;">
                        <div class="progress-bar" :class="f.load / f.capacity > 0.8 ? 'bg-danger' : 'bg-success'" :style="{ width: (f.load / f.capacity * 100) + '%' }"></div>
                      </div>
                      <span v-else class="text-muted">---</span>
                    </td>
                    <td><span class="badge" :class="f.status === 'active' ? 'bg-success' : 'bg-secondary'">{{ f.status }}</span></td>
                    <td>
                      <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(f)">Edit</button>
                      <button class="btn btn-sm btn-outline-danger" @click="remove(f.id)">Delete</button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div class="col-lg-6">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">24h Load Profile</div>
            <div class="card-body p-0">
              <table class="table table-sm table-hover mb-0">
                <thead class="table-light">
                  <tr><th>Time</th><th>Load (kW)</th><th style="width:50%">Bar</th></tr>
                </thead>
                <tbody>
                  <tr v-for="h in hourly" :key="h.time">
                    <td>{{ h.time }}</td>
                    <td>{{ h.load.toLocaleString() }}</td>
                    <td>
                      <div class="progress" style="height: 10px;">
                        <div class="progress-bar bg-primary" :style="{ width: (h.load / maxLoad * 100) + '%' }"></div>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Feed</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
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
