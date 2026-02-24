<script setup lang="ts">
import { useTier } from '../composables/useTier'

const props = defineProps<{
  title: string
  module: string
  requiredTier?: 'standard' | 'plus' | 'platinum'
  description?: string
}>()

const { hasAccess, currentTier } = useTier()
</script>

<template>
  <div>
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb">
      <ol class="breadcrumb">
        <li class="breadcrumb-item">
          <router-link to="/dashboard">Home</router-link>
        </li>
        <li class="breadcrumb-item">{{ module }}</li>
        <li class="breadcrumb-item active">{{ title }}</li>
      </ol>
    </nav>

    <!-- Header -->
    <div class="d-flex align-items-center mb-4">
      <h3 class="mb-0">{{ title }}</h3>
      <span
        v-if="requiredTier && requiredTier !== 'standard'"
        class="badge ms-2"
        :class="requiredTier === 'platinum' ? 'bg-warning text-dark' : 'bg-primary'"
      >
        {{ requiredTier }}
      </span>
    </div>

    <!-- Locked message -->
    <div v-if="requiredTier && !hasAccess(requiredTier)" class="card border-warning">
      <div class="card-body text-center py-5">
        <h1 class="mb-3">🔒</h1>
        <h5>Feature Locked</h5>
        <p class="text-muted">
          This feature requires the <strong>{{ requiredTier }}</strong> tier.
          You are currently on <strong>{{ currentTier }}</strong>.
        </p>
        <button class="btn btn-warning btn-sm">Upgrade to {{ requiredTier }}</button>
      </div>
    </div>

    <!-- Placeholder content -->
    <div v-else class="card">
      <div class="card-body text-center py-5">
        <h1 class="mb-3 text-muted">🚧</h1>
        <h5>{{ title }}</h5>
        <p class="text-muted">{{ description || 'This module is under development.' }}</p>
      </div>
    </div>
  </div>
</template>
