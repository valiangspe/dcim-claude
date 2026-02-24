<script setup lang="ts">
interface SiteNode {
  name: string
  type: string
  children?: SiteNode[]
}

const hierarchy: SiteNode[] = [
  { name: 'DC-East-01', type: 'Site', children: [
    { name: 'Building A', type: 'Building', children: [
      { name: 'Hall A', type: 'Hall', children: [
        { name: 'Row 1 (A01–A04)', type: 'Row' },
        { name: 'Row 2 (A05–A08)', type: 'Row' },
      ]},
      { name: 'Hall B', type: 'Hall', children: [
        { name: 'Row 1 (B01–B03)', type: 'Row' },
        { name: 'Row 2 (B04–B06)', type: 'Row' },
      ]},
    ]},
    { name: 'Building B', type: 'Building', children: [
      { name: 'Hall C', type: 'Hall', children: [
        { name: 'Row 1 (C01–C02)', type: 'Row' },
      ]},
      { name: 'Hall D', type: 'Hall', children: [
        { name: 'Row 1 (D01–D03)', type: 'Row' },
      ]},
    ]},
  ]},
  { name: 'DC-West-01', type: 'Site', children: [
    { name: 'Main Building', type: 'Building', children: [
      { name: 'Hall E', type: 'Hall', children: [
        { name: 'Row 1 (E01–E06)', type: 'Row' },
        { name: 'Row 2 (E07–E12)', type: 'Row' },
      ]},
    ]},
  ]},
]

function typeBadge(type: string): string {
  switch (type) {
    case 'Site': return 'bg-primary'
    case 'Building': return 'bg-success'
    case 'Hall': return 'bg-info text-dark'
    case 'Row': return 'bg-secondary'
    default: return 'bg-dark'
  }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Site Hierarchy</h4>

    <div class="card border-0 shadow-sm">
      <div class="card-body">
        <ul class="list-unstyled mb-0">
          <li v-for="site in hierarchy" :key="site.name" class="mb-3">
            <div class="d-flex align-items-center gap-2 mb-1">
              <span class="badge" :class="typeBadge(site.type)">{{ site.type }}</span>
              <strong>{{ site.name }}</strong>
            </div>
            <ul v-if="site.children" class="list-unstyled ms-4">
              <li v-for="building in site.children" :key="building.name" class="mb-2">
                <div class="d-flex align-items-center gap-2 mb-1">
                  <span class="badge" :class="typeBadge(building.type)">{{ building.type }}</span>
                  <span class="fw-semibold">{{ building.name }}</span>
                </div>
                <ul v-if="building.children" class="list-unstyled ms-4">
                  <li v-for="hall in building.children" :key="hall.name" class="mb-1">
                    <div class="d-flex align-items-center gap-2 mb-1">
                      <span class="badge" :class="typeBadge(hall.type)">{{ hall.type }}</span>
                      <span>{{ hall.name }}</span>
                    </div>
                    <ul v-if="hall.children" class="list-unstyled ms-4">
                      <li v-for="row in hall.children" :key="row.name">
                        <div class="d-flex align-items-center gap-2">
                          <span class="badge" :class="typeBadge(row.type)">{{ row.type }}</span>
                          <span class="text-muted">{{ row.name }}</span>
                        </div>
                      </li>
                    </ul>
                  </li>
                </ul>
              </li>
            </ul>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>
