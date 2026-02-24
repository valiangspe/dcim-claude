<script setup lang="ts">
import { useTier, type Tier } from '../composables/useTier'

const { currentTier, setTier } = useTier()

const tiers: { value: Tier; label: string; icon: string }[] = [
  { value: 'standard', label: 'Standard', icon: '🥉' },
  { value: 'plus', label: 'Plus', icon: '🥈' },
  { value: 'platinum', label: 'Platinum', icon: '🥇' },
]
</script>

<template>
  <div class="btn-group btn-group-sm" role="group">
    <button
      v-for="tier in tiers"
      :key="tier.value"
      type="button"
      class="btn"
      :class="currentTier === tier.value ? 'btn-light' : 'btn-outline-light'"
      @click="setTier(tier.value)"
    >
      {{ tier.icon }} {{ tier.label }}
    </button>
  </div>
</template>
