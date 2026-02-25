<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { siteNodesApi, type SiteNode } from '../../services/api'

const flatNodes = ref<SiteNode[]>([])
const loading = ref(true)
const showModal = ref(false)
const saving = ref(false)
const editing = ref<SiteNode | null>(null)

const defaultForm = { name: '', type: 'Site', parentId: null as number | null }
const form = ref({ ...defaultForm })

interface TreeNode {
  name: string
  type: string
  children?: TreeNode[]
}

const hierarchy = computed((): TreeNode[] => {
  const nodeMap = new Map<number, TreeNode & { id: number; parentId: number | null }>()
  for (const n of flatNodes.value) {
    nodeMap.set(n.id, { ...n, children: [] })
  }
  const roots: TreeNode[] = []
  for (const n of nodeMap.values()) {
    if (n.parentId === null) {
      roots.push(n)
    } else {
      const parent = nodeMap.get(n.parentId)
      if (parent) parent.children!.push(n)
    }
  }
  return roots
})

function typeBadge(type: string): string {
  switch (type) {
    case 'Site': return 'bg-primary'
    case 'Building': return 'bg-success'
    case 'Hall': return 'bg-info text-dark'
    case 'Row': return 'bg-secondary'
    default: return 'bg-dark'
  }
}

async function loadData() {
  flatNodes.value = await siteNodesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: SiteNode) {
  editing.value = item
  form.value = { name: item.name, type: item.type, parentId: item.parentId }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) {
      await siteNodesApi.update(editing.value.id, form.value)
    } else {
      await siteNodesApi.create(form.value)
    }
    showModal.value = false
    await loadData()
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('Are you sure?')) return
  await siteNodesApi.remove(id)
  await loadData()
}
</script>

<template>
  <div>
    <h4 class="mb-4">Site Hierarchy</h4>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <template v-else>
      <div class="card border-0 shadow-sm mb-4">
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

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-transparent fw-semibold d-flex justify-content-between align-items-center">
          <span>All Nodes</span>
          <button class="btn btn-primary btn-sm" @click="openCreate">+ Add Node</button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr><th>Name</th><th>Type</th><th class="text-end">Parent ID</th><th>Actions</th></tr>
            </thead>
            <tbody>
              <tr v-for="n in flatNodes" :key="n.id">
                <td class="fw-semibold">{{ n.name }}</td>
                <td><span class="badge" :class="typeBadge(n.type)">{{ n.type }}</span></td>
                <td class="text-end">{{ n.parentId ?? '—' }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-secondary me-1" @click="openEdit(n)">Edit</button>
                  <button class="btn btn-sm btn-outline-danger" @click="remove(n.id)">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Node</h5>
            <button type="button" class="btn-close" @click="showModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input v-model="form.name" type="text" class="form-control" />
            </div>
            <div class="mb-3">
              <label class="form-label">Type</label>
              <select v-model="form.type" class="form-select">
                <option>Site</option>
                <option>Building</option>
                <option>Hall</option>
                <option>Row</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Parent ID (leave empty for root)</label>
              <input v-model.number="form.parentId" type="number" class="form-control" />
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
            <button class="btn btn-primary" @click="save" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
