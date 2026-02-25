<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { portCapacityRecordsApi, type PortCapacityRecord } from '../../services/api'

const ports = ref<PortCapacityRecord[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<PortCapacityRecord | null>(null)

const defaultForm = { pduName: '', total: 0, used: 0, reserved: 0 }
const form = ref({ ...defaultForm })

const trend = [
  { month: 'Sep 2025', used: 138 }, { month: 'Oct 2025', used: 141 },
  { month: 'Nov 2025', used: 145 }, { month: 'Dec 2025', used: 148 },
  { month: 'Jan 2026', used: 151 }, { month: 'Feb 2026', used: 154 },
]

const totalPorts = computed(() => ports.value.reduce((a, p) => a + p.total, 0))
const maxTrend = 210

async function loadData() {
  ports.value = await portCapacityRecordsApi.getAll()
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

function openEdit(item: PortCapacityRecord) {
  editing.value = item
  form.value = { pduName: item.pduName, total: item.total, used: item.used, reserved: item.reserved }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await portCapacityRecordsApi.update(editing.value.id, form.value)
    } else {
      await portCapacityRecordsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await portCapacityRecordsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Port Capacity</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3 mb-4">
        <div class="col-lg-7">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
              <span>Power Port Allocation</span>
              <button class="btn btn-primary btn-sm" @click="openCreate">+ Add</button>
            </div>
            <div class="card-body p-0">
              <table class="table table-hover align-middle mb-0">
                <thead class="table-light">
                  <tr><th>PDU</th><th class="text-end">Total</th><th class="text-end">Used</th><th class="text-end">Reserved</th><th class="text-end">Free</th><th style="width:20%">Utilization</th><th>Actions</th></tr>
                </thead>
                <tbody>
                  <tr v-for="p in ports" :key="p.id">
                    <td class="fw-semibold">{{ p.pduName }}</td>
                    <td class="text-end">{{ p.total }}</td>
                    <td class="text-end">{{ p.used }}</td>
                    <td class="text-end">{{ p.reserved }}</td>
                    <td class="text-end">{{ p.total - p.used - p.reserved }}</td>
                    <td>
                      <div class="progress" style="height: 8px;">
                        <div class="progress-bar bg-primary" :style="{ width: (p.used / p.total * 100) + '%' }"></div>
                        <div class="progress-bar bg-warning" :style="{ width: (p.reserved / p.total * 100) + '%' }"></div>
                      </div>
                    </td>
                    <td>
                      <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(p)">Edit</button>
                      <button class="btn btn-sm btn-outline-danger" @click="remove(p.id)">Delete</button>
                    </td>
                  </tr>
                </tbody>
                <tfoot class="table-light">
                  <tr class="fw-semibold">
                    <td>Total</td>
                    <td class="text-end">{{ totalPorts }}</td>
                    <td class="text-end">{{ ports.reduce((a, p) => a + p.used, 0) }}</td>
                    <td class="text-end">{{ ports.reduce((a, p) => a + p.reserved, 0) }}</td>
                    <td class="text-end">{{ ports.reduce((a, p) => a + p.total - p.used - p.reserved, 0) }}</td>
                    <td></td>
                    <td></td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>
        </div>
        <div class="col-lg-5">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-header bg-transparent fw-semibold">Usage Trend</div>
            <div class="card-body p-0">
              <table class="table table-sm table-hover mb-0">
                <thead class="table-light">
                  <tr><th>Month</th><th>Ports Used</th><th style="width:45%">Bar</th></tr>
                </thead>
                <tbody>
                  <tr v-for="t in trend" :key="t.month">
                    <td>{{ t.month }}</td>
                    <td>{{ t.used }}</td>
                    <td>
                      <div class="progress" style="height: 10px;">
                        <div class="progress-bar bg-info" :style="{ width: (t.used / maxTrend * 100) + '%' }"></div>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Port Capacity Record</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">PDU Name</label>
              <input v-model="form.pduName" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Total</label>
              <input v-model.number="form.total" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Used</label>
              <input v-model.number="form.used" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Reserved</label>
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
