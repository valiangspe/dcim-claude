<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">Power Usage Heatmap</h1>

    <div class="row mb-3">
      <div class="col-auto">
        <select class="form-select form-select-sm" style="width: auto;" v-model="selectedTimeRange">
          <option value="hour">Last Hour</option>
          <option value="day">Last 24 Hours</option>
          <option value="week">Last 7 Days</option>
          <option value="month">Last 30 Days</option>
        </select>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table mb-0" style="font-size: 12px;">
            <thead>
              <tr>
                <th style="width: 100px;">Hall / Time</th>
                <th v-for="time in timeSlots" :key="time" style="width: 40px; text-align: center;">
                  {{ time }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="hall in halls" :key="hall">
                <td class="fw-medium">{{ hall }}</td>
                <td v-for="(power, idx) in getHallData(hall)" :key="idx"
                    :style="{ backgroundColor: getHeatmapColor(power), color: power > 50 ? 'white' : 'black', textAlign: 'center', padding: '8px 0' }">
                  {{ power }} kW
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer">
        <div class="d-flex align-items-center gap-3">
          <span class="text-muted small">Scale:</span>
          <div style="width: 20px; height: 20px; background-color: rgb(0, 128, 0); border: 1px solid #ccc;"></div>
          <span class="small">Low (&lt;1000 kW)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 200, 0); border: 1px solid #ccc;"></div>
          <span class="small">Medium (1000-2000 kW)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 100, 0); border: 1px solid #ccc;"></div>
          <span class="small">High (2000-3000 kW)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(200, 0, 0); border: 1px solid #ccc;"></div>
          <span class="small">Critical (&gt;3000 kW)</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';

const selectedTimeRange = ref('day');

const halls = ['Hall A', 'Hall B', 'Hall C'];

const timeSlots = ['00:00', '04:00', '08:00', '12:00', '16:00', '20:00'];

const powerData: Record<string, number[]> = {
  'Hall A': [1245, 1380, 1950, 2150, 2340, 1680],
  'Hall B': [980, 1120, 1680, 2010, 2890, 1520],
  'Hall C': [850, 920, 1250, 1580, 1920, 1100]
};

function getHallData(hall: string): number[] {
  return powerData[hall] || [0, 0, 0, 0, 0, 0];
}

function getHeatmapColor(value: number): string {
  if (value < 1000) return 'rgb(0, 128, 0)';
  if (value < 2000) return 'rgb(255, 200, 0)';
  if (value < 3000) return 'rgb(255, 100, 0)';
  return 'rgb(200, 0, 0)';
}
</script>
