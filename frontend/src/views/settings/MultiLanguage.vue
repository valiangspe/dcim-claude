<template>
  <div class="container-fluid py-4">
    <h1 class="h3 mb-4">Multi-Language Support</h1>

    <div class="card">
      <div class="card-body">
        <table class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Language</th>
              <th>Code</th>
              <th>Native Name</th>
              <th>Strings Translated</th>
              <th>Completion %</th>
              <th>Status</th>
              <th>Last Updated</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="language in languages" :key="language.id">
              <td class="fw-medium">{{ language.name }}</td>
              <td class="font-monospace">{{ language.code }}</td>
              <td>{{ language.native }}</td>
              <td>
                <span class="badge bg-light text-dark">{{ language.translated }}/{{ language.total }}</span>
              </td>
              <td>
                <div class="progress" style="height: 20px; width: 120px;">
                  <div class="progress-bar" :class="getProgressClass(language.completion)"
                       :style="{ width: language.completion + '%' }">
                    {{ language.completion }}%
                  </div>
                </div>
              </td>
              <td>
                <span :class="['badge', getStatusBadge(language.completion)]">
                  {{ getStatusText(language.completion) }}
                </span>
              </td>
              <td class="text-muted small">{{ language.lastUpdated }}</td>
              <td>
                <button class="btn btn-sm btn-outline-secondary">Edit</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const languages = [
  {
    id: 1,
    name: 'English',
    code: 'en-US',
    native: 'English (United States)',
    translated: 1245,
    total: 1245,
    completion: 100,
    lastUpdated: '2026-02-15'
  },
  {
    id: 2,
    name: 'German',
    code: 'de-DE',
    native: 'Deutsch',
    translated: 1245,
    total: 1245,
    completion: 100,
    lastUpdated: '2026-02-10'
  },
  {
    id: 3,
    name: 'French',
    code: 'fr-FR',
    native: 'Français',
    translated: 1230,
    total: 1245,
    completion: 99,
    lastUpdated: '2026-02-18'
  },
  {
    id: 4,
    name: 'Spanish',
    code: 'es-ES',
    native: 'Español',
    translated: 1200,
    total: 1245,
    completion: 96,
    lastUpdated: '2026-02-12'
  },
  {
    id: 5,
    name: 'Japanese',
    code: 'ja-JP',
    native: '日本語',
    translated: 890,
    total: 1245,
    completion: 71,
    lastUpdated: '2026-02-05'
  },
  {
    id: 6,
    name: 'Simplified Chinese',
    code: 'zh-CN',
    native: '简体中文',
    translated: 780,
    total: 1245,
    completion: 63,
    lastUpdated: '2026-01-28'
  },
  {
    id: 7,
    name: 'Korean',
    code: 'ko-KR',
    native: '한국어',
    translated: 450,
    total: 1245,
    completion: 36,
    lastUpdated: '2026-01-15'
  },
  {
    id: 8,
    name: 'Portuguese',
    code: 'pt-BR',
    native: 'Português (Brasil)',
    translated: 250,
    total: 1245,
    completion: 20,
    lastUpdated: '2026-01-10'
  }
];

function getProgressClass(completion: number): string {
  if (completion === 100) return 'bg-success';
  if (completion >= 90) return 'bg-info';
  if (completion >= 70) return 'bg-warning';
  return 'bg-danger';
}

function getStatusBadge(completion: number): string {
  if (completion === 100) return 'bg-success';
  if (completion >= 90) return 'bg-info';
  if (completion >= 70) return 'bg-warning';
  return 'bg-danger';
}

function getStatusText(completion: number): string {
  if (completion === 100) return 'Complete';
  if (completion >= 90) return 'Nearly Complete';
  if (completion >= 70) return 'In Progress';
  return 'Not Started';
}
</script>
