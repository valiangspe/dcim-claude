<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useTier } from '../composables/useTier'
import { menuItems } from '../config/menuItems'

const router = useRouter()
const { hasAccess } = useTier()

const expandedMenus = ref<Record<string, boolean>>({})

function toggleMenu(label: string) {
  expandedMenus.value[label] = !expandedMenus.value[label]
}

function navigate(route: string | undefined, tier: string) {
  if (route && hasAccess(tier as 'standard' | 'plus' | 'platinum')) {
    router.push(route)
  }
}

function tierBadge(tier: string): string {
  if (tier === 'plus') return 'Plus'
  if (tier === 'platinum') return 'Platinum'
  return ''
}
</script>

<template>
  <nav class="sidebar bg-dark text-white d-flex flex-column flex-shrink-0" style="width: 260px; overflow-y: auto;">
    <div class="p-3 border-bottom border-secondary">
      <h5 class="mb-0 fw-bold">⚡ DCIM Pro</h5>
      <small class="text-secondary">Infrastructure Management</small>
    </div>

    <ul class="nav flex-column mt-2">
      <li v-for="item in menuItems" :key="item.label" class="nav-item">
        <!-- Parent menu item -->
        <a
          class="nav-link d-flex align-items-center px-3 py-2"
          :class="{
            'text-white': hasAccess(item.tier),
            'text-secondary': !hasAccess(item.tier),
          }"
          href="#"
          @click.prevent="item.children ? toggleMenu(item.label) : navigate(item.route, item.tier)"
        >
          <span class="me-2">{{ item.icon }}</span>
          <span class="flex-grow-1">{{ item.label }}</span>
          <span v-if="!hasAccess(item.tier)" class="badge bg-secondary ms-1" style="font-size: 0.6rem;">🔒</span>
          <span
            v-if="item.children"
            class="ms-auto small"
          >{{ expandedMenus[item.label] ? '▾' : '▸' }}</span>
        </a>

        <!-- Submenu -->
        <ul
          v-if="item.children && expandedMenus[item.label]"
          class="nav flex-column ms-3 border-start border-secondary"
        >
          <li v-for="child in item.children" :key="child.label" class="nav-item">
            <a
              class="nav-link py-1 px-3 small"
              :class="{
                'text-light': hasAccess(child.tier),
                'text-secondary': !hasAccess(child.tier),
              }"
              href="#"
              style="cursor: pointer;"
              @click.prevent="navigate(child.route, child.tier)"
            >
              {{ child.label }}
              <span
                v-if="!hasAccess(child.tier)"
                class="badge ms-1"
                :class="child.tier === 'platinum' ? 'bg-warning text-dark' : 'bg-primary'"
                style="font-size: 0.55rem;"
              >{{ tierBadge(child.tier) }}</span>
            </a>
          </li>
        </ul>
      </li>
    </ul>
  </nav>
</template>
