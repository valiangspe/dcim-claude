<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { trafficRecordsApi, type TrafficRecord } from '../../services/api'

const hourly = ref<TrafficRecord[]>([])
const max = 400
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<TrafficRecord | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { time: '', inbound: 0, outbound: 0, total: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  hourly.value = await trafficRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: TrafficRecord) {
  editing.value = item
  form.value = { time: item.time, inbound: item.inbound, outbound: item.outbound, total: item.total }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await trafficRecordsApi.update(editing.value.id, form.value)
    else await trafficRecordsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: TrafficRecord) {
  if (!confirm(`Delete record "${item.time}"?`)) return
  deleting.value = item.id
  try { await trafficRecordsApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Network Traffic</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="row g-3 mb-4">
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-success bg-opacity-10"><div class="card-body"><h3 class="text-success mb-0">142 Gbps</h3><small class="text-muted">Inbound</small></div></div></div>
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10"><div class="card-body"><h3 class="text-primary mb-0">105 Gbps</h3><small class="text-muted">Outbound</small></div></div></div>
      <div class="col-md-4"><div class="card border-0 shadow-sm text-center bg-info bg-opacity-10"><div class="card-body"><h3 class="text-info mb-0">247 Gbps</h3><small class="text-muted">Total</small></div></div></div>
    </div>
    <div class="card border-0 shadow-sm"><div class="card-header bg-transparent fw-semibold d-flex align-items-center justify-content-between"><span>24h Traffic Profile (Gbps)</span><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div><div class="card-body p-0"><table class="table table-hover mb-0">
      <thead class="table-light"><tr><th>Time</th><th class="text-end">In</th><th class="text-end">Out</th><th class="text-end">Total</th><th style="width:25%">Bar</th><th style="width:120px">Actions</th></tr></thead>
      <tbody><tr v-for="h in hourly" :key="h.id">
        <td>{{ h.time }}</td><td class="text-end text-success">{{ h.inbound }}</td><td class="text-end text-primary">{{ h.outbound }}</td><td class="text-end fw-semibold">{{ h.total }}</td>
        <td><div class="progress" style="height:12px"><div class="progress-bar bg-success" :style="{width:(h.inbound/max*100)+'%'}"></div><div class="progress-bar bg-primary" :style="{width:(h.outbound/max*100)+'%'}"></div></div></td>
        <td>
          <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(h)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(h)" :disabled="deleting === h.id">
            <span v-if="deleting === h.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
          </button>
        </td>
      </tr></tbody>
    </table></div></div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Traffic Record</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Time</label><input v-model="form.time" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Inbound</label><input v-model.number="form.inbound" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Outbound</label><input v-model.number="form.outbound" type="number" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Total</label><input v-model.number="form.total" type="number" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
