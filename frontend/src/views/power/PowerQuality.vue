<script setup lang="ts">
const phases = [
  { name: 'Phase L1', voltage: 230.4, current: 412, frequency: 50.01, thd: 3.2, pf: 0.97 },
  { name: 'Phase L2', voltage: 229.8, current: 398, frequency: 50.01, thd: 3.8, pf: 0.96 },
  { name: 'Phase L3', voltage: 230.1, current: 405, frequency: 50.00, thd: 3.5, pf: 0.97 },
]

const harmonics = [
  { order: '3rd', l1: 2.1, l2: 2.4, l3: 2.2 },
  { order: '5th', l1: 1.8, l2: 2.0, l3: 1.9 },
  { order: '7th', l1: 0.9, l2: 1.1, l3: 1.0 },
  { order: '9th', l1: 0.4, l2: 0.5, l3: 0.4 },
  { order: '11th', l1: 0.3, l2: 0.3, l3: 0.3 },
]

const events = [
  { time: '2026-02-20 08:14', type: 'Sag', phase: 'L2', duration: '120ms', magnitude: '-12%' },
  { time: '2026-02-19 22:41', type: 'Swell', phase: 'L1', duration: '80ms', magnitude: '+8%' },
  { time: '2026-02-18 14:05', type: 'Sag', phase: 'L3', duration: '200ms', magnitude: '-15%' },
  { time: '2026-02-17 03:22', type: 'Transient', phase: 'L1', duration: '5ms', magnitude: '+22%' },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Power Quality</h4>

    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">Phase Measurements</div>
      <div class="card-body p-0">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr><th>Phase</th><th class="text-end">Voltage (V)</th><th class="text-end">Current (A)</th><th class="text-end">Freq (Hz)</th><th class="text-end">THD %</th><th class="text-end">Power Factor</th></tr>
          </thead>
          <tbody>
            <tr v-for="p in phases" :key="p.name">
              <td class="fw-semibold">{{ p.name }}</td>
              <td class="text-end">{{ p.voltage.toFixed(1) }}</td>
              <td class="text-end">{{ p.current }}</td>
              <td class="text-end">{{ p.frequency.toFixed(2) }}</td>
              <td class="text-end" :class="p.thd > 5 ? 'text-danger' : ''">{{ p.thd.toFixed(1) }}%</td>
              <td class="text-end">{{ p.pf.toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="row g-3">
      <div class="col-lg-5">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">Harmonics (%)</div>
          <div class="card-body p-0">
            <table class="table table-sm table-hover mb-0">
              <thead class="table-light">
                <tr><th>Harmonic</th><th class="text-end">L1</th><th class="text-end">L2</th><th class="text-end">L3</th></tr>
              </thead>
              <tbody>
                <tr v-for="h in harmonics" :key="h.order">
                  <td class="fw-semibold">{{ h.order }}</td>
                  <td class="text-end">{{ h.l1 }}</td>
                  <td class="text-end">{{ h.l2 }}</td>
                  <td class="text-end">{{ h.l3 }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <div class="col-lg-7">
        <div class="card border-0 shadow-sm h-100">
          <div class="card-header bg-transparent fw-semibold">Recent Sag/Swell Events</div>
          <div class="card-body p-0">
            <table class="table table-hover mb-0">
              <thead class="table-light">
                <tr><th>Time</th><th>Type</th><th>Phase</th><th>Duration</th><th>Magnitude</th></tr>
              </thead>
              <tbody>
                <tr v-for="e in events" :key="e.time">
                  <td>{{ e.time }}</td>
                  <td><span class="badge" :class="e.type === 'Sag' ? 'bg-warning text-dark' : e.type === 'Swell' ? 'bg-info text-dark' : 'bg-danger'">{{ e.type }}</span></td>
                  <td>{{ e.phase }}</td>
                  <td>{{ e.duration }}</td>
                  <td class="fw-semibold">{{ e.magnitude }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
