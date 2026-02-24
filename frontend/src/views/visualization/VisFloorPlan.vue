<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">Floor Plan Visualization</h1>

    <div class="row mb-4">
      <div class="col-auto">
        <div class="btn-group btn-group-sm" role="group">
          <button type="button" class="btn" :class="selectedHall === 'all' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedHall = 'all'">All Halls</button>
          <button type="button" class="btn" :class="selectedHall === 'A' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedHall = 'A'">Hall A</button>
          <button type="button" class="btn" :class="selectedHall === 'B' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedHall = 'B'">Hall B</button>
          <button type="button" class="btn" :class="selectedHall === 'C' ? 'btn-primary' : 'btn-outline-primary'" @click="selectedHall = 'C'">Hall C</button>
        </div>
      </div>
      <div class="col-auto ms-auto">
        <select class="form-select form-select-sm" style="width: auto;">
          <option value="temp">Temperature</option>
          <option value="power">Power Usage</option>
          <option value="util">Utilization</option>
        </select>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <div class="floor-plan" style="background-color: #f8f9fa; padding: 20px; border-radius: 8px;">
          <div v-for="rack in filteredRacks" :key="rack.id" class="d-inline-block me-2 mb-2">
            <div class="rack-cell" :class="getRackColorClass(rack)"
                 :title="`${rack.id}: ${rack.status}`"
                 style="width: 60px; height: 80px; border: 1px solid #ccc; display: flex; align-items: center; justify-content: center; border-radius: 4px; cursor: pointer; font-weight: bold; color: white;">
              {{ rack.id }}
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer">
        <div class="row">
          <div class="col-auto">
            <span class="badge bg-success me-2">Optimal</span>
            <span class="badge bg-warning me-2">Warning</span>
            <span class="badge bg-danger">Critical</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';

const selectedHall = ref('all');

const racks = [
  { id: 'R01-A', hall: 'A', status: 'Optimal', utilization: 45 },
  { id: 'R02-A', hall: 'A', status: 'Optimal', utilization: 62 },
  { id: 'R03-A', hall: 'A', status: 'Warning', utilization: 78 },
  { id: 'R04-A', hall: 'A', status: 'Optimal', utilization: 55 },
  { id: 'R05-A', hall: 'A', status: 'Optimal', utilization: 48 },
  { id: 'R01-B', hall: 'B', status: 'Optimal', utilization: 70 },
  { id: 'R02-B', hall: 'B', status: 'Critical', utilization: 95 },
  { id: 'R03-B', hall: 'B', status: 'Optimal', utilization: 52 },
  { id: 'R04-B', hall: 'B', status: 'Warning', utilization: 82 },
  { id: 'R01-C', hall: 'C', status: 'Optimal', utilization: 38 },
  { id: 'R02-C', hall: 'C', status: 'Optimal', utilization: 42 },
  { id: 'R03-C', hall: 'C', status: 'Optimal', utilization: 59 }
];

const filteredRacks = computed(() => {
  if (selectedHall.value === 'all') {
    return racks;
  }
  return racks.filter(rack => rack.hall === selectedHall.value);
});

function getRackColorClass(rack: { status: string }): string {
  if (rack.status === 'Optimal') return 'bg-success';
  if (rack.status === 'Warning') return 'bg-warning';
  return 'bg-danger';
}
</script>
