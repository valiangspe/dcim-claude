<script setup lang="ts">
const servers = ['srv-web-01','srv-web-02','srv-db-01','srv-db-02','srv-gpu-01','srv-gpu-02','srv-app-01','srv-stor-01']
const hours = ['00','04','08','12','16','20']
const data: number[][] = [
  [52,48,62,72,68,58],[42,38,50,60,55,45],[68,64,78,88,82,72],[65,60,74,84,78,70],
  [88,85,92,95,93,90],[86,82,90,94,91,88],[32,28,42,52,48,38],[18,15,22,28,24,20],
]
function cellColor(v: number) { return v>=85?'bg-danger text-white':v>=70?'bg-warning text-dark':v>=50?'bg-info text-dark':'bg-success text-white' }
</script>

<template>
  <div>
    <h4 class="mb-4">Server Heatmap</h4>
    <p class="text-muted">CPU utilization (%) by server and time of day.</p>
    <div class="card border-0 shadow-sm mb-3"><div class="card-body p-0"><table class="table table-bordered text-center mb-0">
      <thead class="table-light"><tr><th></th><th v-for="h in hours" :key="h">{{ h }}:00</th></tr></thead>
      <tbody><tr v-for="(row, i) in data" :key="servers[i]">
        <td class="fw-semibold table-light text-start">{{ servers[i] }}</td>
        <td v-for="(v, j) in row" :key="j" :class="cellColor(v)">{{ v }}</td>
      </tr></tbody>
    </table></div></div>
    <div class="d-flex gap-3 small"><span><span class="badge bg-success">&nbsp;&nbsp;</span> &lt;50%</span><span><span class="badge bg-info">&nbsp;&nbsp;</span> 50-70%</span><span><span class="badge bg-warning">&nbsp;&nbsp;</span> 70-85%</span><span><span class="badge bg-danger">&nbsp;&nbsp;</span> &ge;85%</span></div>
  </div>
</template>
