<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { cabinetGroupsApi, type CabinetGroup } from '../../services/api'

const groups = ref<CabinetGroup[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CabinetGroup | null>(null)

const defaultForm = { name: '', racks: '', totalU: 0, usedU: 0, remaining: 0 }
const form = ref({ ...defaultForm })

function utilColor(pct: number): string {
  if (pct >= 90) return 'danger'
  if (pct >= 75) return 'warning'
  return 'success'
}

async function loadData() {
  groups.value = await cabinetGroupsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: CabinetGroup) {
  editing.value = item
  form.value = { name: item.name, racks: item.racks, totalU: item.totalU, usedU: item.usedU, remaining: item.remaining }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await cabinetGroupsApi.update(editing.value.id, form.value)
    } else {
      await cabinetGroupsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await cabinetGroupsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Cabinet Space Remaining</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Cabinet Groups</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Group / Application</th><th>Racks</th><th class="text-end">Total U</th><th class="text-end">Used U</th><th class="text-end">Remaining U</th><th style="width:18%">Utilization</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="g in groups" :key="g.id">
                <td class="fw-semibold">{{ g.name }}</td>
                <td><span class="badge bg-light text-dark border me-1">{{ g.racks }}</span></td>
                <td class="text-end">{{ g.totalU }}</td>
                <td class="text-end">{{ g.usedU }}</td>
                <td class="text-end text-success fw-semibold">{{ g.remaining }}</td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div class="progress flex-fill" style="height: 8px;">
                      <div :class="`progress-bar bg-${utilColor(g.usedU / g.totalU * 100)}`" :style="{ width: (g.usedU / g.totalU * 100) + '%' }"></div>
                    </div>
                    <small>{{ (g.usedU / g.totalU * 100).toFixed(0) }}%</small>
                  </div>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(g)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(g.id)">Delete</button>
                </td>
              </tr>
            </tbody>
            <tfoot class="table-light">
              <tr class="fw-semibold">
                <td>Total</td><td></td>
                <td class="text-end">{{ groups.reduce((a, g) => a + g.totalU, 0) }}</td>
                <td class="text-end">{{ groups.reduce((a, g) => a + g.usedU, 0) }}</td>
                <td class="text-end text-success">{{ groups.reduce((a, g) => a + g.remaining, 0) }}</td>
                <td></td><td></td>
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
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Cabinet Group</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Racks</label>
              <input v-model="form.racks" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Total U</label>
              <input v-model.number="form.totalU" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Used U</label>
              <input v-model.number="form.usedU" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Remaining U</label>
              <input v-model.number="form.remaining" type="number" class="form-control" />
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
