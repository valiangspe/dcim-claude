<template>
  <div class="request-types">
    <div class="card">
      <div class="card-header bg-info text-white">
        <h5 class="mb-0">Change Request Types</h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div v-for="type in requestTypes" :key="type.id" class="col-md-6">
            <div class="card border-start border-4" :style="{ borderColor: type.color }">
              <div class="card-body">
                <h6 class="card-title fw-bold">{{ type.name }}</h6>
                <p class="card-text text-muted small">{{ type.description }}</p>
                <div class="d-flex justify-content-between align-items-center">
                  <span class="badge" :style="{ backgroundColor: type.color }">
                    {{ type.count }} pending
                  </span>
                  <button class="btn btn-sm btn-outline-primary">Edit</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer d-flex justify-content-between">
        <span class="text-muted">Total request types: {{ requestTypes.length }}</span>
        <button class="btn btn-sm btn-primary">Add Type</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface RequestType {
  id: string
  name: string
  description: string
  color: string
  count: number
}

const requestTypes: RequestType[] = [
  {
    id: '1',
    name: 'Hardware',
    description: 'Server, storage, and equipment changes',
    color: '#0d6efd',
    count: 12
  },
  {
    id: '2',
    name: 'Network',
    description: 'Network configuration and connectivity updates',
    color: '#198754',
    count: 8
  },
  {
    id: '3',
    name: 'Software',
    description: 'Application and operating system updates',
    color: '#fd7e14',
    count: 15
  },
  {
    id: '4',
    name: 'Access',
    description: 'Security and access control modifications',
    color: '#6c757d',
    count: 6
  },
  {
    id: '5',
    name: 'Emergency',
    description: 'Critical and emergency changes',
    color: '#dc3545',
    count: 2
  },
  {
    id: '6',
    name: 'Maintenance',
    description: 'Preventive maintenance and housekeeping',
    color: '#0dcaf0',
    count: 4
  }
]
</script>

<style scoped>
.request-types {
  padding: 1rem;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-2px);
}
</style>
