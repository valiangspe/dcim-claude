<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { assetAttributesApi, type AssetAttribute } from '../../services/api'

const attrs = ref<AssetAttribute[]>([])
const loading = ref(true)

const showModal = ref(false)
const saving = ref(false)
const editing = ref<AssetAttribute | null>(null)
const deleting = ref<number | null>(null)
const defaultForm = { attribute: '', value: '', count: 0 }
const form = ref({ ...defaultForm })

const grouped = computed(() => {
  const map = new Map<string, { val: string; count: number }[]>()
  for (const a of attrs.value) {
    if (!map.has(a.attribute)) map.set(a.attribute, [])
    map.get(a.attribute)!.push({ val: a.value, count: a.count })
  }
  return Array.from(map.entries()).map(([attribute, values]) => ({ attribute, values }))
})

async function loadData() {
  attrs.value = await assetAttributesApi.getAll()
}

onMounted(async () => { try { await loadData() } finally { loading.value = false } })

function openCreate() {
  editing.value = null
  form.value = { ...defaultForm }
  showModal.value = true
}

function openEdit(item: AssetAttribute) {
  editing.value = item
  form.value = { attribute: item.attribute, value: item.value, count: item.count }
  showModal.value = true
}

async function save() {
  saving.value = true
  try {
    if (editing.value) await assetAttributesApi.update(editing.value.id, form.value)
    else await assetAttributesApi.create(form.value)
    showModal.value = false
    await loadData()
  } finally { saving.value = false }
}

async function remove(item: AssetAttribute) {
  if (!confirm(`Delete attribute "${item.attribute}: ${item.value}"?`)) return
  deleting.value = item.id
  try { await assetAttributesApi.remove(item.id); await loadData() } finally { deleting.value = null }
}
</script>

<template>
  <div>
    <h4 class="mb-4">Assets by Attributes</h4>
    <div v-if="loading" class="text-center py-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>
    <template v-else>
    <div class="d-flex justify-content-end mb-3"><button class="btn btn-sm btn-primary" @click="openCreate">+ Add</button></div>
    <div class="card border-0 shadow-sm mb-4">
      <div class="card-header bg-transparent fw-semibold">All Attributes</div>
      <div class="card-body p-0"><table class="table table-hover align-middle mb-0">
        <thead class="table-light"><tr><th>Attribute</th><th>Value</th><th class="text-end">Count</th><th style="width:120px">Actions</th></tr></thead>
        <tbody><tr v-for="a in attrs" :key="a.id">
          <td class="fw-semibold">{{ a.attribute }}</td><td>{{ a.value }}</td><td class="text-end">{{ a.count }}</td>
          <td>
            <button class="btn btn-sm btn-outline-primary me-1" @click="openEdit(a)">Edit</button>
            <button class="btn btn-sm btn-outline-danger" @click="remove(a)" :disabled="deleting === a.id">
              <span v-if="deleting === a.id" class="spinner-border spinner-border-sm"></span><span v-else>Delete</span>
            </button>
          </td>
        </tr></tbody>
      </table></div>
    </div>
    <div class="row g-3">
      <div v-for="a in grouped" :key="a.attribute" class="col-md-6">
        <div class="card border-0 shadow-sm h-100"><div class="card-header bg-transparent fw-semibold">{{ a.attribute }}</div><div class="card-body">
          <div v-for="v in a.values" :key="v.val" class="mb-2">
            <div class="d-flex justify-content-between mb-1"><span>{{ v.val }}</span><span class="fw-semibold">{{ v.count }}</span></div>
            <div class="progress" style="height:8px"><div class="progress-bar bg-primary" :style="{width:(v.count/130*100)+'%'}"></div></div>
          </div>
        </div></div>
      </div>
    </div>
    </template>

    <div v-if="showModal" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.5)">
      <div class="modal-dialog"><div class="modal-content">
        <div class="modal-header"><h5 class="modal-title">{{ editing ? 'Edit' : 'Add' }} Attribute</h5><button type="button" class="btn-close" @click="showModal = false"></button></div>
        <div class="modal-body">
          <div class="mb-3"><label class="form-label">Attribute</label><input v-model="form.attribute" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Value</label><input v-model="form.value" type="text" class="form-control" /></div>
          <div class="mb-3"><label class="form-label">Count</label><input v-model.number="form.count" type="number" class="form-control" /></div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showModal = false">Cancel</button>
          <button class="btn btn-primary" @click="save" :disabled="saving"><span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>Save</button>
        </div>
      </div></div>
    </div>
  </div>
</template>
