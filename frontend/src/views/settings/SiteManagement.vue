<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Site Management</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Site</button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <template v-else>
    <div class="card">
      <div class="card-body">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Site Name</th>
              <th>Location</th>
              <th>Address</th>
              <th>Contact</th>
              <th>Capacity</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="site in sites" :key="site.id">
              <td class="fw-medium">{{ site.name }}</td>
              <td>{{ site.location }}</td>
              <td class="text-muted text-truncate" :title="site.address">{{ site.address }}</td>
              <td>
                <div class="small">{{ site.contactName }}</div>
                <div class="text-muted small">{{ site.contactPhone }}</div>
              </td>
              <td>
                <div class="small">{{ site.capacity }} kW</div>
              </td>
              <td>
                <span :class="['badge', site.status === 'Active' ? 'bg-success' : site.status === 'Maintenance' ? 'bg-warning' : 'bg-danger']">
                  {{ site.status }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(site)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" @click="remove(site.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Site</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Location</label>
              <input v-model="form.location" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Address</label>
              <input v-model="form.address" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Contact Name</label>
              <input v-model="form.contactName" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Contact Phone</label>
              <input v-model="form.contactPhone" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Capacity (kW)</label>
              <input v-model.number="form.capacity" type="number" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Status</label>
              <select v-model="form.status" class="form-select">
                <option value="Active">Active</option>
                <option value="Maintenance">Maintenance</option>
                <option value="Offline">Offline</option>
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

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { siteConfigsApi, type SiteConfig } from '../../services/api'

const sites = ref<SiteConfig[]>([])
const loading = ref(true)

async function loadData() {
  sites.value = await siteConfigsApi.getAll()
}

onMounted(async () => {
  try {
    await loadData()
  } finally {
    loading.value = false
  }
})
</script>
