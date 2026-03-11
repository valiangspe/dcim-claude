<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import * as XLSX from 'xlsx'
import {
  upsUnitsApi, type UpsUnit,
  powerPhasesApi, type PowerPhase,
  cabinetTemperaturesApi, type CabinetTemperature,
  alarmsApi, type Alarm,
  dcIncidentsApi, type DcIncident,
  sensorsApi, type Sensor,
  pueReadingsApi, type PueReading,
  energyCostRecordsApi, type EnergyCostRecord,
  coolingMetricsApi, type CoolingMetric,
  racksApi, type Rack,
  uptimeRecordsApi, type UptimeRecord,
  complianceChecklistsApi, type ComplianceChecklist,
  lifecyclePhasesApi, type LifecyclePhase,
  powerChainsApi, type PowerChain,
} from '../../services/api'

// ─── State ───────────────────────────────────────────────────────────────────
const activeTab = ref<'weekly' | 'monthly' | 'quarterly' | 'annual'>('weekly')
const loading = ref(true)
const error = ref('')

// Data
const upsUnits = ref<UpsUnit[]>([])
const powerPhases = ref<PowerPhase[]>([])
const cabinetTemps = ref<CabinetTemperature[]>([])
const alarms = ref<Alarm[]>([])
const incidents = ref<DcIncident[]>([])
const sensors = ref<Sensor[]>([])
const pueReadings = ref<PueReading[]>([])
const energyCosts = ref<EnergyCostRecord[]>([])
const coolingMetrics = ref<CoolingMetric[]>([])
const racks = ref<Rack[]>([])
const uptimeRecords = ref<UptimeRecord[]>([])
const complianceChecklists = ref<ComplianceChecklist[]>([])
const lifecyclePhases = ref<LifecyclePhase[]>([])
const powerChains = ref<PowerChain[]>([])

onMounted(async () => {
  try {
    const results = await Promise.allSettled([
      upsUnitsApi.getAll(), powerPhasesApi.getAll(), cabinetTemperaturesApi.getAll(),
      alarmsApi.getAll(), dcIncidentsApi.getAll(), sensorsApi.getAll(),
      pueReadingsApi.getAll(), energyCostRecordsApi.getAll(), coolingMetricsApi.getAll(),
      racksApi.getAll(), uptimeRecordsApi.getAll(), complianceChecklistsApi.getAll(),
      lifecyclePhasesApi.getAll(), powerChainsApi.getAll(),
    ])
    const get = <T>(r: PromiseSettledResult<T[]>, fallback: T[] = []) =>
      r.status === 'fulfilled' ? r.value : fallback
    upsUnits.value = get(results[0] as PromiseSettledResult<UpsUnit[]>)
    powerPhases.value = get(results[1] as PromiseSettledResult<PowerPhase[]>)
    cabinetTemps.value = get(results[2] as PromiseSettledResult<CabinetTemperature[]>)
    alarms.value = get(results[3] as PromiseSettledResult<Alarm[]>)
    incidents.value = get(results[4] as PromiseSettledResult<DcIncident[]>)
    sensors.value = get(results[5] as PromiseSettledResult<Sensor[]>)
    pueReadings.value = get(results[6] as PromiseSettledResult<PueReading[]>)
    energyCosts.value = get(results[7] as PromiseSettledResult<EnergyCostRecord[]>)
    coolingMetrics.value = get(results[8] as PromiseSettledResult<CoolingMetric[]>)
    racks.value = get(results[9] as PromiseSettledResult<Rack[]>)
    uptimeRecords.value = get(results[10] as PromiseSettledResult<UptimeRecord[]>)
    complianceChecklists.value = get(results[11] as PromiseSettledResult<ComplianceChecklist[]>)
    lifecyclePhases.value = get(results[12] as PromiseSettledResult<LifecyclePhase[]>)
    powerChains.value = get(results[13] as PromiseSettledResult<PowerChain[]>)
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'Failed to load report data'
  } finally {
    loading.value = false
  }
})

// ─── Thresholds ──────────────────────────────────────────────────────────────
const THRESHOLDS = {
  upsLoad:      { warning: 40, critical: 45 },
  inletTemp:    { warning: 25, critical: 27 },
  phaseBalance: { warning: 15, critical: 25 },
  rackUtil:     { warning: 70, critical: 85 },
  pue:          { warning: 1.5, critical: 1.8 },
}

function gate(value: number, warn: number, crit: number): 'ok' | 'warning' | 'critical' {
  if (value >= crit) return 'critical'
  if (value >= warn) return 'warning'
  return 'ok'
}
const gateColor = { ok: 'success', warning: 'warning', critical: 'danger' } as const
const gateLabel = { ok: 'OK', warning: 'Warning', critical: 'Critical' } as const

// ─── Weekly KPIs ─────────────────────────────────────────────────────────────
const weeklyUps = computed(() => upsUnits.value.map(u => ({
  ...u,
  gate: gate(u.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical),
  tierSafe: u.loadPct < THRESHOLDS.upsLoad.warning,
})))

const weeklyTemps = computed(() => cabinetTemps.value.map(c => ({
  ...c,
  gate: gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical),
})))

const weeklyPhases = computed(() => {
  if (!powerPhases.value.length) return []
  const voltages = powerPhases.value.map(p => p.voltage)
  const avgV = voltages.reduce((s, v) => s + v, 0) / voltages.length
  return powerPhases.value.map(p => {
    const dev = avgV > 0 ? Math.abs((p.voltage - avgV) / avgV) * 100 : 0
    return { ...p, deviation: parseFloat(dev.toFixed(1)), gate: gate(dev, THRESHOLDS.phaseBalance.warning, THRESHOLDS.phaseBalance.critical) }
  })
})

const weeklyAlarms = computed(() => ({
  critical: alarms.value.filter(a => a.severity === 'critical').length,
  warning: alarms.value.filter(a => a.severity === 'warning' || a.severity === 'high').length,
  total: alarms.value.length,
  unacked: alarms.value.filter(a => !a.acknowledged).length,
}))

const weeklyIncidents = computed(() => ({
  open: incidents.value.filter(i => ['open', 'investigating', 'in-progress'].includes(i.status)).length,
  critical: incidents.value.filter(i => i.severity === 'critical').length,
  total: incidents.value.length,
}))

const weeklySensors = computed(() => ({
  total: sensors.value.length,
  active: sensors.value.filter(s => s.status === 'active' || s.status === 'online').length,
  offline: sensors.value.filter(s => s.status === 'offline' || s.status === 'error').length,
}))

const weeklyStatus = computed((): 'ok' | 'warning' | 'critical' => {
  const hasUpsCritical = weeklyUps.value.some(u => u.gate === 'critical')
  const hasTempCritical = weeklyTemps.value.some(t => t.gate === 'critical')
  const hasPhaseCritical = weeklyPhases.value.some(p => p.gate === 'critical')
  if (hasUpsCritical || hasTempCritical || hasPhaseCritical || weeklyAlarms.value.critical > 0) return 'critical'
  const hasUpsWarn = weeklyUps.value.some(u => u.gate === 'warning')
  const hasTempWarn = weeklyTemps.value.some(t => t.gate === 'warning')
  if (hasUpsWarn || hasTempWarn || weeklyAlarms.value.warning > 0) return 'warning'
  return 'ok'
})

