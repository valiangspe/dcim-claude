<script setup lang="ts">
const cameras = [
  { id: 1, name: 'Main Entrance', location: 'Data Center A - Floor 1', status: 'online', recording: 'active', resolution: '4K', storage: '68%', lastFrame: '2026-02-20 15:42' },
  { id: 2, name: 'Server Room B', location: 'Data Center B - Floor 2', status: 'online', recording: 'active', resolution: '2K', storage: '45%', lastFrame: '2026-02-20 15:42' },
  { id: 3, name: 'Network Hub', location: 'Data Center A - Floor 3', status: 'online', recording: 'active', resolution: '2K', storage: '52%', lastFrame: '2026-02-20 15:42' },
  { id: 4, name: 'Backup Vault', location: 'Secure Storage - B1', status: 'online', recording: 'active', resolution: '4K', storage: '71%', lastFrame: '2026-02-20 15:42' },
  { id: 5, name: 'UPS Room', location: 'Data Center C - Floor 1', status: 'offline', recording: 'paused', resolution: '1080p', storage: '38%', lastFrame: '2026-02-20 14:15' },
  { id: 6, name: 'Loading Dock', location: 'Facility - Exterior', status: 'online', recording: 'active', resolution: '2K', storage: '59%', lastFrame: '2026-02-20 15:42' },
  { id: 7, name: 'Control Room', location: 'Operations Center', status: 'online', recording: 'active', resolution: '4K', storage: '64%', lastFrame: '2026-02-20 15:42' },
  { id: 8, name: 'Entrance Foyer', location: 'Data Center A - Floor 0', status: 'online', recording: 'active', resolution: '2K', storage: '41%', lastFrame: '2026-02-20 15:42' },
]

const recordingStats = [
  { location: 'Data Center A', cameras: 3, totalStorage: '2TB', usedStorage: '1.34TB', retention: '30 days' },
  { location: 'Data Center B', cameras: 2, totalStorage: '1TB', usedStorage: '0.58TB', retention: '30 days' },
  { location: 'Data Center C', cameras: 1, totalStorage: '512GB', usedStorage: '0.28TB', retention: '30 days' },
  { location: 'Facility', cameras: 2, totalStorage: '1TB', usedStorage: '0.62TB', retention: '30 days' },
]

const statusColor = {
  online: 'success',
  offline: 'danger',
} as const

const recordingColor = {
  active: 'success',
  paused: 'warning',
} as const
</script>

<template>
  <div>
    <h4 class="mb-4">CCTV Integration</h4>

    <!-- System Stats -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-success mb-1">{{ cameras.filter(c => c.status === 'online').length }}/{{ cameras.length }}</h3>
            <small class="text-muted">Cameras Online</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-info mb-1">6.5 TB</h3>
            <small class="text-muted">Total Storage</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-warning mb-1">4.82 TB</h3>
            <small class="text-muted">Used Storage</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-0 shadow-sm">
          <div class="card-body text-center">
            <h3 class="text-primary mb-1">99.8%</h3>
            <small class="text-muted">System Uptime</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Cameras List -->
    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">Camera Status</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Camera Name</th>
              <th>Location</th>
              <th>Status</th>
              <th>Recording</th>
              <th>Resolution</th>
              <th>Storage Used</th>
              <th>Last Frame</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="camera in cameras" :key="camera.id">
              <td class="fw-semibold">{{ camera.name }}</td>
              <td class="text-muted">{{ camera.location }}</td>
              <td><span class="badge" :class="`bg-${statusColor[camera.status as keyof typeof statusColor]}`">{{ camera.status }}</span></td>
              <td><span class="badge" :class="`bg-${recordingColor[camera.recording as keyof typeof recordingColor]}`">{{ camera.recording }}</span></td>
              <td><span class="badge bg-light text-dark">{{ camera.resolution }}</span></td>
              <td>
                <div class="d-flex align-items-center gap-2">
                  <span style="width: 40px;">{{ camera.storage }}</span>
                  <div class="progress" style="width: 80px; height: 6px;">
                    <div class="progress-bar" :class="parseInt(camera.storage) > 70 ? 'bg-danger' : parseInt(camera.storage) > 50 ? 'bg-warning' : 'bg-success'" :style="{ width: camera.storage }"></div>
                  </div>
                </div>
              </td>
              <td class="text-muted">{{ camera.lastFrame }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Storage Stats by Location -->
    <div class="card border-0 shadow-sm">
      <div class="card-header bg-transparent fw-semibold">Storage Statistics by Location</div>
      <div class="card-body p-0">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th>Location</th>
              <th>Cameras</th>
              <th>Total Storage</th>
              <th>Used Storage</th>
              <th>Utilization</th>
              <th>Retention</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="stat in recordingStats" :key="stat.location">
              <td class="fw-semibold">{{ stat.location }}</td>
              <td><span class="badge bg-light text-dark">{{ stat.cameras }}</span></td>
              <td>{{ stat.totalStorage }}</td>
              <td>{{ stat.usedStorage }}</td>
              <td>
                <div class="progress" style="height: 8px;">
                  <div class="progress-bar bg-primary" :style="{ width: ((parseFloat(stat.usedStorage.replace('TB', '')) / parseFloat(stat.totalStorage.replace('TB', '')))) * 100 + '%' }"></div>
                </div>
              </td>
              <td class="text-muted">{{ stat.retention }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
