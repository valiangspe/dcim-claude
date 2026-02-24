<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">API Access</h1>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary btn-sm">+ Generate Key</button>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Key Name</th>
                <th>Key ID</th>
                <th>Created</th>
                <th>Last Used</th>
                <th>Permissions</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="key in apiKeys" :key="key.id">
                <td class="fw-medium">{{ key.name }}</td>
                <td class="font-monospace small">{{ key.keyId }}</td>
                <td class="text-muted small">{{ key.created }}</td>
                <td class="text-muted small">{{ key.lastUsed }}</td>
                <td>
                  <span v-for="perm in key.permissions" :key="perm" class="badge bg-secondary me-1">
                    {{ perm }}
                  </span>
                </td>
                <td>
                  <span :class="['badge', key.active ? 'bg-success' : 'bg-secondary']">
                    {{ key.active ? 'Active' : 'Disabled' }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-danger">Revoke</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const apiKeys = [
  {
    id: 1,
    name: 'Production Monitoring',
    keyId: 'key_prod_2024_001_***',
    created: '2024-03-15 10:22 UTC',
    lastUsed: '2026-02-20 14:32 UTC',
    permissions: ['Read', 'Metrics'],
    active: true
  },
  {
    id: 2,
    name: 'Integration Service',
    keyId: 'key_int_2025_001_***',
    created: '2025-01-08 09:15 UTC',
    lastUsed: '2026-02-20 14:35 UTC',
    permissions: ['Read', 'Write', 'Metrics', 'Config'],
    active: true
  },
  {
    id: 3,
    name: 'Third-party Analytics',
    keyId: 'key_ana_2025_002_***',
    created: '2025-06-20 14:00 UTC',
    lastUsed: '2026-02-19 23:45 UTC',
    permissions: ['Read', 'Metrics'],
    active: true
  },
  {
    id: 4,
    name: 'Development Test',
    keyId: 'key_dev_2025_001_***',
    created: '2025-11-10 16:30 UTC',
    lastUsed: '2026-02-10 13:22 UTC',
    permissions: ['Read', 'Write'],
    active: false
  },
  {
    id: 5,
    name: 'Legacy System',
    keyId: 'key_leg_2023_001_***',
    created: '2023-09-01 08:00 UTC',
    lastUsed: '2025-12-15 11:00 UTC',
    permissions: ['Read'],
    active: false
  }
];
</script>
