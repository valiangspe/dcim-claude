# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DCIM (Data Center Infrastructure Management) web application adhering to ISO/IEC 22237. Vue 3 + TypeScript + Bootstrap 5 frontend with three-tier feature gating (Standard/Plus/Platinum).

## Commands

- **Dev server:** `yarn dev`
- **Build:** `yarn build` (Vite bundle only, does NOT run type-check)
- **Preview production build:** `yarn preview`
- **Type-check only:** `npx vue-tsc -b`

Package manager is **Yarn**. No linter, formatter, or test framework is configured.

## Architecture

- **Framework:** Vue 3 with Composition API (`<script setup lang="ts">`)
- **Styling:** Bootstrap 5 CSS only (no other CSS frameworks)
- **Routing:** vue-router with `createWebHashHistory` (hash-based URLs, e.g. `/#/dashboard`)
- **Build tool:** Vite 7
- **Entry:** `src/main.ts` → `App.vue` → `MainLayout.vue` (sidebar + navbar + router-view)

### Key Files
- `src/composables/useTier.ts` — Global reactive tier state and `hasAccess()` gate
- `src/config/menuItems.ts` — Full sidebar menu definition with tier requirements
- `src/components/SidebarMenu.vue` — Collapsible sidebar with tier-aware items
- `src/components/TierSwitcher.vue` — Tier toggle button group
- `src/components/FeatureGate.vue` — Slot wrapper for tier gating in page content
- `src/components/PlaceholderPage.vue` — Reusable placeholder for unbuilt pages
- `src/views/GenericView.vue` — Route-meta-driven view using PlaceholderPage
- `src/router/index.ts` — All routes with meta (title, module, requiredTier, description)
- `src/layouts/MainLayout.vue` — App shell layout

### Tier System
Three tiers: `standard` < `plus` < `platinum`. Controlled via `useTier()` composable. Menu items and routes specify their `requiredTier`. The sidebar shows locked items with tier badges.

### 13 Modules
Dashboard, Power Management, Cooling Management, Space Management, Network & Connectivity, Server & Assets, Environmental, Security, Alarms & Notifications, Change Management, Reports & Analytics, Settings, Visualization.

## Conventions

- Components use `<script setup lang="ts">`
- Props typed via `defineProps<{ ... }>()` generic syntax
- Routes define page metadata (title, module, requiredTier) — GenericView reads from route.meta
- New pages: create view in `src/views/<module>/`, add route in `router/index.ts`, add menu item in `config/menuItems.ts`
- All routes are eagerly imported (no lazy-loading)
- No path aliases configured (use relative imports, no `@/`)
- Global state is a module-level `ref` in `useTier.ts` (no Pinia/Vuex)
- Most views use hardcoded mock data (no API layer exists yet)
- Backend (`backend/`) is a .NET 8 stub, not connected to the frontend
- TypeScript strict mode with `noUnusedLocals`, `noUnusedParameters`
