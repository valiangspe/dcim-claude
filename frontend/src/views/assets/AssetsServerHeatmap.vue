<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { serverMetricsApi, type ServerMetric } from '../../services/api'

const servers = ['srv-web-01','srv-web-02','srv-db-01','srv-db-02','srv-gpu-01','srv-gpu-02','srv-app-01','srv-stor-01']
const hours = ['00','04','08','12','16','20']
const data: number[][] = [
  [52,48,62,72,68,58],[42,38,50,60,55,45],[68,64,78,88,82,72],[65,60,74,84,78,70],
  [88,85,92,95,93,90],[86,82,90,94,91,88],[32,28,42,52,48,38],[18,15,22,28,24,20],
]
function cellColor(v: number) { return v>=85?'bg-danger text-white':v>=70?'bg-warning text-dark':v>=50?'bg-info text-dark':'bg-success text-white' }

const items = ref<ServerMetric[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<ServerMetric | null>(null)
const saving = ref(false)

const defaultForm = { name: '', cpu: 0, gpu: 0, mem: 0, disk: 0, netIn: 0, netOut: 0, power: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  items.value = await serverMetricsApi.getAll()
}

onMounted(async () => {
  try { await loadData() } finally { loading.value = false }
})

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: ServerMetric) {
  editing.value = item
  form.value = { name: item.name, cpu: item.cpu, gpu: item.gpu, mem: item.mem, disk: item.disk, netIn: item.netIn, netOut: item.netOut, power: item.power }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await serverMetricsApi.update(editing.value.id, form.value)
    } else {
      await serverMetricsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this server metric?')) return
  await serverMetricsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Server Heatmap</h4>
    <p class="text-muted">CPU utilization (%) by server and time of day.</p>
    <div class="card border-0 shadow-sm mb-3"><div class="card-body p-0"><table class="table table-bordered text-center mb-0">
      <thead class="table-light"><tr><th></th><th v-for="h in hours" :key="h">{{ h }}:00</th></tr></thead>
      <tbody><tr v-for="(row, i) in data" :key="servers[i]">
        <td class="fw-semibold table-light text-start">{{ servers[i] }}</td>
        <td v-for="(v, j) in row" :key="j" :class="cellColor(v)">{{ v }}</td>
      </tr></tbody>
    </table></div></div>
    <div class="d-flex gap-3 small mb-4"><span><span class="badge bg-success">&nbsp;&nbsp;</span> &lt;50%</span><span><span class="badge bg-info">&nbsp;&nbsp;</span> 50-70%</span><span><span class="badge bg-warning">&nbsp;&nbsp;</span> 70-85%</span><span><span class="badge bg-danger">&nbsp;&nbsp;</span> &ge;85%</span></div>

    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0">Server Metrics</h5>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Server Metric</button>
    </div>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Name</th><th class="text-end">CPU</th><th class="text-end">GPU</th><th class="text-end">Mem</th><th class="text-end">Disk</th><th class="text-end">Net In</th><th class="text-end">Net Out</th><th class="text-end">Power</th><th>Actions</th></tr></thead>
      <tbody><tr v-for="item in items" :key="item.id">
        <td class="fw-semibold">{{ item.name }}</td><td class="text-end">{{ item.cpu }}</td><td class="text-end">{{ item.gpu }}</td><td class="text-end">{{ item.mem }}</td><td class="text-end">{{ item.disk }}</td><td class="text-end">{{ item.netIn }}</td><td class="text-end">{{ item.netOut }}</td><td class="text-end">{{ item.power }}</td>
        <td>
          <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(item)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(item.id)">Delete</button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Server Metric</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><label class="form-label">Name</label><input v-model="form.name" type="text" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">CPU</label><input v-model.number="form.cpu" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">GPU</label><input v-model.number="form.gpu" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Mem</label><input v-model.number="form.mem" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Disk</label><input v-model.number="form.disk" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Net In</label><input v-model.number="form.netIn" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Net Out</label><input v-model.number="form.netOut" type="number" class="form-control" /></div>
            <div class="mb-3"><label class="form-label">Power</label><input v-model.number="form.power" type="number" class="form-control" /></div>
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
