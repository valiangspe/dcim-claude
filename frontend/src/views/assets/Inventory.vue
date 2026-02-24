<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { assetsApi, type Asset } from '../../services/api'

const classes = ref<Asset[]>([])
const loading = ref(true)
const showModal = ref(false)
const editing = ref<Asset | null>(null)
const saving = ref(false)

const defaultForm = { name: '', class: '', installed: 0, planned: 0, archived: 0 }
const form = ref({ ...defaultForm })

async function loadData() {
  classes.value = await assetsApi.getAll()
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

function openEdit(asset: Asset) {
  editing.value = asset
  form.value = { name: asset.name, class: asset.class, installed: asset.installed, planned: asset.planned, archived: asset.archived }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await assetsApi.update(editing.value.id, form.value)
    } else {
      await assetsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure you want to delete this asset?')) return
  await assetsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Inventory</h4>
      <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Asset</button>
    </div>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
    <div class="card border-0 shadow-sm"><div class="card-body p-0"><table class="table table-hover align-middle mb-0">
      <thead class="table-light"><tr><th>Class</th><th class="text-end">Installed</th><th class="text-end">Planned</th><th class="text-end">Archived</th><th class="text-end">Total</th><th>Actions</th></tr></thead>
      <tbody><tr v-for="c in classes" :key="c.id">
        <td class="fw-semibold">{{ c.name }}</td><td class="text-end text-success">{{ c.installed }}</td><td class="text-end text-warning">{{ c.planned }}</td><td class="text-end text-muted">{{ c.archived }}</td><td class="text-end fw-semibold">{{ c.installed + c.planned + c.archived }}</td>
        <td>
          <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(c)">Edit</button>
          <button class="btn btn-sm btn-outline-danger" @click="remove(c.id)">Delete</button>
        </td>
      </tr></tbody>
      <tfoot class="table-light"><tr class="fw-semibold"><td>Total</td><td class="text-end">{{ classes.reduce((a,c)=>a+c.installed,0) }}</td><td class="text-end">{{ classes.reduce((a,c)=>a+c.planned,0) }}</td><td class="text-end">{{ classes.reduce((a,c)=>a+c.archived,0) }}</td><td class="text-end">{{ classes.reduce((a,c)=>a+c.installed+c.planned+c.archived,0) }}</td><td></td></tr></tfoot>
    </table></div></div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Asset</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Class</label>
              <input v-model="form.class" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Installed</label>
              <input v-model.number="form.installed" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Planned</label>
              <input v-model.number="form.planned" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Archived</label>
              <input v-model.number="form.archived" type="number" class="form-control" />
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
