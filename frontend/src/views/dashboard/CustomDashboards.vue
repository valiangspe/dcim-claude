<script setup lang="ts">
import { computed } from 'vue'
import { useTier } from '../../composables/useTier'

const { hasAccess } = useTier()
const isPlatinum = computed(() => hasAccess('platinum'))

const dashboards = [
  { id: 1, name: 'Power Overview', widgets: 6, modified: '2026-02-19', favorite: true },
  { id: 2, name: 'Cooling Alerts', widgets: 4, modified: '2026-02-18', favorite: false },
  { id: 3, name: 'Executive Summary', widgets: 8, modified: '2026-02-15', favorite: true },
]

const maxDashboards = computed(() => isPlatinum.value ? Infinity : 3)
const canCreate = computed(() => dashboards.length < maxDashboards.value)
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Custom Dashboards</h4>
      <div class="d-flex align-items-center gap-2">
        <span class="text-muted small">
          {{ dashboards.length }} / {{ isPlatinum ? 'unlimited' : '3' }}
        </span>
        <button class="btn btn-primary btn-sm" :disabled="!canCreate">
          + Create New
        </button>
      </div>
    </div>

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
              <button class="btn btn-outline-secondary btn-sm">Edit</button>
              <button class="btn btn-outline-danger btn-sm">Delete</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="!isPlatinum" class="alert alert-info mt-4 mb-0">
      Upgrade to <strong>Platinum</strong> tier for unlimited custom dashboards.
    </div>
  </div>
</template>
