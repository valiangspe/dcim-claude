<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { coolingSystemsApi, type CoolingSystem } from '../../services/api'

const systems = ref<CoolingSystem[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CoolingSystem | null>(null)

const defaultForm = { name: '', status: 'running', uptime: '', lastMaint: '', nextMaint: '', downtime: '' }
const form = ref({ ...defaultForm })

function maintUrgency(next: string): string {
  const days = Math.ceil((new Date(next).getTime() - new Date('2026-02-20').getTime()) / 86400000)
  if (days <= 7) return 'text-danger fw-semibold'
  if (days <= 30) return 'text-warning'
  return ''
}

async function loadData() {
  systems.value = await coolingSystemsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: CoolingSystem) {
  editing.value = item
  form.value = { name: item.name, status: item.status, uptime: item.uptime, lastMaint: item.lastMaint, nextMaint: item.nextMaint, downtime: item.downtime }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await coolingSystemsApi.update(editing.value.id, form.value)
    } else {
      await coolingSystemsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await coolingSystemsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cooling System Status</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Cooling Systems</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>System</th><th>Status</th><th class="text-end">Uptime %</th><th class="text-end">Downtime (h/mo)</th><th>Last Maint.</th><th>Next Maint.</th><th>Actions</th></tr>
              </thead>
              <tbody>
                <tr v-for="s in systems" :key="s.id">
                  <td class="fw-semibold">{{ s.name }}</td>
                  <td>
                    <span class="badge" :class="{ 'bg-success': s.status === 'running', 'bg-warning text-dark': s.status === 'warning', 'bg-secondary': s.status === 'standby' }">{{ s.status }}</span>
                  </td>
                  <td class="text-end">{{ s.uptime }}%</td>
                  <td class="text-end">{{ s.downtime }}</td>
                  <td>{{ s.lastMaint }}</td>
                  <td :class="maintUrgency(s.nextMaint)">{{ s.nextMaint }}</td>
                  <td>
                    <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(s)">Edit</button>
                    <button class="btn btn-sm btn-outline-danger" @click="remove(s.id)">Delete</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} System</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option>running</option>
                <option>warning</option>
                <option>standby</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Uptime %</label>
              <input v-model="form.uptime" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Last Maintenance</label>
              <input v-model="form.lastMaint" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Next Maintenance</label>
              <input v-model="form.nextMaint" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Downtime (h/mo)</label>
              <input v-model="form.downtime" type="text" class="form-control" />
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
