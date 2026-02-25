<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { powerBudgetsApi, type PowerBudget } from '../../services/api'

const sites = ref<PowerBudget[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PowerBudget | null>(null)

const defaultForm = { name: '', budget: 0, actual: 0, reserved: 0 }
const form = ref({ ...defaultForm })

function utilColor(pct: number): string {
  if (pct >= 85) return 'danger'
  if (pct >= 70) return 'warning'
  return 'success'
}

async function loadData() {
  sites.value = await powerBudgetsApi.getAll()
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

function openEdit(item: PowerBudget) {
  editing.value = item
  form.value = { name: item.name, budget: item.budget, actual: item.actual, reserved: item.reserved }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await powerBudgetsApi.update(editing.value.id, form.value)
    } else {
      await powerBudgetsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await powerBudgetsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Budget Capacity</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Power Budgets</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th>Site</th>
                <th class="text-end">Budget (kW)</th>
                <th class="text-end">Actual (kW)</th>
                <th class="text-end">Reserved (kW)</th>
                <th class="text-end">Available (kW)</th>
                <th style="width:20%">Utilization</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="s in sites" :key="s.id">
                <td class="fw-semibold">{{ s.name }}</td>
                <td class="text-end">{{ s.budget.toLocaleString() }}</td>
                <td class="text-end">{{ s.actual > 0 ? s.actual.toLocaleString() : '---' }}</td>
                <td class="text-end">{{ s.reserved }}</td>
                <td class="text-end">{{ (s.budget - s.actual - s.reserved).toLocaleString() }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 10px;">
                      <div :class="`progress-bar bg-${utilColor(s.actual / s.budget * 100)}`" :style="{ width: (s.actual / s.budget * 100) + '%' }"></div>
                    </div>
                    <small class="text-nowrap">{{ (s.actual / s.budget * 100).toFixed(1) }}%</small>
                  </div>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(s)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(s.id)">Delete</button>
                </td>
              </tr>
            </tbody>
            <tfoot class="table-light">
              <tr class="fw-semibold">
                <td>Total</td>
                <td class="text-end">{{ sites.reduce((a, s) => a + s.budget, 0).toLocaleString() }}</td>
                <td class="text-end">{{ sites.reduce((a, s) => a + s.actual, 0).toLocaleString() }}</td>
                <td class="text-end">{{ sites.reduce((a, s) => a + s.reserved, 0) }}</td>
                <td class="text-end">{{ sites.reduce((a, s) => a + s.budget - s.actual - s.reserved, 0).toLocaleString() }}</td>
                <td></td>
                <td></td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Power Budget</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Budget (kW)</label>
              <input v-model.number="form.budget" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Actual (kW)</label>
              <input v-model.number="form.actual" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Reserved (kW)</label>
              <input v-model.number="form.reserved" type="number" class="form-control" />
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
