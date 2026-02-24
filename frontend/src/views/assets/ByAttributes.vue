<script setup lang="ts">
const attrs = [
  { attribute: 'Form Factor', values: [{ val: '1U', count: 68 },{ val: '2U', count: 42 },{ val: '4U', count: 8 },{ val: 'Blade', count: 6 }] },
  { attribute: 'CPU Architecture', values: [{ val: 'x86_64', count: 108 },{ val: 'ARM64', count: 12 },{ val: 'GPU', count: 8 }] },
  { attribute: 'Network Speed', values: [{ val: '10G', count: 84 },{ val: '25G', count: 32 },{ val: '100G', count: 8 }] },
  { attribute: 'Power Redundancy', values: [{ val: 'Dual PSU', count: 98 },{ val: 'Single PSU', count: 26 }] },
]
</script>

<template>
  <div>
    <h4 class="mb-4">Assets by Attributes</h4>
    <div class="row g-3">
      <div v-for="a in attrs" :key="a.attribute" class="col-md-6">
        <div class="card border-0 shadow-sm h-100"><div class="card-header bg-transparent fw-semibold">{{ a.attribute }}</div><div class="card-body">
          <div v-for="v in a.values" :key="v.val" class="mb-2">
            <div class="d-flex justify-content-between mb-1"><span>{{ v.val }}</span><span class="fw-semibold">{{ v.count }}</span></div>
            <div class="progress" style="height:8px"><div class="progress-bar bg-primary" :style="{width:(v.count/130*100)+'%'}"></div></div>
          </div>
        </div></div>
      </div>
    </div>
  </div>
</template>
