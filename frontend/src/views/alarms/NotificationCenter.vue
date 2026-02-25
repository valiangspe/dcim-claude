<template>
  <div class="container-fluid">
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Notification Center</h1>
      <div>
        <span class="badge bg-danger fs-6 me-2">{{ unreadCount }} Unread</span>
        <button class="btn btn-sm btn-primary" @click="openCreate">+ Add Notification</button>
      </div>
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
                class="btn btn-xs btn-outline-secondary me-1"
                @click="markAsRead(notification.id)"
              >
                Mark Read
              </button>
              <button class="btn btn-xs btn-outline-primary me-1" @click="openEdit(notification)">Edit</button>
              <button class="btn btn-xs btn-outline-danger" @click="remove(notification.id)">Delete</button>
            </div>
          </div>
        </div>
      </div>
    </div>
    </template>

    <!-- Modal -->
    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Notification</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Type</label>
              <select v-model="form.type" class="form-select">
                <option value="Alert">Alert</option>
                <option value="Warning">Warning</option>
                <option value="Info">Info</option>
                <option value="Maintenance">Maintenance</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Message</label>
              <input v-model="form.message" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Time</label>
              <input v-model="form.time" type="text" class="form-control" />
            </div>
            <div class="mb-3 form-check">
              <input v-model="form.read" type="checkbox" class="form-check-input" id="formRead" />
              <label class="form-check-label" for="formRead">Read</label>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { notificationRecordsApi, type NotificationRecord } from '../../services/api'

const notifications = ref<NotificationRecord[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<NotificationRecord | null>(null)
const defaultForm = { type: 'Info', message: '', time: '', read: false }
const form = ref({ ...defaultForm })

async function loadData() {
  notifications.value = await notificationRecordsApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(notification: NotificationRecord) {
  editing.value = notification
  form.value = { type: notification.type, message: notification.message, time: notification.time, read: notification.read }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await notificationRecordsApi.update(editing.value.id, form.value)
    else await notificationRecordsApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await notificationRecordsApi.remove(id)
  await loadData()
}

const unreadCount = computed(() => notifications.value.filter((n) => !n.read).length)

function formatTime(date: string | Date): string {
  const d = typeof date === 'string' ? new Date(date) : date
  const now = new Date()
  const diffMs = now.getTime() - d.getTime()
  const diffMins = Math.floor(diffMs / 60000)

  if (diffMins < 60) return `${diffMins}m ago`
  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours}h ago`
  return d.toLocaleDateString()
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
