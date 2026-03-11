import * as XLSX from 'xlsx'
import type { UpsUnit, PowerPhase, CabinetTemperature, Alarm, DcIncident, Sensor, PueReading, EnergyCostRecord, CoolingMetric, Rack, UptimeRecord, ComplianceChecklist, LifecyclePhase, PowerChain } from '../services/api'

export const THRESHOLDS = {
  upsLoad:      { warning: 40, critical: 45 },
  inletTemp:    { warning: 25, critical: 27 },
  phaseBalance: { warning: 15, critical: 25 },
  rackUtil:     { warning: 70, critical: 85 },
  pue:          { warning: 1.5, critical: 1.8 },
}

export type GateResult = 'ok' | 'warning' | 'critical'
export const GATE_COLOR: Record<GateResult, string> = { ok: 'success', warning: 'warning', critical: 'danger' }
export const GATE_LABEL: Record<GateResult, string> = { ok: 'OK', warning: 'Warning', critical: 'Critical' }

export function gate(value: number, warn: number, crit: number): GateResult {
  if (value >= crit) return 'critical'
  if (value >= warn) return 'warning'
  return 'ok'
}

export function fmtMins(m: number) {
  if (!m) return '0m'
  const h = Math.floor(m / 60); const min = m % 60
  return h > 0 ? `${h}h ${min}m` : `${min}m`
}

export function phaseDeviations(phases: PowerPhase[]) {
  if (!phases.length) return []
  const avg = phases.reduce((s, p) => s + p.voltage, 0) / phases.length
  return phases.map(p => {
    const dev = avg > 0 ? Math.abs((p.voltage - avg) / avg) * 100 : 0
    return { ...p, deviation: parseFloat(dev.toFixed(1)), gate: gate(dev, THRESHOLDS.phaseBalance.warning, THRESHOLDS.phaseBalance.critical) }
  })
}

function coverSheet(wb: XLSX.WorkBook, title: string, cadence: string) {
  const today = new Date().toISOString().slice(0, 10)
  const ws = XLSX.utils.aoa_to_sheet([
    ['DATACENTER ACTIVE COMPLIANCE ENGINE'],
    [title],
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
  ])
  ws['!cols'] = [{ wch: 30 }, { wch: 25 }, { wch: 20 }, { wch: 20 }]
  XLSX.utils.book_append_sheet(wb, ws, 'Cover')
}

