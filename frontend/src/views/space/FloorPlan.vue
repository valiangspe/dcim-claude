<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { floorPlanRacksApi, type FloorPlanRack } from '../../services/api'

const allRacks = ref<FloorPlanRack[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<FloorPlanRack | null>(null)

const defaultForm = { rackId: '', rowLabel: '', position: 1, utilization: 0 }
const form = ref({ ...defaultForm })

const rows = computed(() => {
  const grouped = new Map<string, ({ id: string; util: number } | null)[]>()
  for (const r of allRacks.value) {
    if (!grouped.has(r.rowLabel)) grouped.set(r.rowLabel, [])
    const row = grouped.get(r.rowLabel)!
    while (row.length < r.position - 1) row.push(null)
    row[r.position - 1] = { id: r.rackId, util: r.utilization }
  }
  return Array.from(grouped.entries()).map(([label, racks]) => ({ label, racks }))
})

function cellBg(util: number): string {
  if (util >= 90) return 'bg-danger text-white'
  if (util >= 70) return 'bg-warning text-dark'
  if (util >= 40) return 'bg-info text-dark'
  return 'bg-success text-white'
}

async function loadData() {
  allRacks.value = await floorPlanRacksApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: FloorPlanRack) {
  editing.value = item
  form.value = { rackId: item.rackId, rowLabel: item.rowLabel, position: item.position, utilization: item.utilization }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await floorPlanRacksApi.update(editing.value.id, form.value)
    } else {
      await floorPlanRacksApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await floorPlanRacksApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Floor Plan</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>Hall A & B — Floor Layout</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Rack</button>
        </div>
        <div class="card-body">
          <div class="table-responsive">
            <table class="table table-bordered text-center mb-0" style="table-layout: fixed;">
              <thead>
                <tr class="table-light">
                  <th style="width:70px"></th>
                  <th v-for="i in 8" :key="i">Pos {{ i }}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in rows" :key="row.label">
                  <td class="fw-semibold table-light align-middle">{{ row.label }}</td>
                  <td v-for="(rack, i) in row.racks" :key="i" :class="rack ? cellBg(rack.util) : 'bg-light'" style="height: 60px; vertical-align: middle;">
                    <template v-if="rack">
                      <div class="fw-semibold small">{{ rack.id }}</div>
                      <div class="small">{{ rack.util }}%</div>
                    </template>
                    <template v-else>
                      <span class="text-muted small">—</span>
                    </template>
                  </td>
                </tr>
                <tr>
                  <td class="table-secondary fw-semibold" colspan="5">Cold Aisle</td>
                  <td class="table-secondary fw-semibold" colspan="4">Cold Aisle</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm mb-4">
        <div class="card-header bg-transparent fw-semibold">All Floor Plan Racks</div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Rack ID</th><th>Row</th><th class="text-end">Position</th><th class="text-end">Utilization %</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="r in allRacks" :key="r.id">
                <td class="fw-semibold">{{ r.rackId }}</td>
                <td>{{ r.rowLabel }}</td>
                <td class="text-end">{{ r.position }}</td>
                <td class="text-end">{{ r.utilization }}%</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(r)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(r.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="d-flex gap-3 small">
        <span><span class="badge bg-success">&nbsp;&nbsp;</span> &lt;40%</span>
        <span><span class="badge bg-info">&nbsp;&nbsp;</span> 40-70%</span>
        <span><span class="badge bg-warning">&nbsp;&nbsp;</span> 70-90%</span>
        <span><span class="badge bg-danger">&nbsp;&nbsp;</span> &ge;90%</span>
        <span><span class="badge bg-light text-muted border">&nbsp;&nbsp;</span> Empty</span>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Floor Plan Rack</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Rack ID</label>
              <input v-model="form.rackId" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Row Label</label>
              <input v-model="form.rowLabel" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Position</label>
              <input v-model.number="form.position" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Utilization (%)</label>
              <input v-model.number="form.utilization" type="number" class="form-control" />
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
