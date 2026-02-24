<script setup lang="ts">
import { computed } from 'vue'
import { useTier, type Tier } from '../composables/useTier'

const props = defineProps<{
  requiredTier: Tier
}>()

const { hasAccess, currentTier } = useTier()
const allowed = computed(() => hasAccess(props.requiredTier))
</script>

<template>
  <slot v-if="allowed" />
  <slot v-else name="locked">
    <div class="alert alert-secondary d-flex align-items-center" role="alert">
      <span class="me-2">🔒</span>
      <span>
        This feature requires the <strong>{{ requiredTier }}</strong> tier.
        You are currently on <strong>{{ currentTier }}</strong>.
      </span>
    </div>
  </slot>
</template>
