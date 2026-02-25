<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { waterUsageRecordsApi, type WaterUsageRecord } from '../../services/api'

const currentWue = 1.42
const targetWue = 1.30

const monthly = ref<WaterUsageRecord[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<WaterUsageRecord | null>(null)

const defaultForm = { month: '', waterL: 0, itKwh: 0, wue: 0 }
const form = ref({ ...defaultForm })

function wueColor(wue: number): string {
  if (wue <= 1.30) return 'success'
  if (wue <= 1.50) return 'warning'
  return 'danger'
}

async function loadData() {
  monthly.value = await waterUsageRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: WaterUsageRecord) {
  editing.value = item
  form.value = { month: item.month, waterL: item.waterL, itKwh: item.itKwh, wue: item.wue }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await waterUsageRecordsApi.update(editing.value.id, form.value)
    } else {
      await waterUsageRecordsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await waterUsageRecordsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Water Usage Effectiveness (WUE)</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <div :class="`card border-0 shadow-sm text-center bg-${wueColor(currentWue)} bg-opacity-10`">
            <div class="card-body py-4">
              <div :class="`display-3 fw-bold text-${wueColor(currentWue)}`">{{ currentWue.toFixed(2) }}</div>
              <div class="text-muted">Current WUE (L/kWh)</div>
              <small class="text-muted">Target: {{ targetWue.toFixed(2) }}</small>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-info bg-opacity-10">
            <div class="card-body py-4">
              <h2 class="text-info mb-0">160,000 L</h2>
              <small class="text-muted">Monthly Water Consumption</small>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm text-center bg-primary bg-opacity-10">
            <div class="card-body py-4">
              <h2 class="text-primary mb-0">1.92M L</h2>
              <small class="text-muted">Annual Projection</small>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Monthly WUE Trend</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Month</th><th class="text-end">Water (L)</th><th class="text-end">IT Energy (kWh)</th><th class="text-end">WUE</th><th class="text-end">vs Target</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="m in monthly" :key="m.id">
                <td class="fw-semibold">{{ m.month }}</td>
                <td class="text-end">{{ m.waterL.toLocaleString() }}</td>
                <td class="text-end">{{ m.itKwh.toLocaleString() }}</td>
                <td class="text-end" :class="`text-${wueColor(m.wue)} fw-semibold`">{{ m.wue.toFixed(2) }}</td>
                <td class="text-end">
                  <span :class="m.wue <= targetWue ? 'text-success' : 'text-danger'">
                    {{ m.wue <= targetWue ? 'On target' : '+' + (m.wue - targetWue).toFixed(2) }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(m)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(m.id)">Delete</button>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Water Usage Record</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Month</label>
              <input v-model="form.month" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Water (L)</label>
              <input v-model.number="form.waterL" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">IT Energy (kWh)</label>
              <input v-model.number="form.itKwh" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">WUE</label>
              <input v-model.number="form.wue" type="number" class="form-control" />
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
