<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">Temperature Heatmap</h1>

    <div class="row mb-3">
      <div class="col-auto">
        <select class="form-select form-select-sm" style="width: auto;" v-model="selectedTimeRange">
          <option value="hour">Last Hour</option>
          <option value="day">Last 24 Hours</option>
          <option value="week">Last 7 Days</option>
        </select>
      </div>
    </div>

    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table mb-0" style="font-size: 12px;">
            <thead>
              <tr>
                <th style="width: 120px;">Cabinet / Time</th>
                <th v-for="time in timeSlots" :key="time" style="width: 50px; text-align: center;">
                  {{ time }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="cabinet in cabinets" :key="cabinet">
                <td class="fw-medium">{{ cabinet }}</td>
                <td v-for="(temp, idx) in getTemperatureData(cabinet)" :key="idx"
                    :style="{ backgroundColor: getTempColor(temp), color: 'white', textAlign: 'center', padding: '8px 0', fontWeight: 'bold' }">
                  {{ temp }}°C
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer">
        <div class="d-flex align-items-center gap-3">
          <span class="text-muted small">Temperature Scale:</span>
          <div style="width: 20px; height: 20px; background-color: rgb(0, 0, 255); border: 1px solid #ccc;"></div>
          <span class="small">Cold (&lt;20°C)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(0, 200, 0); border: 1px solid #ccc;"></div>
          <span class="small">Ideal (20-25°C)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 200, 0); border: 1px solid #ccc;"></div>
          <span class="small">Warm (25-30°C)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 0, 0); border: 1px solid #ccc;"></div>
          <span class="small">Critical (&gt;30°C)</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const selectedTimeRange = ref('day');

const cabinets = [
  'CAB-A-01', 'CAB-A-02', 'CAB-A-03',
  'CAB-B-01', 'CAB-B-02', 'CAB-B-03',
  'CAB-C-01', 'CAB-C-02'
];

const timeSlots = ['00:00', '06:00', '12:00', '18:00', '24:00'];

const tempData: Record<string, number[]> = {
  'CAB-A-01': [22.1, 22.5, 23.8, 23.2, 22.4],
  'CAB-A-02': [23.2, 23.5, 24.1, 23.8, 23.1],
  'CAB-A-03': [24.5, 25.1, 26.2, 25.8, 24.3],
  'CAB-B-01': [21.8, 22.2, 23.5, 22.9, 21.6],
  'CAB-B-02': [26.1, 27.2, 28.5, 27.8, 26.2],
  'CAB-B-03': [28.9, 29.5, 31.2, 30.1, 28.5],
  'CAB-C-01': [19.5, 20.1, 21.5, 20.8, 19.2],
  'CAB-C-02': [20.2, 21.1, 22.8, 22.1, 20.5]
};

function getTemperatureData(cabinet: string): number[] {
  return tempData[cabinet] || [0, 0, 0, 0, 0];
}

function getTempColor(temp: number): string {
  if (temp < 20) return 'rgb(0, 0, 255)';
  if (temp < 25) return 'rgb(0, 200, 0)';
  if (temp < 30) return 'rgb(255, 200, 0)';
  return 'rgb(255, 0, 0)';
}
</script>
