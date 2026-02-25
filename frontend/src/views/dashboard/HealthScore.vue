<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { healthScoreCategoriesApi, type HealthScoreCategory } from '../../services/api'

const overallScore = 87

const categories = ref<HealthScoreCategory[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<HealthScoreCategory | null>(null)

const defaultForm = { name: '', score: 0, color: 'success' }
const form = ref({ ...defaultForm })

function scoreColor(score: number): string {
  if (score >= 90) return 'success'
  if (score >= 70) return 'warning'
  return 'danger'
}

const history = [
  { date: '2026-02-20', score: 87, change: 0 },
  { date: '2026-02-19', score: 87, change: 1 },
  { date: '2026-02-18', score: 86, change: -2 },
  { date: '2026-02-17', score: 88, change: 3 },
  { date: '2026-02-16', score: 85, change: -1 },
  { date: '2026-02-15', score: 86, change: 2 },
  { date: '2026-02-14', score: 84, change: -3 },
]

async function loadData() {
  categories.value = await healthScoreCategoriesApi.getAll()
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

function openEdit(item: HealthScoreCategory) {
  editing.value = item
  form.value = { name: item.name, score: item.score, color: item.color }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await healthScoreCategoriesApi.update(editing.value.id, form.value)
    } else {
      await healthScoreCategoriesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await healthScoreCategoriesApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Health Score</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <!-- Overall Score -->
      <div class="row justify-content-center mb-4">
        <div class="col-md-4">
          <div :class="`card border-0 shadow-sm text-center bg-${scoreColor(overallScore)} bg-opacity-10`">
            <div class="card-body py-5">
              <div :class="`display-1 fw-bold text-${scoreColor(overallScore)}`">{{ overallScore }}</div>
              <div class="text-muted fs-5">/ 100</div>
              <div class="mt-2 fw-semibold">Overall Health Score</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Category Breakdown -->
      <div class="row g-3 mb-4">
        <div v-for="cat in categories" :key="cat.name" class="col-sm-6 col-lg-3">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body text-center">
              <h2 :class="`text-${cat.color} mb-1`">{{ cat.score }}</h2>
              <div class="progress mb-2" style="height: 8px;">
                <div
                  :class="`progress-bar bg-${cat.color}`"
                  :style="{ width: cat.score + '%' }"
                ></div>
              </div>
              <small class="text-muted">{{ cat.name }}</small>
            </div>
          </div>
        </div>
      </div>

      <!-- Categories Table with CRUD -->
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Health Score Categories</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Name</th><th class="text-end">Score</th><th>Color</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="cat in categories" :key="cat.id">
                <td class="fw-semibold">{{ cat.name }}</td>
                <td class="text-end">{{ cat.score }}</td>
                <td><span class="badge" :class="`bg-${cat.color}`">{{ cat.color }}</span></td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(cat)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(cat.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Historical Trend -->
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold">7-Day Trend</div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Date</th>
                <th>Score</th>
                <th style="width: 40%;">Bar</th>
                <th class="text-end">Change</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in history" :key="row.date">
                <td>{{ row.date }}</td>
                <td class="fw-semibold">{{ row.score }}</td>
                <td>
                  <div class="progress" style="height: 10px;">
                    <div
                      :class="`progress-bar bg-${scoreColor(row.score)}`"
                      :style="{ width: row.score + '%' }"
                    ></div>
                  </div>
                </td>
                <td class="text-end">
                  <span v-if="row.change > 0" class="text-success">+{{ row.change }}</span>
                  <span v-else-if="row.change < 0" class="text-danger">{{ row.change }}</span>
                  <span v-else class="text-muted">0</span>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Health Score Category</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Score</label>
              <input v-model.number="form.score" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Color</label>
              <select v-model="form.color" class="form-select">
                <option>success</option>
                <option>warning</option>
                <option>danger</option>
                <option>primary</option>
                <option>info</option>
                <option>secondary</option>
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
