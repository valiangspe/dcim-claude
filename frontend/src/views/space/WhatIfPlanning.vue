<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { scenariosApi, type Scenario } from '../../services/api'

const scenarios = ref<Scenario[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<Scenario | null>(null)

const defaultForm = { name: '', racks: 0, power: 0, cooling: 0, space: 0, feasibility: 'possible', notes: '' }
const form = ref({ ...defaultForm })

function feasBadge(f: string): string {
  if (f === 'recommended') return 'bg-success'
  if (f === 'possible') return 'bg-info text-dark'
  return 'bg-danger'
}

async function loadData() {
  scenarios.value = await scenariosApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: Scenario) {
  editing.value = item
  form.value = { name: item.name, racks: item.racks, power: item.power, cooling: item.cooling, space: item.space, feasibility: item.feasibility, notes: item.notes }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await scenariosApi.update(editing.value.id, form.value)
    } else {
      await scenariosApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await scenariosApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">What-If Capacity Planning</h4>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Scenario</button>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3">
        <div v-for="s in scenarios" :key="s.id" class="col-12">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-transparent d-flex justify-content-between align-items-center">
              <span class="fw-semibold">{{ s.name }}</span>
              <div>
                <span class="badge me-2" :class="feasBadge(s.feasibility)">{{ s.feasibility }}</span>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(s)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(s.id)">Delete</button>
              </div>
            </div>
            <div class="card-body">
              <div class="row g-3 mb-3">
                <div class="col-sm-3">
                  <div class="border rounded p-2 text-center">
                    <div class="fw-semibold" :class="s.racks > 0 ? 'text-danger' : s.racks < 0 ? 'text-success' : ''">
                      {{ s.racks > 0 ? '+' : '' }}{{ s.racks }} racks
                    </div>
                    <small class="text-muted">Rack Impact</small>
                  </div>
                </div>
                <div class="col-sm-3">
                  <div class="border rounded p-2 text-center">
                    <div class="fw-semibold" :class="s.power > 0 ? 'text-danger' : 'text-success'">
                      {{ s.power > 0 ? '+' : '' }}{{ s.power }} kW
                    </div>
                    <small class="text-muted">Power Impact</small>
                  </div>
                </div>
                <div class="col-sm-3">
                  <div class="border rounded p-2 text-center">
                    <div class="fw-semibold" :class="s.cooling > 0 ? 'text-danger' : 'text-success'">
                      {{ s.cooling > 0 ? '+' : '' }}{{ s.cooling }} kW
                    </div>
                    <small class="text-muted">Cooling Impact</small>
                  </div>
                </div>
                <div class="col-sm-3">
                  <div class="border rounded p-2 text-center">
                    <div class="fw-semibold">{{ s.space }}</div>
                    <small class="text-muted">Space Impact</small>
                  </div>
                </div>
              </div>
              <p class="text-muted mb-0 small">{{ s.notes }}</p>
            </div>
          </div>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Scenario</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Racks</label>
              <input v-model.number="form.racks" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Power (kW)</label>
              <input v-model.number="form.power" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Cooling (kW)</label>
              <input v-model.number="form.cooling" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Space</label>
              <input v-model.number="form.space" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Feasibility</label>
              <select v-model="form.feasibility" class="form-select">
                <option>recommended</option>
                <option>possible</option>
                <option>not recommended</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Notes</label>
              <input v-model="form.notes" type="text" class="form-control" />
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
