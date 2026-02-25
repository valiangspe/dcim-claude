<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useTier } from '../../composables/useTier'
import { customDashboardsApi, type CustomDashboard } from '../../services/api'

const { hasAccess } = useTier()
const isPlatinum = computed(() => hasAccess('platinum'))

const dashboards = ref<CustomDashboard[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<CustomDashboard | null>(null)

const defaultForm = { name: '', widgets: 0, modified: '', favorite: false }
const form = ref({ ...defaultForm })

const maxDashboards = computed(() => isPlatinum.value ? Infinity : 3)
const canCreate = computed(() => dashboards.value.length < maxDashboards.value)

async function loadData() {
  dashboards.value = await customDashboardsApi.getAll()
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
  form.value = { ...defaultForm, modified: new Date().toISOString().slice(0, 10) }
  showModal.value = true
}

function openEdit(item: CustomDashboard) {
  editing.value = item
  form.value = { name: item.name, widgets: item.widgets, modified: item.modified, favorite: item.favorite }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await customDashboardsApi.update(editing.value.id, form.value)
    } else {
      await customDashboardsApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await customDashboardsApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Custom Dashboards</h4>
      <div class="d-flex align-items-center gap-2">
        <span class="text-muted small">
          {{ dashboards.length }} / {{ isPlatinum ? 'unlimited' : '3' }}
        </span>
        <button class="btn btn-primary btn-sm" :disabled="!canCreate" @click="openCreate">
          + Create New
        </button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="row g-3">
        <div v-for="db in dashboards" :key="db.id" class="col-md-6 col-lg-4">
          <div class="card border-0 shadow-sm h-100">
            <div class="card-body">
              <div class="d-flex justify-content-between align-items-start mb-2">
                <h6 class="card-title mb-0">{{ db.name }}</h6>
                <button class="btn btn-sm btn-link p-0" :class="db.favorite ? 'text-warning' : 'text-muted'">
                  {{ db.favorite ? '\u2605' : '\u2606' }}
                </button>
              </div>
              <div class="text-muted small mb-3">
                {{ db.widgets }} widgets &middot; Modified {{ db.modified }}
              </div>
              <div class="d-flex gap-2">
                <button class="btn btn-outline-primary btn-sm flex-fill">Open</button>
                <button class="btn btn-outline-secondary btn-sm" @click="openEdit(db)">Edit</button>
                <button class="btn btn-outline-danger btn-sm" @click="remove(db.id)">Delete</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-if="!isPlatinum" class="alert alert-info mt-4 mb-0">
        Upgrade to <strong>Platinum</strong> tier for unlimited custom dashboards.
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Dashboard</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Widgets</label>
              <input v-model.number="form.widgets" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Modified</label>
              <input v-model="form.modified" type="text" class="form-control" />
            </div>
            <div class="mb-3 form-check">
              <input v-model="form.favorite" type="checkbox" class="form-check-input" id="favCheck" />
              <label class="form-check-label" for="favCheck">Favorite</label>
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
