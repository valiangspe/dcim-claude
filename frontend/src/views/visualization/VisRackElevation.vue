<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3 mb-0">Rack Elevation View</h1>
      </div>
      <div class="col-auto">
        <select class="form-select form-select-sm" style="width: auto;" v-model="selectedRack">
          <option v-for="rack in rackList" :key="rack" :value="rack">
            {{ rack }}
          </option>
        </select>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-8">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">{{ selectedRack }} - U1 to U48</h5>
          </div>
          <div class="card-body" style="background-color: #f8f9fa;">
            <div class="elevation-view" style="border: 2px solid #333; width: 200px; margin: auto;">
              <div v-for="device in getRackDevices()" :key="device.u"
                   class="device-slot"
                   :class="getDeviceColor(device)"
                   style="height: 30px; border-bottom: 1px solid #999; padding: 4px; font-size: 11px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; color: white; font-weight: bold;">
                U{{ device.u }}: {{ device.name }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Rack Details</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label text-muted">Name</label>
              <p class="fw-medium">{{ selectedRack }}</p>
            </div>
            <div class="mb-3">
              <label class="form-label text-muted">Total U</label>
              <p class="fw-medium">48U</p>
            </div>
            <div class="mb-3">
              <label class="form-label text-muted">Used U</label>
              <p class="fw-medium">38U (79%)</p>
            </div>
            <div class="mb-3">
              <label class="form-label text-muted">Power Supply</label>
              <p class="fw-medium">2x 32A 208V (6.6 kW)</p>
            </div>
            <div class="mb-3">
              <label class="form-label text-muted">Current Temperature</label>
              <p class="fw-medium">23.5°C</p>
            </div>
            <div class="mb-0">
              <label class="form-label text-muted">Status</label>
              <div>
                <span class="badge bg-success">Operational</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const selectedRack = ref('R01-A');

const rackList = ['R01-A', 'R02-A', 'R03-A', 'R04-A', 'R01-B', 'R02-B', 'R03-B'];

const devices: Record<string, Array<{ u: number; name: string; type: string }>> = {
  'R01-A': [
    { u: 48, name: 'PDU 01', type: 'PDU' },
    { u: 46, name: 'Switch 01', type: 'Network' },
    { u: 44, name: 'Firewall', type: 'Network' },
    { u: 42, name: 'Router 01', type: 'Network' },
    { u: 38, name: 'Server 01', type: 'Server' },
    { u: 36, name: 'Server 02', type: 'Server' },
    { u: 34, name: 'Server 03', type: 'Server' },
    { u: 32, name: 'Storage 01', type: 'Storage' },
    { u: 28, name: 'Storage 02', type: 'Storage' }
  ],
  'R02-A': [
    { u: 48, name: 'PDU 02', type: 'PDU' },
    { u: 46, name: 'Switch 02', type: 'Network' },
    { u: 44, name: 'Server 04', type: 'Server' },
    { u: 42, name: 'Server 05', type: 'Server' },
    { u: 40, name: 'Server 06', type: 'Server' }
  ],
  'R03-A': [
    { u: 48, name: 'PDU 03', type: 'PDU' },
    { u: 45, name: 'Server 07', type: 'Server' },
    { u: 43, name: 'Server 08', type: 'Server' },
    { u: 41, name: 'Server 09', type: 'Server' }
  ]
};

function getRackDevices() {
  const rackDevices = devices[selectedRack.value] || devices['R01-A'];
  const filledUs = new Set(rackDevices.map(d => d.u));
  const allSlots = [];
  for (let u = 48; u >= 1; u--) {
    if (filledUs.has(u)) {
      const device = rackDevices.find(d => d.u === u);
      allSlots.push(device!);
    } else {
      allSlots.push({ u, name: 'Empty', type: 'empty' });
    }
  }
  return allSlots;
}

function getDeviceColor(device: { type: string }): string {
  if (device.type === 'empty') return 'bg-light';
  if (device.type === 'Server') return 'bg-primary';
  if (device.type === 'Storage') return 'bg-info';
  if (device.type === 'Network') return 'bg-warning text-dark';
  if (device.type === 'PDU') return 'bg-danger';
  return 'bg-secondary';
}
</script>
