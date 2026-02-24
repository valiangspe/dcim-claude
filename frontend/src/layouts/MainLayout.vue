<script setup lang="ts">
import { ref } from 'vue'
import SidebarMenu from '../components/SidebarMenu.vue'
import TierSwitcher from '../components/TierSwitcher.vue'

const sidebarOpen = ref(true)
</script>

<template>
  <div class="d-flex vh-100">
    <!-- Sidebar -->
    <SidebarMenu v-if="sidebarOpen" />

    <!-- Main content -->
    <div class="flex-grow-1 d-flex flex-column overflow-hidden">
      <!-- Top navbar -->
      <nav class="navbar navbar-dark bg-dark border-bottom border-secondary px-3 flex-shrink-0">
        <button class="btn btn-outline-light btn-sm me-3" @click="sidebarOpen = !sidebarOpen">
          ☰
        </button>
        <span class="navbar-brand mb-0">DCIM</span>
        <div class="d-flex align-items-center ms-auto gap-3">
          <TierSwitcher />
          <span class="text-light" style="cursor: pointer;" title="Notifications">🔔</span>
          <div class="dropdown">
            <a class="text-light text-decoration-none dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
              👤 Admin
            </a>
          </div>
        </div>
      </nav>

      <!-- Page content -->
      <main class="p-4 bg-light flex-grow-1 overflow-auto">
        <router-view />
      </main>
    </div>
  </div>
</template>
