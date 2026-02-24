import { ref, computed } from 'vue'

export type Tier = 'standard' | 'plus' | 'platinum'

const TIER_LEVELS: Record<Tier, number> = {
  standard: 0,
  plus: 1,
  platinum: 2,
}

const currentTier = ref<Tier>('platinum')

export function useTier() {
  function setTier(tier: Tier) {
    currentTier.value = tier
  }

  function hasAccess(requiredTier: Tier): boolean {
    return TIER_LEVELS[currentTier.value] >= TIER_LEVELS[requiredTier]
  }

  const tierLabel = computed(() => {
    return currentTier.value.charAt(0).toUpperCase() + currentTier.value.slice(1)
  })

  const tierBadgeClass = computed(() => {
    const map: Record<Tier, string> = {
      standard: 'bg-secondary',
      plus: 'bg-primary',
      platinum: 'bg-warning text-dark',
    }
    return map[currentTier.value]
  })

  return {
    currentTier,
    setTier,
    hasAccess,
    tierLabel,
    tierBadgeClass,
  }
}
