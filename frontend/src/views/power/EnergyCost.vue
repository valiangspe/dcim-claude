<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { energyCostRecordsApi, type EnergyCostRecord } from '../../services/api'

const summary = [
  { label: 'Avg Cost / kWh', value: '$0.087', color: 'primary' },
  { label: 'Monthly Bill', value: '$178,420', color: 'danger' },
  { label: 'YTD Spend', value: '$354,810', color: 'warning' },
  { label: 'Budget Remaining', value: '$745,190', color: 'success' },
]

const monthly = ref<EnergyCostRecord[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<EnergyCostRecord | null>(null)

const defaultForm = { month: '', kwh: 0, cost: 0, rate: 0 }
const form = ref({ ...defaultForm })

const maxCost = 200000

async function loadData() {
  monthly.value = await energyCostRecordsApi.getAll()
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

function openEdit(item: EnergyCostRecord) {
  editing.value = item
  form.value = { month: item.month, kwh: item.kwh, cost: item.cost, rate: item.rate }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await energyCostRecordsApi.update(editing.value.id, form.value)
    } else {
      await energyCostRecordsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await energyCostRecordsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Energy Cost & Billing</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div v-for="s in summary" :key="s.label" class="col-sm-6 col-lg-3">
          <div class="card border-0 shadow-sm text-center h-100">
            <div class="card-body">
              <h3 :class="`text-${s.color} mb-0`">{{ s.value }}</h3>
              <small class="text-muted">{{ s.label }}</small>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Monthly Billing</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Month</th><th class="text-end">Consumption (kWh)</th><th class="text-end">Rate ($/kWh)</th><th class="text-end">Cost ($)</th><th style="width:20%">Bar</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="m in monthly" :key="m.id">
                <td class="fw-semibold">{{ m.month }}</td>
                <td class="text-end">{{ m.kwh.toLocaleString() }}</td>
                <td class="text-end">{{ m.rate.toFixed(4) }}</td>
                <td class="text-end fw-semibold">${{ m.cost.toLocaleString() }}</td>
                <td>
                  <div class="progress" style="height: 10px;">
                    <div class="progress-bar bg-danger" :style="{ width: (m.cost / maxCost * 100) + '%' }"></div>
                  </div>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Energy Cost Record</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Month</label>
              <input v-model="form.month" type="text" class="form-control" placeholder="e.g. Jan 2026" />
            </div>
            <div class="mb-3">
              <label class="form-label">Consumption (kWh)</label>
              <input v-model.number="form.kwh" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Cost ($)</label>
              <input v-model.number="form.cost" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Rate ($/kWh)</label>
              <input v-model.number="form.rate" type="number" step="0.0001" class="form-control" />
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
