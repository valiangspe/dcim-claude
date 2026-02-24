<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const categories = [
  { name: 'Power', route: '/power', stat: '2,847 kW', status: 'success', description: 'Total facility power consumption' },
  { name: 'Cooling', route: '/cooling', stat: '76% capacity', status: 'warning', description: 'Cooling capacity utilization' },
  { name: 'Space', route: '/space', stat: '68% utilized', status: 'success', description: 'Rack space utilization' },
  { name: 'Network', route: '/network', stat: '99.97% uptime', status: 'success', description: 'Network availability' },
  { name: 'Security', route: '/security', stat: '2 incidents', status: 'warning', description: 'Active security events' },
  { name: 'Environmental', route: '/environmental', stat: '142 T CO2e', status: 'info', description: 'Annual carbon footprint' },
]

function navigate(route: string) {
  router.push(route)
}
</script>

<template>
  <div>
    <h4 class="mb-4">Helicopter View</h4>

    <div class="row g-3">
      <div v-for="cat in categories" :key="cat.name" class="col-sm-6 col-lg-4">
        <div
          class="card border-0 shadow-sm h-100"
          role="button"
          @click="navigate(cat.route)"
          style="cursor: pointer; transition: transform 0.15s;"
          @mouseenter="($event.currentTarget as HTMLElement).style.transform = 'translateY(-2px)'"
          @mouseleave="($event.currentTarget as HTMLElement).style.transform = ''"
        >
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-start mb-2">
              <h5 class="card-title mb-0">{{ cat.name }}</h5>
              <span class="badge" :class="`bg-${cat.status}`">{{ cat.status === 'success' ? 'OK' : cat.status === 'warning' ? 'WARN' : 'INFO' }}</span>
            </div>
            <div class="display-6 fw-bold mb-1">{{ cat.stat }}</div>
            <small class="text-muted">{{ cat.description }}</small>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
