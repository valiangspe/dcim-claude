<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">Utilization Heatmap</h1>

    <div class="row mb-3">
      <div class="col-auto">
        <select class="form-select form-select-sm" style="width: auto;" v-model="selectedMetric">
          <option value="cpu">CPU Utilization</option>
          <option value="memory">Memory Utilization</option>
          <option value="disk">Disk Utilization</option>
          <option value="network">Network Utilization</option>
        </select>
      </div>
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
                <th style="width: 120px;">Server / Time</th>
                <th v-for="time in timeSlots" :key="time" style="width: 50px; text-align: center;">
                  {{ time }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="server in servers" :key="server">
                <td class="fw-medium">{{ server }}</td>
                <td v-for="(util, idx) in getUtilizationData(server)" :key="idx"
                    :style="{ backgroundColor: getUtilColor(util), color: 'white', textAlign: 'center', padding: '8px 0', fontWeight: 'bold' }">
                  {{ util }}%
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer">
        <div class="d-flex align-items-center gap-3">
          <span class="text-muted small">Utilization Scale:</span>
          <div style="width: 20px; height: 20px; background-color: rgb(0, 200, 0); border: 1px solid #ccc;"></div>
          <span class="small">Low (&lt;50%)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 200, 0); border: 1px solid #ccc;"></div>
          <span class="small">Medium (50-75%)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 100, 0); border: 1px solid #ccc;"></div>
          <span class="small">High (75-90%)</span>
          <div style="width: 20px; height: 20px; background-color: rgb(255, 0, 0); border: 1px solid #ccc;"></div>
          <span class="small">Critical (&gt;90%)</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const selectedMetric = ref('cpu');
const selectedTimeRange = ref('day');

const servers = [
  'SRV-WEB-01', 'SRV-WEB-02', 'SRV-APP-01', 'SRV-APP-02',
  'SRV-DB-01', 'SRV-DB-02', 'SRV-CACHE-01'
];

const timeSlots = ['00:00', '06:00', '12:00', '18:00', '24:00'];

const utilData: Record<string, number[]> = {
  'SRV-WEB-01': [45, 52, 68, 55, 48],
  'SRV-WEB-02': [42, 48, 65, 52, 45],
  'SRV-APP-01': [65, 72, 85, 78, 68],
  'SRV-APP-02': [62, 68, 82, 75, 65],
  'SRV-DB-01': [78, 82, 92, 88, 80],
  'SRV-DB-02': [75, 80, 89, 85, 78],
  'SRV-CACHE-01': [35, 42, 55, 48, 38]
};

function getUtilizationData(server: string): number[] {
  return utilData[server] || [0, 0, 0, 0, 0];
}

function getUtilColor(util: number): string {
  if (util < 50) return 'rgb(0, 200, 0)';
  if (util < 75) return 'rgb(255, 200, 0)';
  if (util < 90) return 'rgb(255, 100, 0)';
  return 'rgb(255, 0, 0)';
}
</script>