export function downloadWeekly(
  upsUnits: UpsUnit[], phases: PowerPhase[], temps: CabinetTemperature[],
  alarms: Alarm[], incidents: DcIncident[], sensors: Sensor[],
) {
  const today = new Date().toISOString().slice(0, 10)
  const wb = XLSX.utils.book_new()
  coverSheet(wb, 'Weekly Operational Health Report', 'Weekly')

  // Overall gate summary
  const upsRows = upsUnits.map(u => ({ ...u, g: gate(u.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical) }))
  const tempRows = temps.map(c => ({ ...c, g: gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical) }))
  const phaseRows = phaseDeviations(phases)
  const critAlarms = alarms.filter(a => a.severity === 'critical').length
  const openIncidents = incidents.filter(i => ['open', 'investigating', 'in-progress'].includes(i.status)).length
  const offlineSensors = sensors.filter(s => s.status === 'offline' || s.status === 'error').length

  const summaryData = [
    ['OVERALL GATE RESULTS'],
    ['Check', 'Result', 'Detail'],
    ['UPS Tier III Load (<40%)', upsRows.every(u => u.g === 'ok') ? 'PASS' : upsRows.some(u => u.g === 'critical') ? 'FAIL-CRITICAL' : 'FAIL-WARNING', `${upsRows.filter(u => u.g !== 'ok').length} unit(s) above threshold`],
    ['Server Inlet Temp (<25°C)', tempRows.every(t => t.g === 'ok') ? 'PASS' : tempRows.some(t => t.g === 'critical') ? 'FAIL-CRITICAL' : 'FAIL-WARNING', `Max: ${temps.length ? Math.max(...temps.map(t => t.inlet)) : '—'}°C`],
    ['Phase Balance (<15% dev)', phaseRows.every(p => p.gate === 'ok') ? 'PASS' : phaseRows.some(p => p.gate === 'critical') ? 'FAIL-CRITICAL' : 'FAIL-WARNING', `${phaseRows.filter(p => p.gate !== 'ok').length} phase(s) out of balance`],
    ['Critical Alarms', critAlarms === 0 ? 'PASS' : 'FAIL', `${critAlarms} critical alarm(s)`],
    ['Open Incidents', openIncidents === 0 ? 'PASS' : 'WARN', `${openIncidents} open`],
    ['Sensor Connectivity', offlineSensors === 0 ? 'PASS' : 'WARN', `${offlineSensors} offline`],
  ]
  const wsSum = XLSX.utils.aoa_to_sheet(summaryData)
  wsSum['!cols'] = [{ wch: 30 }, { wch: 16 }, { wch: 36 }]
  XLSX.utils.book_append_sheet(wb, wsSum, 'Gate Summary')

  // UPS
  const wsUps = XLSX.utils.aoa_to_sheet([
    ['UPS BUS LOAD — TIER III CONCURRENT MAINTAINABILITY'],
    ['UPS Unit', 'Capacity (kW)', 'Load (kW)', 'Load %', 'Gate', 'Tier III Safe (<40%)', 'Mode', 'Status'],
    ...upsRows.map(u => [u.name, u.capacity, u.load, u.loadPct + '%', GATE_LABEL[u.g], u.g === 'ok' ? 'YES' : 'NO — ACTION REQUIRED', u.mode, u.status]),
    [''], ['SUMMARY'], ['Warning (40–45%)', upsRows.filter(u => u.g === 'warning').length], ['Critical (>45%)', upsRows.filter(u => u.g === 'critical').length],
  ])
  wsUps['!cols'] = [{ wch: 22 }, { wch: 14 }, { wch: 12 }, { wch: 10 }, { wch: 12 }, { wch: 28 }, { wch: 12 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsUps, 'UPS Load')

  // Temps
  const wsTemp = XLSX.utils.aoa_to_sheet([
    ['SERVER INLET TEMPERATURE — ASHRAE A1'],
    ['Cabinet', 'Location', 'Inlet (°C)', 'Outlet (°C)', 'Delta', 'Gate'],
    ...tempRows.map(c => [c.cabinetId, c.location, c.inlet, c.outlet, c.delta, GATE_LABEL[c.g]]),
    [''], ['SUMMARY'], ['OK (<25°C)', tempRows.filter(t => t.g === 'ok').length], ['Warning (25–27°C)', tempRows.filter(t => t.g === 'warning').length], ['Critical (>27°C)', tempRows.filter(t => t.g === 'critical').length],
  ])
  wsTemp['!cols'] = [{ wch: 16 }, { wch: 20 }, { wch: 12 }, { wch: 13 }, { wch: 8 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsTemp, 'Inlet Temps')

  // Phases
  const wsPhase = XLSX.utils.aoa_to_sheet([
    ['PHASE BALANCE'],
    ['Phase', 'Voltage (V)', 'Current (A)', 'Freq (Hz)', 'THD', 'PF', 'Deviation %', 'Gate'],
    ...phaseRows.map(p => [p.name, p.voltage, p.current, p.frequency, p.thd, p.pf, p.deviation + '%', GATE_LABEL[p.gate]]),
  ])
  wsPhase['!cols'] = [{ wch: 14 }, { wch: 13 }, { wch: 13 }, { wch: 14 }, { wch: 8 }, { wch: 8 }, { wch: 14 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsPhase, 'Phase Balance')

  // Alarms & IIoT
  const wsAlarms = XLSX.utils.aoa_to_sheet([
    ['ALARMS & IIoT SENSOR HEALTH'],
    ['ALARMS'], ['Severity', 'Count'],
    ['Critical', alarms.filter(a => a.severity === 'critical').length],
    ['Warning', alarms.filter(a => a.severity === 'warning' || a.severity === 'high').length],
    ['Total', alarms.length], ['Unacknowledged', alarms.filter(a => !a.acknowledged).length],
    [''], ['OPEN INCIDENTS'], ['Open', openIncidents], ['Critical', incidents.filter(i => i.severity === 'critical').length],
    [''], ['IIoT SENSORS'], ['Total', sensors.length], ['Active', sensors.filter(s => s.status === 'active' || s.status === 'online').length], ['Offline', offlineSensors],
  ])
  wsAlarms['!cols'] = [{ wch: 25 }, { wch: 15 }]
  XLSX.utils.book_append_sheet(wb, wsAlarms, 'Alarms & IIoT')

  XLSX.writeFile(wb, `weekly-health-report-${today}.xlsx`)
}

export function downloadMonthly(
  pueReadings: PueReading[], energyCosts: EnergyCostRecord[],
  coolingMetrics: CoolingMetric[], temps: CabinetTemperature[],
) {
  const today = new Date().toISOString().slice(0, 10)
  const wb = XLSX.utils.book_new()
  coverSheet(wb, 'Monthly Efficiency & Thermal Report', 'Monthly')

  const wsPue = XLSX.utils.aoa_to_sheet([
    ['PUE ANALYSIS — ISO 50001'],
    ['Date', 'PUE', 'Component', 'Power (kW)', 'Share %', 'Gate'],
    ...pueReadings.map(r => [r.date, r.pue, r.component, r.power, r.pct + '%', GATE_LABEL[gate(r.pue, THRESHOLDS.pue.warning, THRESHOLDS.pue.critical)]]),
    [''], ['Average PUE', pueReadings.length ? (pueReadings.reduce((s, r) => s + r.pue, 0) / pueReadings.length).toFixed(3) : '—'],
  ])
  wsPue['!cols'] = [{ wch: 14 }, { wch: 8 }, { wch: 20 }, { wch: 12 }, { wch: 10 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsPue, 'PUE')

  const wsEnergy = XLSX.utils.aoa_to_sheet([
    ['ENERGY CONSUMPTION & COST'],
    ['Month', 'kWh', 'Cost ($)', 'Rate ($/kWh)'],
    ...energyCosts.map(r => [r.month, r.kwh, r.cost, r.rate]),
    [''], ['Total kWh', energyCosts.reduce((s, r) => s + r.kwh, 0).toFixed(0)],
    ['Total Cost ($)', energyCosts.reduce((s, r) => s + r.cost, 0).toFixed(2)],
  ])
  wsEnergy['!cols'] = [{ wch: 14 }, { wch: 12 }, { wch: 12 }, { wch: 16 }]
  XLSX.utils.book_append_sheet(wb, wsEnergy, 'Energy Cost')

  const wsThermal = XLSX.utils.aoa_to_sheet([
    ['THERMAL PROFILE'],
    ['Cabinet', 'Location', 'Inlet (°C)', 'Outlet (°C)', 'Delta', 'Gate'],
    ...temps.map(c => [c.cabinetId, c.location, c.inlet, c.outlet, c.delta, GATE_LABEL[gate(c.inlet, THRESHOLDS.inletTemp.warning, THRESHOLDS.inletTemp.critical)]]),
    [''], ['Max Inlet', temps.length ? Math.max(...temps.map(t => t.inlet)) + '°C' : '—'],
    ['Avg Inlet', temps.length ? (temps.reduce((s, t) => s + t.inlet, 0) / temps.length).toFixed(1) + '°C' : '—'],
    ['Hotspots (≥25°C)', temps.filter(t => t.inlet >= 25).length],
  ])
  wsThermal['!cols'] = [{ wch: 16 }, { wch: 20 }, { wch: 12 }, { wch: 14 }, { wch: 14 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsThermal, 'Thermal Profile')

  const wsCooling = XLSX.utils.aoa_to_sheet([
    ['COOLING METRICS'], ['Metric', 'Value', 'Unit'],
    ...coolingMetrics.map(m => [m.name, m.value, m.unit]),
  ])
  wsCooling['!cols'] = [{ wch: 28 }, { wch: 16 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsCooling, 'Cooling Metrics')

  XLSX.writeFile(wb, `monthly-efficiency-report-${today}.xlsx`)
}

export function downloadQuarterly(
  upsUnits: UpsUnit[], powerChains: PowerChain[], racks: Rack[],
) {
  const today = new Date().toISOString().slice(0, 10)
  const wb = XLSX.utils.book_new()
  coverSheet(wb, 'Quarterly Capacity & Redundancy Report', 'Quarterly')

  const upsRows = upsUnits.map(u => ({ ...u, g: gate(u.loadPct, THRESHOLDS.upsLoad.warning, THRESHOLDS.upsLoad.critical), tierSafe: u.loadPct < THRESHOLDS.upsLoad.warning }))
  const chainRows = powerChains.map(c => { const pct = c.capacity > 0 ? parseFloat(((c.load / c.capacity) * 100).toFixed(1)) : 0; return { ...c, loadPct: pct, safe: pct < THRESHOLDS.upsLoad.warning } })
  const cmPass = upsRows.every(u => u.tierSafe) && chainRows.every(c => c.safe)

  const wsCM = XLSX.utils.aoa_to_sheet([
    ['TIER III CONCURRENT MAINTAINABILITY ASSESSMENT'],
    ['Result', cmPass ? 'PASS — Safe to remove components at current load' : 'FAIL — Load exceeds Tier III thresholds'],
    ['UPS issues', upsRows.filter(u => !u.tierSafe).length],
    ['Chain issues', chainRows.filter(c => !c.safe).length],
    [''],
    ['UPS DETAIL'], ['UPS Unit', 'Load %', 'Tier III Safe', 'Gate', 'N-1 Est. Load %', 'N-1 Safe?'],
    ...upsRows.map(u => {
      const n1 = u.capacity > 0 ? parseFloat(((u.load / (u.capacity * 0.5)) * 100).toFixed(1)) : 0
      return [u.name, u.loadPct + '%', u.tierSafe ? 'YES' : 'NO', GATE_LABEL[u.g], n1 + '%', n1 < THRESHOLDS.upsLoad.critical ? 'YES' : 'NO']
    }),
    [''], ['CHAIN DETAIL'], ['Chain', 'Source', 'Load %', 'Redundancy', 'Concurrent Safe'],
    ...chainRows.map(c => [c.name, c.source, c.loadPct + '%', c.redundancy, c.safe ? 'YES' : 'NO']),
  ])
  wsCM['!cols'] = [{ wch: 30 }, { wch: 14 }, { wch: 14 }, { wch: 12 }, { wch: 18 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsCM, 'Concurrent Maintainability')

  const rackRows = racks.map(r => { const pct = r.totalU > 0 ? parseFloat(((r.usedU / r.totalU) * 100).toFixed(1)) : 0; return { ...r, pct } })
  const wsSpace = XLSX.utils.aoa_to_sheet([
    ['RACK SPACE CAPACITY'],
    ['Rack', 'Location', 'Total U', 'Used U', 'Free U', 'Utilization %', 'Gate'],
    ...rackRows.map(r => [r.rackId, r.hall, r.totalU, r.usedU, r.totalU - r.usedU, r.pct + '%', GATE_LABEL[gate(r.pct, THRESHOLDS.rackUtil.warning, THRESHOLDS.rackUtil.critical)]]),
    [''], ['Total Racks', racks.length],
    ['Overall Util %', racks.length ? (rackRows.reduce((s, r) => s + r.pct, 0) / rackRows.length).toFixed(1) + '%' : '—'],
  ])
  wsSpace['!cols'] = [{ wch: 16 }, { wch: 20 }, { wch: 10 }, { wch: 10 }, { wch: 10 }, { wch: 16 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsSpace, 'Space Capacity')

  const wsStress = XLSX.utils.aoa_to_sheet([
    ['N-1 REDUNDANCY STRESS TEST'],
    ['Assumes 50% capacity when one unit is removed'],
    [''],
    ['UPS Unit', 'Current Load %', 'N-1 Estimated Load %', 'N-1 Safe?', 'Recommendation'],
    ...upsRows.map(u => {
      const n1 = u.capacity > 0 ? parseFloat(((u.load / (u.capacity * 0.5)) * 100).toFixed(1)) : 0
      return [u.name, u.loadPct + '%', n1 + '%', n1 < THRESHOLDS.upsLoad.critical ? 'YES' : 'NO', n1 < THRESHOLDS.upsLoad.critical ? 'No action required' : 'Reduce load before maintenance']
    }),
  ])
  wsStress['!cols'] = [{ wch: 22 }, { wch: 18 }, { wch: 24 }, { wch: 12 }, { wch: 36 }]
  XLSX.utils.book_append_sheet(wb, wsStress, 'N-1 Stress Test')

  XLSX.writeFile(wb, `quarterly-capacity-report-${today}.xlsx`)
}

export function downloadAnnual(
  uptimeRecords: UptimeRecord[], incidents: DcIncident[],
  checklists: ComplianceChecklist[], lifecycle: LifecyclePhase[],
) {
  const today = new Date().toISOString().slice(0, 10)
  const wb = XLSX.utils.book_new()
  coverSheet(wb, 'Annual SLA Validation & Lifecycle Report', 'Annual')

  const slaRows = uptimeRecords.map(r => {
    const total = r.uptimeMinutes + r.downtimeMinutes
    const pct = total > 0 ? parseFloat(((r.uptimeMinutes / total) * 100).toFixed(3)) : 100
    return { ...r, pct, met: pct >= r.slaTarget }
  })

  const wsSla = XLSX.utils.aoa_to_sheet([
    ['ANNUAL SLA VALIDATION — UPTIME INSTITUTE TIER III'],
    ['Site', 'System', 'Period', 'Availability %', 'SLA Target %', 'SLA Met?', 'Downtime', 'Incidents'],
    ...slaRows.map(r => [r.site, r.system, `${r.periodStart} – ${r.periodEnd}`, r.pct + '%', r.slaTarget + '%', r.met ? 'YES' : 'BREACH', fmtMins(r.downtimeMinutes), r.incidents]),
    [''], ['SLA Met', slaRows.filter(r => r.met).length], ['SLA Breached', slaRows.filter(r => !r.met).length],
    ['Total Downtime', fmtMins(slaRows.reduce((s, r) => s + r.downtimeMinutes, 0))],
  ])
  wsSla['!cols'] = [{ wch: 16 }, { wch: 22 }, { wch: 24 }, { wch: 16 }, { wch: 14 }, { wch: 12 }, { wch: 12 }, { wch: 12 }]
  XLSX.utils.book_append_sheet(wb, wsSla, 'SLA Validation')

  const wsInc = XLSX.utils.aoa_to_sheet([
    ['ANNUAL INCIDENT SUMMARY'],
    ['Title', 'Category', 'Severity', 'Status', 'Site', 'Detected', 'Resolved', 'Downtime (min)', 'Root Cause'],
    ...incidents.map(i => [i.title, i.category, i.severity, i.status, i.site, i.detectedAt, i.resolvedAt || 'Open', i.downtimeMinutes, i.rootCause || '—']),
    [''], ['Total Incidents', incidents.length],
    ['Total Downtime', fmtMins(incidents.reduce((s, i) => s + i.downtimeMinutes, 0))],
  ])
  wsInc['!cols'] = [{ wch: 36 }, { wch: 20 }, { wch: 12 }, { wch: 14 }, { wch: 14 }, { wch: 20 }, { wch: 20 }, { wch: 16 }, { wch: 36 }]
  XLSX.utils.book_append_sheet(wb, wsInc, 'Incidents')

  const total = checklists.length
  const compliant = checklists.filter(c => c.status === 'compliant').length
  const wsComp = XLSX.utils.aoa_to_sheet([
    ['ISO/IEC 22237 COMPLIANCE'],
    ['Score', total > 0 ? Math.round((compliant / total) * 100) + '%' : '—'],
    ['Compliant', compliant], ['Non-Compliant', checklists.filter(c => c.status === 'non-compliant').length], ['Total', total],
    [''],
    ['Category', 'Requirement', 'ISO Reference', 'Status', 'Assigned To', 'Due Date', 'Evidence'],
    ...checklists.map(c => [c.category, c.requirement, c.isoReference, c.status, c.assignedTo || '—', c.dueDate || '—', c.evidence || '—']),
  ])
  wsComp['!cols'] = [{ wch: 16 }, { wch: 36 }, { wch: 24 }, { wch: 16 }, { wch: 18 }, { wch: 14 }, { wch: 20 }]
  XLSX.utils.book_append_sheet(wb, wsComp, 'Compliance')

  const wsLc = XLSX.utils.aoa_to_sheet([
    ['ASSET LIFECYCLE'], ['Phase', 'Count', 'Items', 'ETA'],
    ...lifecycle.map(p => [p.phase, p.count, p.items, p.eta]),
  ])
  wsLc['!cols'] = [{ wch: 20 }, { wch: 10 }, { wch: 40 }, { wch: 20 }]
  XLSX.utils.book_append_sheet(wb, wsLc, 'Asset Lifecycle')

  XLSX.writeFile(wb, `annual-sla-report-${today}.xlsx`)
}