// ─── Monthly KPIs ─────────────────────────────────────────────────────────────
const monthlyPue = computed(() => {
  if (!pueReadings.value.length) return null
  const avg = pueReadings.value.reduce((s, r) => s + r.pue, 0) / pueReadings.value.length
  return { avg: parseFloat(avg.toFixed(3)), gate: gate(avg, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical) }
})

const monthlyEnergy = computed(() => {
  if (!energyCosts.value.length) return null
  const total = energyCosts.value.reduce((s, r) => s + r.kwh, 0)
  const cost = energyCosts.value.reduce((s, r) => s + r.cost, 0)
  const avgRate = energyCosts.value.reduce((s, r) => s + r.rate, 0) / energyCosts.value.length
  return { totalKwh: total.toFixed(0), totalCost: cost.toFixed(2), avgRate: avgRate.toFixed(4) }
})

const monthlyCooling = computed(() => coolingMetrics.value)

const monthlyThermal = computed(() => {
  if (!cabinetTemps.value.length) return null
  const inlets = cabinetTemps.value.map(c => c.inlet)
  const max = Math.max(...inlets)
  const avg = inlets.reduce((s, v) => s + v, 0) / inlets.length
  const hotspots = cabinetTemps.value.filter(c => c.inlet >= THRESHOLDS.inletTemp.warning)
  return { max: max.toFixed(1), avg: avg.toFixed(1), hotspots: hotspots.length, gate: gate(max, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical) }
})

// ─── Quarterly KPIs ──────────────────────────────────────────────────────────
const quarterlyRacks = computed(() => {
  if (!racks.value.length) return null
  const totalU = racks.value.reduce((s, r) => s + r.totalU, 0)
  const usedU = racks.value.reduce((s, r) => s + r.usedU, 0)
  const pct = totalU > 0 ? (usedU / totalU) * 100 : 0
  return { totalU, usedU, pct: parseFloat(pct.toFixed(1)), gate: gate(pct, THRESHOLDS.rackUtil.warning, THRESHOLDS.rackUtil.critical) }
})

const quarterlyRedundancy = computed(() => powerChains.value.map(c => ({
  ...c,
  loadPct: c.capacity > 0 ? parseFloat(((c.load / c.capacity) * 100).toFixed(1)) : 0,
  concurrentlySafe: c.capacity > 0 && (c.load / c.capacity) * 100 < THRESHOLDS.upsLoad.warning,
})))

const quarterlyConcurrentMaintainability = computed(() => {
  const upsIssues = weeklyUps.value.filter(u => !u.tierSafe)
  const chainIssues = quarterlyRedundancy.value.filter(c => !c.concurrentlySafe)
  const pass = upsIssues.length === 0 && chainIssues.length === 0
  return { pass, upsIssues: upsIssues.length, chainIssues: chainIssues.length }
})

// ─── Annual KPIs ─────────────────────────────────────────────────────────────
const annualUptime = computed(() => {
  if (!uptimeRecords.value.length) return []
  return uptimeRecords.value.map(r => {
    const total = r.uptimeMinutes + r.downtimeMinutes
    const pct = total > 0 ? ((r.uptimeMinutes / total) * 100) : 100
    return { ...r, availabilityPct: parseFloat(pct.toFixed(3)), slaMet: pct >= r.slaTarget }
  })
})

const annualCompliance = computed(() => {
  const total = complianceChecklists.value.length
  const compliant = complianceChecklists.value.filter(c => c.status === 'compliant').length
  const nonCompliant = complianceChecklists.value.filter(c => c.status === 'non-compliant').length
  const score = total > 0 ? Math.round((compliant / total) * 100) : 0
  return { total, compliant, nonCompliant, score }
})

const annualIncidentSummary = computed(() => {
  const byCategory: Record<string, number> = {}
  incidents.value.forEach(i => { byCategory[i.category] = (byCategory[i.category] || 0) + 1 })
  const totalDowntime = incidents.value.reduce((s, i) => s + i.downtimeMinutes, 0)
  return { total: incidents.value.length, byCategory, totalDowntime }
})

const annualLifecycle = computed(() => lifecyclePhases.value)

// ─── XLSX Export ─────────────────────────────────────────────────────────────
const today = new Date().toISOString().slice(0, 10)

function styleHeader(ws: XLSX.WorkSheet, range: string) {
  // SheetJS CE doesn't support cell styling; headers are just bold via value convention
  return ws
}

function addCoverSheet(wb: XLSX.WorkBook, reportTitle: string, cadence: string) {
  const data = [
    ['DATACENTER COMPLIANCE ENGINE'],
    [reportTitle],
    [''],
    ['Generated:', today],
    ['Cadence:', cadence],
    ['Standards:', 'Uptime Institute Tier III | TIA-942 | ISO 50001 | ISO/IEC 22237'],
    [''],
    ['THRESHOLD REFERENCE'],
    ['Category', 'Metric', 'Warning Gate', 'Critical Gate'],
    ['Power', 'UPS Bus Load', '>40%', '>45%'],
    ['Cooling', 'Server Inlet Temp', '>25°C', '>27°C'],
    ['Redundancy', 'Phase Balance Deviation', '>15%', '>25%'],
    ['Space', 'Rack Utilization', '>70%', '>85%'],
    ['Efficiency', 'PUE', '>1.5', '>1.8'],
  ]
  const ws = XLSX.utils.aoa_to_sheet(data)
  ws['!cols'] = [{ wch: 30 }, { wch: 25 }, { wch: 20 }, { wch: 20 }]
  XLSX.utils.book_append_sheet(wb, ws, 'Cover')
}

