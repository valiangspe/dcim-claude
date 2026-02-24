<template>
  <div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Notification Center</h1>
      <span class="badge bg-danger fs-6">{{ unreadCount }} Unread</span>
    </div>

    <!-- Action Buttons -->
    <div class="mb-4">
      <button class="btn btn-sm btn-outline-primary me-2">Mark All as Read</button>
      <button class="btn btn-sm btn-outline-secondary">Clear Old</button>
    </div>

    <!-- Notifications List -->
    <div class="card">
      <div class="card-header">
        <h5 class="mb-0">Recent Notifications</h5>
      </div>
      <div class="list-group list-group-flush">
        <div
          v-for="notification in notifications"
          :key="notification.id"
          :class="['list-group-item', { 'bg-light': !notification.read }]"
        >
          <div class="d-flex justify-content-between align-items-start">
            <div class="flex-grow-1">
              <div class="mb-2">
                <span :class="getTypeBadgeClass(notification.type)">
                  {{ notification.type }}
                </span>
                <span v-if="!notification.read" class="badge bg-primary ms-2">NEW</span>
              </div>
              <p class="mb-2 fw-500">{{ notification.message }}</p>
              <small class="text-muted">{{ formatTime(notification.time) }}</small>
            </div>
            <div class="ms-3">
              <button
                v-if="!notification.read"
                class="btn btn-xs btn-outline-secondary"
                @click="markAsRead(notification.id)"
              >
                Mark Read
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'

interface Notification {
  id: number
  type: 'Alert' | 'Warning' | 'Info' | 'Maintenance'
  message: string
  time: Date
  read: boolean
}

const notifications = ref<Notification[]>([
  {
    id: 1,
    type: 'Alert',
    message: 'Critical: Temperature in Rack A-01 exceeded 42°C',
    time: new Date(Date.now() - 2 * 60000),
    read: false,
  },
  {
    id: 2,
    type: 'Alert',
    message: 'High: Power consumption anomaly in PDU-B-03',
    time: new Date(Date.now() - 8 * 60000),
    read: false,
  },
  {
    id: 3,
    type: 'Warning',
    message: 'Cooling Unit C-02 refrigerant pressure low',
    time: new Date(Date.now() - 30 * 60000),
    read: true,
  },
  {
    id: 4,
    type: 'Info',
    message: 'Scheduled maintenance window completed successfully',
    time: new Date(Date.now() - 2 * 60 * 60 * 1000),
    read: true,
  },
  {
    id: 5,
    type: 'Warning',
    message: 'Network Switch D-01 port utilization at 88%',
    time: new Date(Date.now() - 4 * 60 * 60 * 1000),
    read: true,
  },
  {
    id: 6,
    type: 'Maintenance',
    message: 'Scheduled maintenance: UPS battery replacement on Feb 25',
    time: new Date(Date.now() - 24 * 60 * 60 * 1000),
    read: true,
  },
])

const unreadCount = computed(() => notifications.value.filter((n) => !n.read).length)

function formatTime(date: Date): string {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)

  if (diffMins < 60) return `${diffMins}m ago`
  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours}h ago`
  return date.toLocaleDateString()
}

function getTypeBadgeClass(type: string): string {
  const classes: Record<string, string> = {
    Alert: 'badge bg-danger',
    Warning: 'badge bg-warning text-dark',
    Info: 'badge bg-info',
    Maintenance: 'badge bg-secondary',
  }
  return classes[type] || 'badge bg-secondary'
}

function markAsRead(id: number): void {
  const notification = notifications.value.find((n) => n.id === id)
  if (notification) {
    notification.read = true
  }
}
</script>
