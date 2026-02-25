<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { pueReadingsApi, type PueReading } from '../../services/api'

const currentPue = 1.38
const target = 1.40

const daily = ref<PueReading[]>([])
const breakdown = ref<PueReading[]>([])
const allReadings = ref<PueReading[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PueReading | null>(null)

const defaultForm = { date: '', pue: 0, component: '', power: 0, pct: 0 }
const form = ref({ ...defaultForm })

function pueColor(pue: number): string {
  if (pue <= 1.35) return 'success'
  if (pue <= 1.45) return 'warning'
  return 'danger'
}

async function loadData() {
  const all = await pueReadingsApi.getAll()
  allReadings.value = all
  daily.value = all.filter(r => r.date)
  breakdown.value = all.filter(r => r.component)
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

function openEdit(item: PueReading) {
  editing.value = item
  form.value = { date: item.date, pue: item.pue, component: item.component, power: item.power, pct: item.pct }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await pueReadingsApi.update(editing.value.id, form.value)
    } else {
      await pueReadingsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await pueReadingsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">PUE Monitoring</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <div :class="`card border-0 shadow-sm text-center bg-${pueColor(currentPue)} bg-opacity-10`">
            <div class="card-body py-4">
              <div :class="`display-3 fw-bold text-${pueColor(currentPue)}`">{{ currentPue.toFixed(2) }}</div>
              <div class="text-muted">Current PUE</div>
              <small class="text-muted">Target: {{ target.toFixed(2) }}</small>
            </div>
          </div>
        </div>
        <div class="col-md-8">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Power Breakdown</div>
            <div class="card-body">
              <div v-for="b in breakdown" :key="b.component" class="mb-3">
                <div class="d-flex justify-content-between mb-1">
                  <span>{{ b.component }}</span>
                  <span class="fw-semibold">{{ b.power.toLocaleString() }} kW ({{ b.pct }}%)</span>
                </div>
                <div class="progress" style="height: 8px;">
                  <div class="progress-bar bg-primary" :style="{ width: b.pct + '%' }"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>PUE Readings</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Date</th><th>Component</th><th class="text-end">PUE</th><th class="text-end">Power (kW)</th><th class="text-end">Pct</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="r in allReadings" :key="r.id">
                <td>{{ r.date || '---' }}</td>
                <td>{{ r.component || '---' }}</td>
                <td class="text-end">{{ r.pue > 0 ? r.pue.toFixed(2) : '---' }}</td>
                <td class="text-end">{{ r.power > 0 ? r.power.toLocaleString() : '---' }}</td>
                <td class="text-end">{{ r.pct > 0 ? r.pct + '%' : '---' }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(r)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(r.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">7-Day PUE Trend</div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr><th>Date</th><th>PUE</th><th style="width:50%">Bar</th><th class="text-end">vs Target</th></tr>
            </thead>
            <tbody>
              <tr v-for="d in daily" :key="d.date">
                <td>{{ d.date }}</td>
                <td :class="`fw-semibold text-${pueColor(d.pue)}`">{{ d.pue.toFixed(2) }}</td>
                <td>
                  <div class="progress" style="height: 10px;">
                    <div :class="`progress-bar bg-${pueColor(d.pue)}`" :style="{ width: ((d.pue - 1.0) / 0.6 * 100) + '%' }"></div>
                  </div>
                </td>
                <td class="text-end">
                  <span :class="d.pue <= target ? 'text-success' : 'text-danger'">
                    {{ d.pue <= target ? 'On target' : '+' + (d.pue - target).toFixed(2) }}
                  </span>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} PUE Reading</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Date</label>
              <input v-model="form.date" type="text" class="form-control" placeholder="e.g. 2026-02-20" />
            </div>
            <div class="mb-3">
              <label class="form-label">PUE</label>
              <input v-model.number="form.pue" type="number" step="0.01" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Component</label>
              <input v-model="form.component" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Power (kW)</label>
              <input v-model.number="form.power" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Pct (%)</label>
              <input v-model.number="form.pct" type="number" class="form-control" />
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
