<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import PlaceholderPage from '../components/PlaceholderPage.vue'

const route = useRoute()

const title = computed(() => (route.meta.title as string) || 'Page')
const module = computed(() => (route.meta.module as string) || '')
const requiredTier = computed(() => (route.meta.requiredTier as 'standard' | 'plus' | 'platinum') || 'standard')
const description = computed(() => (route.meta.description as string) || '')
</script>

<template>
  <PlaceholderPage
    :title="title"
    :module="module"
    :required-tier="requiredTier"
    :description="description"
  />
</template>