function downloadWeekly() {
  const wb = XLSX.utils.book_new()
  addCoverSheet(wb, 'Weekly Operational Health Report', 'Weekly')

  // UPS Load
  const upsData = [
    ['WEEKLY UPS LOAD — TIER III CONCURRENT MAINTAINABILITY CHECK'],
    ['UPS Unit', 'Capacity (kW)', 'Load (kW)', 'Load %', 'Gate', 'Tier III Safe (<40%)', 'Mode', 'Status'],
    ...weeklyUps.value.map(u => [
      u.name, u.capacity, u.load, u.loadPct + '%',
      gateLabel[u.gate], u.tierSafe ? 'YES' : 'NO — ACTION REQUIRED',
      u.mode, u.status,
    ]),
    [''],
    ['SUMMARY'],
    ['Total UPS Units', upsUnits.value.length],
    ['Tier III Safe', weeklyUps.value.filter(u => u.tierSafe).length],
    ['Warning (40–45%)', weeklyUps.value.filter(u => u.gate === 'warning').length],
    ['Critical (>45%)', weeklyUps.value.filter(u => u.gate === 'critical').length],
  ]
  const wsUps = XLSX.utils.aoa_to_sheet(upsData)
  wsUps['!cols'] = [{ wch: 22 }, { wch: 14 }, { wch: 12 }, { wch: 10 }, { wch: 12 }, { wch: 28 }, { wch: 12 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsUps, 'UPS Load')

  // Inlet Temperature
  const tempData = [
    ['WEEKLY SERVER INLET TEMPERATURE'],
    ['Cabinet ID', 'Location', 'Inlet (°C)', 'Outlet (°C)', 'Delta', 'Gate', 'ASHRAE Alarm'],
    ...weeklyTemps.value.map(c => [
      c.cabinetId, c.location, c.inlet, c.outlet, c.delta,
      gateLabel[c.gate], c.alarm ? 'YES' : 'No',
    ]),
    [''],
    ['SUMMARY'],
    ['Total Cabinets', cabinetTemps.value.length],
    ['OK (<25°C)', weeklyTemps.value.filter(t => t.gate === 'ok').length],
    ['Warning (25–27°C)', weeklyTemps.value.filter(t => t.gate === 'warning').length],
    ['Critical (>27°C)', weeklyTemps.value.filter(t => t.gate === 'critical').length],
    ['Max Inlet (°C)', monthlyThermal.value?.max ?? '—'],
    ['Avg Inlet (°C)', monthlyThermal.value?.avg ?? '—'],
  ]
  const wsTemp = XLSX.utils.aoa_to_sheet(tempData)
  wsTemp['!cols'] = [{ wch: 16 }, { wch: 20 }, { wch: 12 }, { wch: 13 }, { wch: 8 }, { wch: 12 }, { wch: 14 }]
  XLSX.utils.book_append_sheet(wb, wsTemp, 'Inlet Temps')

  // Phase Balance
  const phaseData = [
    ['WEEKLY PHASE BALANCE — REDUNDANCY CHECK'],
    ['Phase', 'Voltage (V)', 'Current (A)', 'Frequency (Hz)', 'THD', 'PF', 'Deviation %', 'Gate'],
    ...weeklyPhases.value.map(p => [
      p.name, p.voltage, p.current, p.frequency, p.thd, p.pf,
      p.deviation + '%', gateLabel[p.gate],
    ]),
    [''],
    ['SUMMARY'],
    ['Phases Monitored', powerPhases.value.length],
    ['OK (<15% dev)', weeklyPhases.value.filter(p => p.gate === 'ok').length],
    ['Warning (15–25%)', weeklyPhases.value.filter(p => p.gate === 'warning').length],
    ['Critical (>25%)', weeklyPhases.value.filter(p => p.gate === 'critical').length],
  ]
  const wsPhase = XLSX.utils.aoa_to_sheet(phaseData)
  wsPhase['!cols'] = [{ wch: 14 }, { wch: 13 }, { wch: 13 }, { wch: 16 }, { wch: 8 }, { wch: 8 }, { wch: 14 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsPhase, 'Phase Balance')

  // Alarms & Incidents
  const alarmData = [
    ['WEEKLY ALARMS & INCIDENTS'],
    ['ACTIVE ALARMS'],
    ['Severity', 'Count'],
    ['Critical', weeklyAlarms.value.critical],
    ['Warning/High', weeklyAlarms.value.warning],
    ['Total Active', weeklyAlarms.value.total],
    ['Unacknowledged', weeklyAlarms.value.unacked],
    [''],
    ['OPEN INCIDENTS'],
    ['Metric', 'Count'],
    ['Open Incidents', weeklyIncidents.value.open],
    ['Critical Severity', weeklyIncidents.value.critical],
    ['Total (all time)', weeklyIncidents.value.total],
    [''],
    ['SENSOR HEALTH (IIoT)'],
    ['Metric', 'Count'],
    ['Total Sensors', weeklySensors.value.total],
    ['Active/Online', weeklySensors.value.active],
    ['Offline/Error', weeklySensors.value.offline],
  ]
  const wsAlarm = XLSX.utils.aoa_to_sheet(alarmData)
  wsAlarm['!cols'] = [{ wch: 25 }, { wch: 15 }]
  XLSX.utils.book_append_sheet(wb, wsAlarm, 'Alarms & Incidents')

  // Overall Status
  const statusData = [
    ['WEEKLY HEALTH REPORT — OVERALL STATUS'],
    [''],
    ['Overall Status', weeklyStatus.value.toUpperCase()],
    [''],
    ['GATE RESULTS'],
    ['Check', 'Result', 'Details'],
    ['UPS Tier III Load', weeklyUps.value.every(u => u.tierSafe) ? 'PASS' : 'FAIL', `${weeklyUps.value.filter(u => !u.tierSafe).length} units above 40%`],
    ['Inlet Temperature', weeklyTemps.value.every(t => t.gate === 'ok') ? 'PASS' : weeklyTemps.value.some(t => t.gate === 'critical') ? 'FAIL' : 'WARN', `Max: ${monthlyThermal.value?.max ?? '—'}°C`],
    ['Phase Balance', weeklyPhases.value.every(p => p.gate === 'ok') ? 'PASS' : weeklyPhases.value.some(p => p.gate === 'critical') ? 'FAIL' : 'WARN', `${weeklyPhases.value.filter(p => p.gate !== 'ok').length} phases out of balance`],
    ['Active Critical Alarms', weeklyAlarms.value.critical === 0 ? 'PASS' : 'FAIL', `${weeklyAlarms.value.critical} critical alarm(s)`],
    ['Open Incidents', weeklyIncidents.value.open === 0 ? 'PASS' : 'WARN', `${weeklyIncidents.value.open} open incident(s)`],
    ['Sensor Connectivity', weeklySensors.value.offline === 0 ? 'PASS' : 'WARN', `${weeklySensors.value.offline} sensor(s) offline`],
  ]
  const wsStatus = XLSX.utils.aoa_to_sheet(statusData)
  wsStatus['!cols'] = [{ wch: 28 }, { wch: 10 }, { wch: 35 }]
  XLSX.utils.book_append_sheet(wb, wsStatus, 'Overall Status')

  XLSX.writeFile(wb, `weekly-health-report-${today}.xlsx`)
}

function downloadMonthly() {
  const wb = XLSX.utils.book_new()
  addCoverSheet(wb, 'Monthly Efficiency & Thermal Report', 'Monthly')

  // PUE
  const pueData = [
    ['MONTHLY PUE ANALYSIS — ISO 50001'],
    ['Date', 'PUE', 'Component', 'Power (kW)', 'Share %', 'Gate'],
    ...pueReadings.value.map(r => [
      r.date, r.pue, r.component, r.power, r.pct + '%',
      gateLabel[gate(r.pue, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical)],
    ]),
    [''],
    ['SUMMARY'],
    ['Average PUE', monthlyPue.value?.avg ?? '—'],
    ['PUE Gate', monthlyPue.value ? gateLabel[monthlyPue.value.gate] : '—'],
    ['ISO 50001 Target', '< 1.5'],
    ['Readings Count', pueReadings.value.length],
  ]
  const wsPue = XLSX.utils.aoa_to_sheet(pueData)
  wsPue['!cols'] = [{ wch: 14 }, { wch: 8 }, { wch: 20 }, { wch: 12 }, { wch: 10 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsPue, 'PUE')

  // Energy Cost
  const energyData = [
    ['MONTHLY ENERGY CONSUMPTION & COST'],
    ['Month', 'kWh', 'Cost ($)', 'Rate ($/kWh)'],
    ...energyCosts.value.map(r => [r.month, r.kwh, r.cost, r.rate]),
    [''],
    ['TOTAL / AVERAGE'],
    ['Total kWh', monthlyEnergy.value?.totalKwh ?? '—'],
    ['Total Cost ($)', monthlyEnergy.value?.totalCost ?? '—'],
    ['Avg Rate ($/kWh)', monthlyEnergy.value?.avgRate ?? '—'],
  ]
  const wsEnergy = XLSX.utils.aoa_to_sheet(energyData)
  wsEnergy['!cols'] = [{ wch: 14 }, { wch: 12 }, { wch: 12 }, { wch: 16 }]
  XLSX.utils.book_append_sheet(wb, wsEnergy, 'Energy Cost')

  // Thermal Profile
  const thermalData = [
    ['MONTHLY THERMAL PROFILE'],
    ['Cabinet ID', 'Location', 'Inlet (°C)', 'Outlet (°C)', 'Delta (°C)', 'Gate', 'ASHRAE A1 Limit'],
    ...cabinetTemps.value.map(c => [
      c.cabinetId, c.location, c.inlet, c.outlet, c.delta,
      gateLabel[gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical)],
      '27°C',
    ]),
    [''],
    ['THERMAL SUMMARY'],
    ['Max Inlet (°C)', monthlyThermal.value?.max ?? '—'],
    ['Avg Inlet (°C)', monthlyThermal.value?.avg ?? '—'],
    ['Hotspots (≥25°C)', monthlyThermal.value?.hotspots ?? 0],
    ['Overall Gate', monthlyThermal.value ? gateLabel[monthlyThermal.value.gate] : '—'],
  ]
  const wsThermal = XLSX.utils.aoa_to_sheet(thermalData)
  wsThermal['!cols'] = [{ wch: 16 }, { wch: 20 }, { wch: 12 }, { wch: 14 }, { wch: 14 }, { wch: 12 }, { wch: 18 }]
  XLSX.utils.book_append_sheet(wb, wsThermal, 'Thermal Profile')

  // Cooling Metrics
  const coolingData = [
    ['MONTHLY COOLING METRICS'],
    ['Metric', 'Value', 'Unit'],
    ...coolingMetrics.value.map(m => [m.name, m.value, m.unit]),
  ]
  const wsCooling = XLSX.utils.aoa_to_sheet(coolingData)
  wsCooling['!cols'] = [{ wch: 28 }, { wch: 16 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsCooling, 'Cooling Metrics')

  XLSX.writeFile(wb, `monthly-efficiency-report-${today}.xlsx`)
}

function downloadQuarterly() {
  const wb = XLSX.utils.book_new()
  addCoverSheet(wb, 'Quarterly Capacity & Redundancy Report', 'Quarterly')

  // Concurrent Maintainability
  const cmData = [
    ['QUARTERLY TIER III CONCURRENT MAINTAINABILITY ASSESSMENT'],
    [''],
    ['RESULT', quarterlyConcurrentMaintainability.value.pass ? 'PASS — Site can sustain component removal at current loads' : 'FAIL — Load exceeds Tier III thresholds; concurrent maintainability at risk'],
    [''],
    ['UPS Units Above 40% Threshold', quarterlyConcurrentMaintainability.value.upsIssues],
    ['Power Chains Above 40% Threshold', quarterlyConcurrentMaintainability.value.chainIssues],
    [''],
    ['UPS DETAIL'],
    ['UPS Unit', 'Load %', 'Tier III Safe', 'Gate', 'Capacity (kW)', 'Load (kW)'],
    ...weeklyUps.value.map(u => [u.name, u.loadPct + '%', u.tierSafe ? 'YES' : 'NO', gateLabel[u.gate], u.capacity, u.load]),
    [''],
    ['POWER CHAIN DETAIL'],
    ['Chain', 'Source', 'Capacity (kW)', 'Load (kW)', 'Load %', 'Redundancy', 'Concurrent Safe'],
    ...quarterlyRedundancy.value.map(c => [
      c.name, c.source, c.capacity, c.load, c.loadPct + '%',
      c.redundancy, c.concurrentlySafe ? 'YES' : 'NO',
    ]),
  ]
  const wsCM = XLSX.utils.aoa_to_sheet(cmData)
  wsCM['!cols'] = [{ wch: 30 }, { wch: 14 }, { wch: 20 }, { wch: 12 }, { wch: 14 }, { wch: 12 }, { wch: 18 }]
  XLSX.utils.book_append_sheet(wb, wsCM, 'Concurrent Maintainability')

  // Space Capacity
  const spaceData = [
    ['QUARTERLY RACK SPACE CAPACITY'],
    ['Rack ID', 'Location', 'Total U', 'Used U', 'Free U', 'Utilization %', 'Gate'],
    ...racks.value.map(r => {
      const pct = r.totalU > 0 ? parseFloat(((r.usedU / r.totalU) * 100).toFixed(1)) : 0
      return [r.rackId, r.location, r.totalU, r.usedU, r.totalU - r.usedU, pct + '%', gateLabel[gate(pct, THRESHOLDS.rackUtil.warning, THRESHOLDS.rackUtil.critical)]]
    }),
    [''],
    ['SPACE SUMMARY'],
    ['Total Racks', racks.value.length],
    ['Total U', quarterlyRacks.value?.totalU ?? '—'],
    ['Used U', quarterlyRacks.value?.usedU ?? '—'],
    ['Overall Utilization', quarterlyRacks.value ? quarterlyRacks.value.pct + '%' : '—'],
    ['Space Gate', quarterlyRacks.value ? gateLabel[quarterlyRacks.value.gate] : '—'],
  ]
  const wsSpace = XLSX.utils.aoa_to_sheet(spaceData)
  wsSpace['!cols'] = [{ wch: 16 }, { wch: 20 }, { wch: 10 }, { wch: 10 }, { wch: 10 }, { wch: 16 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsSpace, 'Space Capacity')

  // Redundancy Stress
  const redData = [
    ['QUARTERLY REDUNDANCY STRESS TEST'],
    ['Scenario: What happens if one UPS/chain is taken offline for maintenance?'],
    [''],
    ['UPS Unit', 'Current Load %', 'If N-1 Estimated Load %', 'N-1 Safe?', 'Recommendation'],
    ...weeklyUps.value.map(u => {
      const n1Est = u.capacity > 0 ? parseFloat(((u.load / (u.capacity * 0.5)) * 100).toFixed(1)) : 0
      const n1Safe = n1Est < THRESHOLDS.upsLoad.critical
      return [u.name, u.loadPct + '%', n1Est + '%', n1Safe ? 'YES' : 'NO', n1Safe ? 'No action required' : 'Reduce load before maintenance']
    }),
    [''],
    ['NOTE: N-1 estimate assumes 50% capacity available when one unit is removed.'],
  ]
  const wsRed = XLSX.utils.aoa_to_sheet(redData)
  wsRed['!cols'] = [{ wch: 22 }, { wch: 18 }, { wch: 24 }, { wch: 12 }, { wch: 36 }]
  XLSX.utils.book_append_sheet(wb, wsRed, 'Redundancy Stress Test')

  XLSX.writeFile(wb, `quarterly-capacity-report-${today}.xlsx`)
}

function downloadAnnual() {
  const wb = XLSX.utils.book_new()
  addCoverSheet(wb, 'Annual SLA Validation & Lifecycle Report', 'Annual')

  // SLA / Uptime
  const slaData = [
    ['ANNUAL SLA VALIDATION — UPTIME INSTITUTE TIER III'],
    ['Site', 'System', 'Period', 'Uptime (min)', 'Downtime (min)', 'Availability %', 'SLA Target %', 'SLA Met?', 'Incidents'],
    ...annualUptime.value.map(r => [
      r.site, r.system, `${r.periodStart} – ${r.periodEnd}`,
      r.uptimeMinutes, r.downtimeMinutes, r.availabilityPct + '%',
      r.slaTarget + '%', r.slaMet ? 'YES' : 'NO — BREACH',
      r.incidents,
    ]),
    [''],
    ['SLA SUMMARY'],
    ['Total Records', uptimeRecords.value.length],
    ['SLA Met', annualUptime.value.filter(r => r.slaMet).length],
    ['SLA Breached', annualUptime.value.filter(r => !r.slaMet).length],
    ['Total Downtime (min)', annualUptime.value.reduce((s, r) => s + r.downtimeMinutes, 0)],
    ['Total Incidents', annualUptime.value.reduce((s, r) => s + r.incidents, 0)],
  ]
  const wsSla = XLSX.utils.aoa_to_sheet(slaData)
  wsSla['!cols'] = [{ wch: 16 }, { wch: 22 }, { wch: 24 }, { wch: 14 }, { wch: 16 }, { wch: 16 }, { wch: 14 }, { wch: 18 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsSla, 'SLA Validation')

  // Incident Summary
  const incData = [
    ['ANNUAL INCIDENT SUMMARY'],
    ['Title', 'Category', 'Severity', 'Status', 'Site', 'Detected', 'Resolved', 'Downtime (min)', 'Root Cause'],
    ...incidents.value.map(i => [
      i.title, i.category, i.severity, i.status, i.site,
      i.detectedAt, i.resolvedAt || 'Open', i.downtimeMinutes, i.rootCause || '—',
    ]),
    [''],
    ['BY CATEGORY'],
    ['Category', 'Count'],
    ...Object.entries(annualIncidentSummary.value.byCategory).map(([cat, count]) => [cat, count]),
    [''],
    ['TOTALS'],
    ['Total Incidents', annualIncidentSummary.value.total],
    ['Total Downtime (min)', annualIncidentSummary.value.totalDowntime],
    ['Total Downtime (hrs)', parseFloat((annualIncidentSummary.value.totalDowntime / 60).toFixed(1))],
  ]
  const wsInc = XLSX.utils.aoa_to_sheet(incData)
  wsInc['!cols'] = [{ wch: 36 }, { wch: 20 }, { wch: 12 }, { wch: 16 }, { wch: 14 }, { wch: 20 }, { wch: 20 }, { wch: 16 }, { wch: 36 }]
  XLSX.utils.book_append_sheet(wb, wsInc, 'Incident Summary')

  // ISO Compliance
  const compData = [
    ['ANNUAL ISO/IEC 22237 COMPLIANCE STATUS'],
    [''],
    ['Overall Score', annualCompliance.value.score + '%'],
    ['Compliant', annualCompliance.value.compliant],
    ['Non-Compliant', annualCompliance.value.nonCompliant],
    ['Total Requirements', annualCompliance.value.total],
    [''],
    ['REQUIREMENT DETAIL'],
    ['Category', 'Requirement', 'ISO Reference', 'Status', 'Assigned To', 'Due Date', 'Evidence'],
    ...complianceChecklists.value.map(c => [
      c.category, c.requirement, c.isoReference, c.status,
      c.assignedTo || '—', c.dueDate || '—', c.evidence || '—',
    ]),
  ]
  const wsComp = XLSX.utils.aoa_to_sheet(compData)
  wsComp['!cols'] = [{ wch: 16 }, { wch: 36 }, { wch: 24 }, { wch: 16 }, { wch: 18 }, { wch: 14 }, { wch: 20 }]
  XLSX.utils.book_append_sheet(wb, wsComp, 'Compliance')

  // Lifecycle
  const lcData = [
    ['ANNUAL ASSET LIFECYCLE SUMMARY'],
    ['Phase', 'Count', 'Key Items', 'ETA / Timeline'],
    ...annualLifecycle.value.map(p => [p.phase, p.count, p.items, p.eta]),
  ]
  const wsLc = XLSX.utils.aoa_to_sheet(lcData)
  wsLc['!cols'] = [{ wch: 20 }, { wch: 10 }, { wch: 40 }, { wch: 20 }]
  XLSX.utils.book_append_sheet(wb, wsLc, 'Asset Lifecycle')

  XLSX.writeFile(wb, `annual-sla-report-${today}.xlsx`)
}

// ─── Helpers ─────────────────────────────────────────────────────────────────
function fmtMins(m: number) {
  if (!m) return '0m'
  const h = Math.floor(m / 60); const min = m % 60
  return h > 0 ? `${h}h ${min}m` : `${min}m`
}
</script>

<template>
  <div>
    <div class="d-flex align-items-start justify-content-between mb-4 flex-wrap gap-3">
      <div>
        <h4 class="mb-1">Active Compliance Engine — Reports</h4>
        <p class="text-muted small mb-0">Uptime Institute Tier III · TIA-942 · ISO 50001 · ISO/IEC 22237</p>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5"><div class="spinner-border"></div></div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>
    <template v-else>

      <!-- Tab Nav -->
      <ul class="nav nav-tabs mb-4">
        <li class="nav-item">
          <button class="nav-link" :class="{ active: activeTab === 'weekly' }" @click="activeTab = 'weekly'">
            Weekly <span class="badge ms-1" :class="`bg-${gateColor[weeklyStatus]}`">{{ gateLabel[weeklyStatus] }}</span>
          </button>
        </li>
        <li class="nav-item">
          <button class="nav-link" :class="{ active: activeTab === 'monthly' }" @click="activeTab = 'monthly'">Monthly</button>
        </li>
        <li class="nav-item">
          <button class="nav-link" :class="{ active: activeTab === 'quarterly' }" @click="activeTab = 'quarterly'">Quarterly</button>
        </li>
        <li class="nav-item">
          <button class="nav-link" :class="{ active: activeTab === 'annual' }" @click="activeTab = 'annual'">Annual</button>
        </li>
      </ul>

      <!-- ── WEEKLY ─────────────────────────────────────────────────────── -->
      <div v-if="activeTab === 'weekly'">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <div>
            <h5 class="mb-0">Weekly Operational Health Report</h5>
            <small class="text-muted">UPS load · Inlet temps · Phase balance · Alarms · IIoT</small>
          </div>
          <button class="btn btn-success" @click="downloadWeekly">⬇ Download XLSX</button>
        </div>

        <!-- Overall status banner -->
        <div class="alert mb-4" :class="`alert-${gateColor[weeklyStatus]}`">
          <strong>Overall Status: {{ gateLabel[weeklyStatus] }}</strong>
          — Generated {{ today }}
        </div>

        <!-- UPS Load -->
        <div class="card border-0 shadow-sm mb-4">
          <div class="card-header bg-transparent fw-semibold d-flex justify-content-between">
            <span>UPS Bus Load — Tier III Gate (&lt;40% Warning, &lt;45% Critical)</span>
            <span class="badge" :class="weeklyUps.every(u => u.tierSafe) ? 'bg-success' : 'bg-danger'">
              {{ weeklyUps.every(u => u.tierSafe) ? 'PASS' : 'FAIL' }}
            </span>
          </div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>UPS</th><th>Load %</th><th>Load / Capacity</th><th>Gate</th><th>Tier III Safe</th><th>Mode</th></tr>
              </thead>
              <tbody>
                <tr v-for="u in weeklyUps" :key="u.id">
                  <td class="fw-semibold">{{ u.name }}</td>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="progress flex-grow-1" style="height:8px;min-width:80px">
                        <div class="progress-bar" :class="`bg-${gateColor[u.gate]}`" :style="`width:${u.loadPct}%`"></div>
                      </div>
                      <span :class="`text-${gateColor[u.gate]}`">{{ u.loadPct }}%</span>
                    </div>
                  </td>
                  <td class="text-muted">{{ u.load }} / {{ u.capacity }} kW</td>
                  <td><span class="badge" :class="`bg-${gateColor[u.gate]}`">{{ gateLabel[u.gate] }}</span></td>
                  <td>
                    <span class="badge" :class="u.tierSafe ? 'bg-success' : 'bg-danger'">
                      {{ u.tierSafe ? 'Yes' : 'No — Act' }}
                    </span>
                  </td>
                  <td class="text-muted">{{ u.mode }}</td>
                </tr>
                <tr v-if="!weeklyUps.length"><td colspan="6" class="text-muted text-center py-3">No UPS data</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Inlet Temps -->
        <div class="card border-0 shadow-sm mb-4">
          <div class="card-header bg-transparent fw-semibold d-flex justify-content-between">
            <span>Server Inlet Temperature — Gate: 25°C Warning / 27°C Critical (ASHRAE A1)</span>
            <span class="badge" :class="weeklyTemps.some(t => t.gate === 'critical') ? 'bg-danger' : weeklyTemps.some(t => t.gate === 'warning') ? 'bg-warning text-dark' : 'bg-success'">
              {{ weeklyTemps.some(t => t.gate === 'critical') ? 'CRITICAL' : weeklyTemps.some(t => t.gate === 'warning') ? 'WARNING' : 'PASS' }}
            </span>
          </div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>Cabinet</th><th>Location</th><th>Inlet °C</th><th>Outlet °C</th><th>Delta</th><th>Gate</th></tr>
              </thead>
              <tbody>
                <tr v-for="c in weeklyTemps" :key="c.id">
                  <td class="fw-semibold">{{ c.cabinetId }}</td>
                  <td class="text-muted">{{ c.location }}</td>
                  <td><span class="fw-semibold" :class="`text-${gateColor[c.gate]}`">{{ c.inlet }}°C</span></td>
                  <td>{{ c.outlet }}°C</td>
                  <td class="text-muted">{{ c.delta }}°C</td>
                  <td><span class="badge" :class="`bg-${gateColor[c.gate]}`">{{ gateLabel[c.gate] }}</span></td>
                </tr>
                <tr v-if="!weeklyTemps.length"><td colspan="6" class="text-muted text-center py-3">No temperature data</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Phase Balance -->
        <div class="card border-0 shadow-sm mb-4">
          <div class="card-header bg-transparent fw-semibold d-flex justify-content-between">
            <span>Phase Balance — Gate: 15% Deviation Warning / 25% Critical</span>
            <span class="badge" :class="weeklyPhases.some(p => p.gate === 'critical') ? 'bg-danger' : weeklyPhases.some(p => p.gate === 'warning') ? 'bg-warning text-dark' : 'bg-success'">
              {{ weeklyPhases.some(p => p.gate === 'critical') ? 'CRITICAL' : weeklyPhases.some(p => p.gate === 'warning') ? 'WARNING' : 'PASS' }}
            </span>
          </div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>Phase</th><th>Voltage (V)</th><th>Current (A)</th><th>Freq (Hz)</th><th>PF</th><th>Deviation</th><th>Gate</th></tr>
              </thead>
              <tbody>
                <tr v-for="p in weeklyPhases" :key="p.id">
                  <td class="fw-semibold">{{ p.name }}</td>
                  <td>{{ p.voltage }}</td><td>{{ p.current }}</td><td>{{ p.frequency }}</td><td>{{ p.pf }}</td>
                  <td><span :class="`text-${gateColor[p.gate]}`">{{ p.deviation }}%</span></td>
                  <td><span class="badge" :class="`bg-${gateColor[p.gate]}`">{{ gateLabel[p.gate] }}</span></td>
                </tr>
                <tr v-if="!weeklyPhases.length"><td colspan="7" class="text-muted text-center py-3">No phase data</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Alarms + Sensors row -->
        <div class="row g-3">
          <div class="col-md-6">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-header bg-transparent fw-semibold">Active Alarms</div>
              <div class="card-body">
                <div class="row g-2 text-center">
                  <div class="col-6"><h3 class="text-danger">{{ weeklyAlarms.critical }}</h3><small class="text-muted">Critical</small></div>
                  <div class="col-6"><h3 class="text-warning">{{ weeklyAlarms.warning }}</h3><small class="text-muted">Warning</small></div>
                  <div class="col-6"><h3>{{ weeklyAlarms.total }}</h3><small class="text-muted">Total Active</small></div>
                  <div class="col-6"><h3 class="text-info">{{ weeklyAlarms.unacked }}</h3><small class="text-muted">Unacknowledged</small></div>
                </div>
              </div>
            </div>
          </div>
          <div class="col-md-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-header bg-transparent fw-semibold">Open Incidents</div>
              <div class="card-body text-center">
                <h2 :class="weeklyIncidents.open > 0 ? 'text-danger' : 'text-success'">{{ weeklyIncidents.open }}</h2>
                <small class="text-muted d-block">Open / Active</small>
                <small class="text-muted">{{ weeklyIncidents.critical }} critical</small>
              </div>
            </div>
          </div>
          <div class="col-md-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-header bg-transparent fw-semibold">Sensor Health (IIoT)</div>
              <div class="card-body text-center">
                <h2 :class="weeklySensors.offline > 0 ? 'text-warning' : 'text-success'">{{ weeklySensors.active }}/{{ weeklySensors.total }}</h2>
                <small class="text-muted d-block">Active / Total</small>
                <small :class="weeklySensors.offline > 0 ? 'text-danger' : 'text-success'">{{ weeklySensors.offline }} offline</small>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- ── MONTHLY ─────────────────────────────────────────────────────── -->
      <div v-else-if="activeTab === 'monthly'">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <div>
            <h5 class="mb-0">Monthly Efficiency &amp; Thermal Report</h5>
            <small class="text-muted">PUE trends · Energy cost · Thermal profile · Cooling metrics</small>
          </div>
          <button class="btn btn-success" @click="downloadMonthly">⬇ Download XLSX</button>
        </div>

        <div class="row g-3 mb-4">
          <div class="col-sm-6 col-lg-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 :class="`text-${monthlyPue ? gateColor[monthlyPue.gate] : 'secondary'}`">{{ monthlyPue?.avg ?? '—' }}</h3>
                <small class="text-muted d-block">Average PUE</small>
                <span class="badge" :class="`bg-${monthlyPue ? gateColor[monthlyPue.gate] : 'secondary'}`">
                  {{ monthlyPue ? gateLabel[monthlyPue.gate] : 'No data' }}
                </span>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 class="text-primary">{{ monthlyEnergy?.totalKwh ?? '—' }}</h3>
                <small class="text-muted d-block">Total kWh</small>
                <small class="text-secondary">${{ monthlyEnergy?.totalCost ?? '—' }} total cost</small>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 :class="`text-${monthlyThermal ? gateColor[monthlyThermal.gate] : 'secondary'}`">{{ monthlyThermal?.max ?? '—' }}°C</h3>
                <small class="text-muted d-block">Max Inlet Temp</small>
                <small class="text-secondary">{{ monthlyThermal?.hotspots ?? 0 }} hotspot(s) ≥25°C</small>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 class="text-info">{{ monthlyEnergy?.avgRate ?? '—' }}</h3>
                <small class="text-muted d-block">Avg $/kWh</small>
                <small class="text-secondary">ISO 50001 energy baseline</small>
              </div>
            </div>
          </div>
        </div>

        <div class="card border-0 shadow-sm mb-4">
          <div class="card-header bg-transparent fw-semibold">PUE Readings</div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>Date</th><th>PUE</th><th>Component</th><th>Power (kW)</th><th>Share %</th><th>Gate</th></tr>
              </thead>
              <tbody>
                <tr v-for="r in pueReadings" :key="r.id">
                  <td>{{ r.date }}</td>
                  <td :class="`text-${gateColor[gate(r.pue, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical)]}`" class="fw-semibold">{{ r.pue }}</td>
                  <td>{{ r.component }}</td><td>{{ r.power }}</td><td>{{ r.pct }}%</td>
                  <td><span class="badge" :class="`bg-${gateColor[gate(r.pue, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical)]}`">{{ gateLabel[gate(r.pue, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical)] }}</span></td>
                </tr>
                <tr v-if="!pueReadings.length"><td colspan="6" class="text-muted text-center py-3">No PUE data</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="card border-0 shadow-sm">
          <div class="card-header bg-transparent fw-semibold">Energy Cost by Month</div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>Month</th><th>kWh</th><th>Cost ($)</th><th>Rate ($/kWh)</th></tr>
              </thead>
              <tbody>
                <tr v-for="r in energyCosts" :key="r.id">
                  <td class="fw-semibold">{{ r.month }}</td>
                  <td>{{ r.kwh.toLocaleString() }}</td>
                  <td>${{ r.cost.toLocaleString() }}</td>
                  <td class="text-muted">${{ r.rate }}</td>
                </tr>
                <tr v-if="!energyCosts.length"><td colspan="4" class="text-muted text-center py-3">No energy cost data</td></tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- ── QUARTERLY ───────────────────────────────────────────────────── -->
      <div v-else-if="activeTab === 'quarterly'">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <div>
            <h5 class="mb-0">Quarterly Capacity &amp; Redundancy Report</h5>
            <small class="text-muted">Concurrent maintainability · Rack capacity · Redundancy stress test</small>
          </div>
          <button class="btn btn-success" @click="downloadQuarterly">⬇ Download XLSX</button>
        </div>

        <!-- CM Banner -->
        <div class="alert mb-4" :class="quarterlyConcurrentMaintainability.pass ? 'alert-success' : 'alert-danger'">
          <strong>Tier III Concurrent Maintainability: {{ quarterlyConcurrentMaintainability.pass ? 'PASS' : 'FAIL' }}</strong>
          <span v-if="!quarterlyConcurrentMaintainability.pass" class="ms-2">
            — {{ quarterlyConcurrentMaintainability.upsIssues }} UPS unit(s) and
            {{ quarterlyConcurrentMaintainability.chainIssues }} chain(s) above the 40% threshold.
            Component removal is not safe at current loads.
          </span>
          <span v-else class="ms-2">— All systems below 40% load. Component removal is safe.</span>
        </div>

        <div class="row g-3 mb-4">
          <div class="col-md-4">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 :class="`text-${quarterlyRacks ? gateColor[quarterlyRacks.gate] : 'secondary'}`">{{ quarterlyRacks?.pct ?? '—' }}%</h3>
                <small class="text-muted d-block">Rack Space Utilization</small>
                <small class="text-secondary">{{ quarterlyRacks?.usedU ?? '—' }} / {{ quarterlyRacks?.totalU ?? '—' }} U used</small>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 :class="quarterlyConcurrentMaintainability.pass ? 'text-success' : 'text-danger'">
                  {{ quarterlyConcurrentMaintainability.pass ? 'Safe' : 'At Risk' }}
                </h3>
                <small class="text-muted d-block">Concurrent Maintainability</small>
                <small class="text-secondary">Tier III validation</small>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 class="text-info">{{ powerChains.length }}</h3>
                <small class="text-muted d-block">Power Chains</small>
                <small :class="quarterlyRedundancy.filter(c => !c.concurrentlySafe).length > 0 ? 'text-danger' : 'text-success'">
                  {{ quarterlyRedundancy.filter(c => !c.concurrentlySafe).length }} above threshold
                </small>
              </div>
            </div>
          </div>
        </div>

        <div class="card border-0 shadow-sm mb-4">
          <div class="card-header bg-transparent fw-semibold">Power Chain Redundancy</div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>Chain</th><th>Source</th><th>Load %</th><th>Redundancy</th><th>Concurrent Safe</th></tr>
              </thead>
              <tbody>
                <tr v-for="c in quarterlyRedundancy" :key="c.id">
                  <td class="fw-semibold">{{ c.name }}</td>
                  <td class="text-muted">{{ c.source }}</td>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="progress flex-grow-1" style="height:6px;min-width:60px">
                        <div class="progress-bar" :class="`bg-${gateColor[gate(c.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical)]}`" :style="`width:${c.loadPct}%`"></div>
                      </div>
                      <span>{{ c.loadPct }}%</span>
                    </div>
                  </td>
                  <td><span class="badge bg-light text-dark border">{{ c.redundancy }}</span></td>
                  <td>
                    <span class="badge" :class="c.concurrentlySafe ? 'bg-success' : 'bg-danger'">
                      {{ c.concurrentlySafe ? 'Yes' : 'No — Load' }}
                    </span>
                  </td>
                </tr>
                <tr v-if="!quarterlyRedundancy.length"><td colspan="5" class="text-muted text-center py-3">No power chain data</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="card border-0 shadow-sm">
          <div class="card-header bg-transparent fw-semibold">N-1 Redundancy Stress Test (UPS)</div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>UPS</th><th>Current Load %</th><th>Estimated N-1 Load %</th><th>N-1 Safe?</th><th>Recommendation</th></tr>
              </thead>
              <tbody>
                <tr v-for="u in weeklyUps" :key="u.id">
                  <td class="fw-semibold">{{ u.name }}</td>
                  <td>{{ u.loadPct }}%</td>
                  <td>
                    <span v-if="u.capacity > 0" :class="`text-${gateColor[gate((u.load / (u.capacity * 0.5)) * 100, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical)]}`">
                      {{ ((u.load / (u.capacity * 0.5)) * 100).toFixed(1) }}%
                    </span>
                  </td>
                  <td>
                    <span class="badge" :class="u.capacity > 0 && (u.load / (u.capacity * 0.5)) * 100 < THRESHOLDS.upsLoad.critical ? 'bg-success' : 'bg-danger'">
                      {{ u.capacity > 0 && (u.load / (u.capacity * 0.5)) * 100 < THRESHOLDS.upsLoad.critical ? 'Safe' : 'Unsafe' }}
                    </span>
                  </td>
                  <td class="text-muted small">
                    {{ u.capacity > 0 && (u.load / (u.capacity * 0.5)) * 100 < THRESHOLDS.upsLoad.critical ? 'No action required' : 'Reduce load before maintenance window' }}
                  </td>
                </tr>
                <tr v-if="!weeklyUps.length"><td colspan="5" class="text-muted text-center py-3">No UPS data</td></tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- ── ANNUAL ──────────────────────────────────────────────────────── -->
      <div v-else-if="activeTab === 'annual'">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <div>
            <h5 class="mb-0">Annual SLA Validation &amp; Lifecycle Report</h5>
            <small class="text-muted">Uptime SLA · Incident analysis · ISO compliance score · Asset lifecycle</small>
          </div>
          <button class="btn btn-success" @click="downloadAnnual">⬇ Download XLSX</button>
        </div>

        <div class="row g-3 mb-4">
          <div class="col-sm-6 col-lg-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 :class="annualCompliance.score >= 80 ? 'text-success' : annualCompliance.score >= 60 ? 'text-warning' : 'text-danger'">{{ annualCompliance.score }}%</h3>
                <small class="text-muted d-block">ISO Compliance Score</small>
                <small class="text-secondary">{{ annualCompliance.compliant }}/{{ annualCompliance.total }} met</small>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 :class="annualUptime.filter(r => !r.slaMet).length > 0 ? 'text-danger' : 'text-success'">
                  {{ annualUptime.filter(r => r.slaMet).length }}/{{ annualUptime.length }}
                </h3>
                <small class="text-muted d-block">SLA Records Met</small>
                <small :class="annualUptime.filter(r => !r.slaMet).length > 0 ? 'text-danger' : 'text-success'">
                  {{ annualUptime.filter(r => !r.slaMet).length }} breach(es)
                </small>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 class="text-warning">{{ incidents.length }}</h3>
                <small class="text-muted d-block">Total Incidents</small>
                <small class="text-secondary">{{ fmtMins(annualIncidentSummary.totalDowntime) }} downtime</small>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-3">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body">
                <h3 class="text-danger">{{ annualCompliance.nonCompliant }}</h3>
                <small class="text-muted d-block">Non-Compliant Items</small>
                <small class="text-secondary">Action required</small>
              </div>
            </div>
          </div>
        </div>

        <div class="card border-0 shadow-sm mb-4">
          <div class="card-header bg-transparent fw-semibold">SLA Validation — Uptime Institute</div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>Site</th><th>System</th><th>Period</th><th>Availability</th><th>SLA Target</th><th>SLA</th><th>Downtime</th></tr>
              </thead>
              <tbody>
                <tr v-for="r in annualUptime" :key="r.id">
                  <td class="fw-semibold">{{ r.site }}</td>
                  <td>{{ r.system }}</td>
                  <td class="text-muted">{{ r.periodStart }} – {{ r.periodEnd }}</td>
                  <td :class="r.slaMet ? 'text-success' : 'text-danger'" class="fw-semibold">{{ r.availabilityPct }}%</td>
                  <td class="text-muted">{{ r.slaTarget }}%</td>
                  <td><span class="badge" :class="r.slaMet ? 'bg-success' : 'bg-danger'">{{ r.slaMet ? 'Met' : 'Breach' }}</span></td>
                  <td :class="r.downtimeMinutes > 0 ? 'text-danger' : 'text-muted'">{{ fmtMins(r.downtimeMinutes) }}</td>
                </tr>
                <tr v-if="!annualUptime.length"><td colspan="7" class="text-muted text-center py-3">No uptime records</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="card border-0 shadow-sm mb-4">
          <div class="card-header bg-transparent fw-semibold">ISO/IEC 22237 Compliance Requirements</div>
          <div class="card-body p-0">
            <table class="table table-hover align-middle mb-0 small">
              <thead class="table-light">
                <tr><th>Category</th><th>Requirement</th><th>ISO Ref</th><th>Status</th><th>Assigned</th><th>Due</th></tr>
              </thead>
              <tbody>
                <tr v-for="c in complianceChecklists" :key="c.id">
                  <td><span class="badge bg-light text-dark border">{{ c.category }}</span></td>
                  <td>{{ c.requirement }}</td>
                  <td class="text-muted font-monospace small">{{ c.isoReference }}</td>
                  <td>
                    <span class="badge" :class="c.status === 'compliant' ? 'bg-success' : c.status === 'non-compliant' ? 'bg-danger' : c.status === 'in-progress' ? 'bg-warning text-dark' : 'bg-secondary'">
                      {{ c.status }}
                    </span>
                  </td>
                  <td class="text-muted">{{ c.assignedTo || '—' }}</td>
                  <td class="text-muted">{{ c.dueDate || '—' }}</td>
                </tr>
                <tr v-if="!complianceChecklists.length"><td colspan="6" class="text-muted text-center py-3">No compliance data</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="card border-0 shadow-sm">
          <div class="card-header bg-transparent fw-semibold">Incidents by Category</div>
          <div class="card-body">
            <div class="row g-2">
              <div v-for="(count, cat) in annualIncidentSummary.byCategory" :key="cat" class="col-sm-6 col-md-4 col-lg-3">
                <div class="card border-0 bg-light">
                  <div class="card-body py-2">
                    <div class="fw-semibold small">{{ cat }}</div>
                    <h4 class="mb-0">{{ count }}</h4>
                  </div>
                </div>
              </div>
              <div v-if="!Object.keys(annualIncidentSummary.byCategory).length" class="col-12">
                <p class="text-muted mb-0">No incidents recorded</p>
              </div>
            </div>
          </div>
        </div>
      </div>

    </template>
  </div>
</template>
