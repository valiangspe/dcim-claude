const BASE = `${import.meta.env.VITE_APP_BASE_URL || ''}/api`

async function fetchApi<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${url}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })
  if (!res.ok) {
    throw new Error(`API ${res.status}: ${res.statusText}`)
  }
  if (res.status === 204) return undefined as T
  return res.json()
}

function crudApi<T>(resource: string) {
  return {
    getAll: () => fetchApi<T[]>(`/${resource}`),
    getById: (id: number) => fetchApi<T>(`/${resource}/${id}`),
    create: (data: Partial<T>) => fetchApi<T>(`/${resource}`, { method: 'POST', body: JSON.stringify(data) }),
    update: (id: number, data: Partial<T>) => fetchApi<T>(`/${resource}/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
    remove: (id: number) => fetchApi<void>(`/${resource}/${id}`, { method: 'DELETE' }),
  }
}

// ── Original interfaces ──
export interface PowerFeed { id: number; name: string; capacity: number; load: number; status: string }
export interface CoolingUnit { id: number; name: string; type: string; capacity: number; load: number; supplyTemp: number; returnTemp: number; fanSpeed: number; status: string }
export interface Hall { id: number; name: string; sqft: number; racks: number; occupied: number; totalU: number; usedU: number; power: number }
export interface NetworkLink { id: number; name: string; capacity: number; usage: number; latency: number; loss: number; status: string }
export interface Asset { id: number; name: string; class: string; installed: number; planned: number; archived: number }
export interface Alarm { id: number; source: string; severity: string; message: string; time: string; acknowledged: boolean }
export interface ChangeRequest { id: number; requestId: string; requester: string; type: string; priority: string; status: string; requestedDate: string }
export interface User { id: number; name: string; email: string; role: string; lastLogin: string; status: string }
export interface Report { id: number; name: string; description: string; color: string; lastRun: string }
export interface SecurityIncident { id: number; type: string; timestamp: string; location: string; severity: string; status: string }
export interface EnvironmentalMetric { id: number; category: string; name: string; value: number; unit: string; month: number; year: number }
export interface Sensor { id: number; sensorId: string; name: string; type: string; location: string; status: string; minThreshold: string; maxThreshold: string; pollingInterval: string }

// ── Dashboard ──
export interface HealthScoreCategory { id: number; name: string; score: number; color: string }
export interface Site { id: number; name: string; location: string; lat: number; lng: number; status: string; power: number; pue: number; racks: number; alarms: number }
export interface CustomDashboard { id: number; name: string; widgets: number; modified: string; favorite: boolean }

// ── Power ──
export interface PueReading { id: number; date: string; pue: number; component: string; power: number; pct: number }
export interface PowerBudget { id: number; name: string; budget: number; actual: number; reserved: number }
export interface Cabinet { id: number; cabinetId: string; location: string; power: number; capacity: number; phase: string }
export interface PowerChain { id: number; name: string; source: string; capacity: number; load: number; redundancy: string; status: string }
export interface Pdu { id: number; name: string; chain: string; input: number; output: number; load: number; capacity: number; breakers: number }
export interface StrandedPower { id: number; location: string; allocated: number; actual: number; stranded: number; pct: number }
export interface Rectifier { id: number; rectifierId: string; voltage: number; current: number; power: number; efficiency: number; status: string }
export interface DcBus { id: number; name: string; voltage: number; current: number; power: number; temp: number }
export interface BatteryBank { id: number; name: string; type: string; cells: number; soc: number; soh: number; voltage: number; temp: number; runtime: number; status: string }
export interface PowerPhase { id: number; name: string; voltage: number; current: number; frequency: number; thd: number; pf: number }
export interface PowerEvent { id: number; time: string; type: string; phase: string; duration: string; magnitude: string }
export interface ServerPowerRecord { id: number; name: string; rack: string; power: number; cpu: number; psu: string; status: string }
export interface PortCapacityRecord { id: number; pduName: string; total: number; used: number; reserved: number }
export interface EnergyCostRecord { id: number; month: string; kwh: number; cost: number; rate: number }
export interface UpsUnit { id: number; name: string; capacity: number; load: number; loadPct: number; input: number; output: number; battery: number; runtime: number; mode: string; status: string }
export interface BreakerRecord { id: number; breakerId: string; pdu: string; rating: number; load: number; circuits: number; trip: boolean }
export interface PueMetric { id: number; name: string; value: number; target: number; stdDev: number }

// ── Cooling ──
export interface CoolingMetric { id: number; name: string; value: number; unit: string; target: number; description: string }
export interface CabinetTemperature { id: number; cabinetId: string; location: string; inlet: number; outlet: number; delta: number; alarm: boolean }
export interface CabinetHumidity { id: number; cabinetId: string; location: string; rh: number; dewpoint: number; alarm: boolean }
export interface SensorTrend { id: number; sensorId: string; cabinet: string; type: string; time: string; value: number }
export interface CoolingCapacity { id: number; name: string; current: number; max: number; planned: number; available: number; growth: number }
export interface SupplyReturnUnit { id: number; name: string; supplyTemp: number; returnTemp: number; supplyFlow: number; returnFlow: number; deltaT: number; status: string }
export interface WaterUsageRecord { id: number; month: string; waterL: number; itKwh: number; wue: number }
export interface CoolingSystem { id: number; name: string; status: string; uptime: string; lastMaint: string; nextMaint: string; downtime: string }
export interface Chiller { id: number; name: string; type: string; capacity: number; load: number; cop: number; eer: number; supplyW: number; returnW: number; flow: number; status: string }
export interface CabinetAirflow { id: number; cabinetId: string; cfm: number; kw: number; cfmPerKw: number; pressure: number; leakage: number }

// ── Space ──
export interface Rack { id: number; rackId: string; hall: string; totalU: number; usedU: number; power: number; weight: number }
export interface RackGap { id: number; rackId: string; totalU: number; freeU: number; gapStart: number; gapSize: number }
export interface CabinetGroup { id: number; name: string; racks: string; totalU: number; usedU: number; remaining: number }
export interface FloorPlanRack { id: number; rackId: string; rowLabel: string; position: number; utilization: number }
export interface RackDevice { id: number; rackId: string; name: string; startU: number; sizeU: number; type: string; color: string }
export interface Scenario { id: number; name: string; racks: number; power: number; cooling: number; space: number; feasibility: string; notes: string }
export interface SiteNode { id: number; name: string; type: string; parentId: number | null }
export interface StorageSystem { id: number; name: string; type: string; totalTB: number; usedTB: number; iops: number; throughput: string; status: string }
export interface PendingAsset { id: number; name: string; size: number; type: string; targetRack: string }

// ── Network ──
export interface TrafficRecord { id: number; time: string; inbound: number; outbound: number; total: number }
export interface DataSwitch { id: number; name: string; total: number; used: number; speed: string; status: string }
export interface PowerPortRecord { id: number; name: string; total: number; used: number; reserved: number; type: string }
export interface PortRecord { id: number; portId: string; speed: string; vlan: number; color: string; connected: string; status: string }
export interface Cable { id: number; cableId: string; fromEnd: string; toEnd: string; type: string; length: string; color: string; path: string }
export interface ContainerRecord { id: number; name: string; ip: string; pods: number; cpu: number; mem: number; network: string; status: string }
export interface NetworkLayer { id: number; name: string; devices: string; connections: string }

// ── Assets ──
export interface ServerMetric { id: number; name: string; cpu: number; gpu: number; mem: number; disk: number; netIn: number; netOut: number; power: number }
export interface ServerRecord { id: number; name: string; rack: string; os: string; uptime: string; status: string }
export interface HardwareModel { id: number; model: string; vendor: string; category: string; count: number; avgAge: string }
export interface AssetStatus { id: number; status: string; count: number; pct: number; color: string }
export interface Part { id: number; name: string; category: string; inStock: number; reserved: number; reorder: number; unit: string }
export interface VmHost { id: number; name: string; type: string; physical: boolean; vms: number; vcpus: number; vmem: number }
export interface OsRecord { id: number; os: string; count: number; pct: number }
export interface ItemAge { id: number; name: string; customer: string; purchased: string; age: string; warranty: string; status: string }
export interface AssetAttribute { id: number; attribute: string; value: string; count: number }
export interface LifecyclePhase { id: number; phase: string; count: number; items: string; eta: string }
export interface ImportExportRecord { id: number; date: string; type: string; file: string; records: number; status: string }

// ── Security ──
export interface AccessLog { id: number; timestamp: string; door: string; person: string; method: string; result: string }
export interface FirewallEvent { id: number; timestamp: string; type: string; sourceIP: string; destPort: number; action: string; severity: string }
export interface IdsAlert { id: number; timestamp: string; alert: string; protocol: string; confidence: number; status: string }
export interface BlockedIp { id: number; ip: string; reason: string; incidents: number; firstSeen: string; status: string }
export interface AccessRole { id: number; name: string; permissionCount: number; userCount: number; description: string; status: string }
export interface AccessRoleUser { id: number; name: string; role: string; department: string; lastLogin: string; status: string }
export interface CctvCamera { id: number; name: string; location: string; status: string; recording: string; resolution: string; storage: string; lastFrame: string }
export interface RecordingStat { id: number; location: string; cameras: number; totalStorage: string; usedStorage: string; retention: string }
export interface SecurityAudit { id: number; timestamp: string; user: string; action: string; resource: string; result: string; details: string }

// ── Alarms ──
export interface AlarmHistory { id: number; triggered: string; resolved: string; source: string; severity: string; message: string }
export interface NotificationRecord { id: number; type: string; message: string; time: string; read: boolean }
export interface EmailRecipient { id: number; email: string; frequency: string; alertTypes: string; active: boolean }
export interface SmsRecipient { id: number; name: string; phone: string; severity: string; enabled: boolean }
export interface PushChannel { id: number; name: string; type: string; endpoint: string; alertTypes: string; active: boolean }
export interface AlertRule { id: number; name: string; condition: string; threshold: string; action: string; enabled: boolean }
export interface EscalationPolicy { id: number; name: string; description: string; active: boolean; levels: string }
export interface MaintenanceWindow { id: number; component: string; startTime: string; endTime: string; impact: string; status: string }
export interface WarningPattern { id: number; name: string; frequency: string; lastOccurrence: string; trend: string }

// ── Changes ──
export interface CompletedRequest { id: number; requestId: string; requester: string; type: string; priority: string; requestedDate: string; completionDate: string; duration: number }
export interface RequestType { id: number; name: string; description: string; color: string; count: number }
export interface WorkOrder { id: number; workOrderId: string; changeRequest: string; technician: string; priority: string; status: string; dueDate: string }
export interface ChangeAudit { id: number; auditId: string; changeRequest: string; action: string; user: string; timestamp: string; details: string }
export interface ApprovalWorkflow { id: number; workflowId: string; name: string; type: string; priority: string; steps: string }

// ── Reports ──
export interface PueTrend { id: number; month: string; pueValue: number; trend: string; progressPercent: number; status: string }
export interface CapacityReport { id: number; name: string; color: string; currentUsage: number; available: string; total: string; forecast: string; forecastDays: number }
export interface ExportCategory { id: number; name: string; description: string }
export interface ExportTemplate { id: number; name: string; pages: number; description: string }
export interface ScheduledReport { id: number; name: string; frequency: string; recipients: string; lastSent: string; nextRun: string; status: string }
export interface ComplianceSection { id: number; sectionId: string; name: string; description: string; items: string }
export interface ReportMetric { id: number; name: string; value: string; unit: string; color: string; category: string }
export interface HeatmapData { id: number; category: string; rowLabel: string; colLabel: string; value: number; displayValue: string }

// ── Settings ──
export interface RbacRole { id: number; name: string; userCount: number; dashboard: boolean; power: boolean; cooling: boolean; space: boolean; security: boolean }
export interface SystemSetting { id: number; key: string; value: string; category: string }
export interface SiteConfig { id: number; name: string; location: string; address: string; contactName: string; contactPhone: string; capacity: number; status: string }
export interface Integration { id: number; name: string; type: string; endpoint: string; status: string; lastSync: string; metrics: number }
export interface ApiKey { id: number; name: string; keyId: string; created: string; lastUsed: string; permissions: string; active: boolean }
export interface WebhookConfig { id: number; name: string; url: string; events: string; status: string; lastEvent: string; deliveryRate: number }
export interface SettingsAudit { id: number; timestamp: string; user: string; eventType: string; action: string; resource: string; result: string; ipAddress: string }
export interface LanguageConfig { id: number; name: string; code: string; native: string; translated: number; total: number; completion: number; lastUpdated: string }

// ── Visualization ──
export interface VisRack { id: number; rackId: string; hall: string; status: string; utilization: number }
export interface NetworkDevice { id: number; name: string; status: string; type: string; layer: string }
export interface VisLink { id: number; source: string; destination: string; status: string; utilization: number; speed: string }
export interface Widget { id: number; severity: string; message: string; time: string }
export interface WeatherLocation { id: number; name: string; region: string; temp: number; feelsLike: number; condition: string; windSpeed: number; humidity: number; pressure: number; aqi: number; impact: string; lastUpdated: string }

// ── Environmental (shared) ──
export interface SustainabilityMetric { id: number; category: string; name: string; value: string; unit: string; target: string; status: string; page: string }

// ── Original API clients ──
export const powerFeedsApi = crudApi<PowerFeed>('power-feeds')
export const coolingUnitsApi = crudApi<CoolingUnit>('cooling-units')
export const hallsApi = crudApi<Hall>('halls')
export const networkLinksApi = crudApi<NetworkLink>('network-links')
export const assetsApi = crudApi<Asset>('assets')
export const alarmsApi = crudApi<Alarm>('alarms')
export const changeRequestsApi = crudApi<ChangeRequest>('change-requests')
export const usersApi = crudApi<User>('users')
export const reportsApi = crudApi<Report>('reports')
export const securityIncidentsApi = crudApi<SecurityIncident>('security-incidents')
export const environmentalMetricsApi = crudApi<EnvironmentalMetric>('environmental-metrics')
export const sensorsApi = crudApi<Sensor>('sensors')

// ── Dashboard API clients ──
export const healthScoreCategoriesApi = crudApi<HealthScoreCategory>('health-score-categories')
export const sitesApi = crudApi<Site>('sites')
export const customDashboardsApi = crudApi<CustomDashboard>('custom-dashboards')

// ── Power API clients ──
export const pueReadingsApi = crudApi<PueReading>('pue-readings')
export const powerBudgetsApi = crudApi<PowerBudget>('power-budgets')
export const cabinetsApi = crudApi<Cabinet>('cabinets')
export const powerChainsApi = crudApi<PowerChain>('power-chains')
export const pdusApi = crudApi<Pdu>('pdus')
export const strandedPowersApi = crudApi<StrandedPower>('stranded-powers')
export const rectifiersApi = crudApi<Rectifier>('rectifiers')
export const dcBusesApi = crudApi<DcBus>('dc-buses')
export const batteryBanksApi = crudApi<BatteryBank>('battery-banks')
export const powerPhasesApi = crudApi<PowerPhase>('power-phases')
export const powerEventsApi = crudApi<PowerEvent>('power-events')
export const serverPowerRecordsApi = crudApi<ServerPowerRecord>('server-power-records')
export const portCapacityRecordsApi = crudApi<PortCapacityRecord>('port-capacity-records')
export const energyCostRecordsApi = crudApi<EnergyCostRecord>('energy-cost-records')
export const upsUnitsApi = crudApi<UpsUnit>('ups-units')
export const breakerRecordsApi = crudApi<BreakerRecord>('breaker-records')
export const pueMetricsApi = crudApi<PueMetric>('pue-metrics')

// ── Cooling API clients ──
export const coolingMetricsApi = crudApi<CoolingMetric>('cooling-metrics')
export const cabinetTemperaturesApi = crudApi<CabinetTemperature>('cabinet-temperatures')
export const cabinetHumiditiesApi = crudApi<CabinetHumidity>('cabinet-humidities')
export const sensorTrendsApi = crudApi<SensorTrend>('sensor-trends')
export const coolingCapacitiesApi = crudApi<CoolingCapacity>('cooling-capacities')
export const supplyReturnUnitsApi = crudApi<SupplyReturnUnit>('supply-return-units')
export const waterUsageRecordsApi = crudApi<WaterUsageRecord>('water-usage-records')
export const coolingSystemsApi = crudApi<CoolingSystem>('cooling-systems')
export const chillersApi = crudApi<Chiller>('chillers')
export const cabinetAirflowsApi = crudApi<CabinetAirflow>('cabinet-airflows')

// ── Space API clients ──
export const racksApi = crudApi<Rack>('racks')
export const rackGapsApi = crudApi<RackGap>('rack-gaps')
export const cabinetGroupsApi = crudApi<CabinetGroup>('cabinet-groups')
export const floorPlanRacksApi = crudApi<FloorPlanRack>('floor-plan-racks')
export const rackDevicesApi = crudApi<RackDevice>('rack-devices')
export const scenariosApi = crudApi<Scenario>('scenarios')
export const siteNodesApi = crudApi<SiteNode>('site-nodes')
export const storageSystemsApi = crudApi<StorageSystem>('storage-systems')
export const pendingAssetsApi = crudApi<PendingAsset>('pending-assets')

// ── Network API clients ──
export const trafficRecordsApi = crudApi<TrafficRecord>('traffic-records')
export const dataSwitchesApi = crudApi<DataSwitch>('data-switches')
export const powerPortRecordsApi = crudApi<PowerPortRecord>('power-port-records')
export const portRecordsApi = crudApi<PortRecord>('port-records')
export const cablesApi = crudApi<Cable>('cables')
export const containerRecordsApi = crudApi<ContainerRecord>('container-records')
export const networkLayersApi = crudApi<NetworkLayer>('network-layers')

// ── Assets API clients ──
export const serverMetricsApi = crudApi<ServerMetric>('server-metrics')
export const serverRecordsApi = crudApi<ServerRecord>('server-records')
export const hardwareModelsApi = crudApi<HardwareModel>('hardware-models')
export const assetStatusesApi = crudApi<AssetStatus>('asset-statuses')
export const partsApi = crudApi<Part>('parts')
export const vmHostsApi = crudApi<VmHost>('vm-hosts')
export const osRecordsApi = crudApi<OsRecord>('os-records')
export const itemAgesApi = crudApi<ItemAge>('item-ages')
export const assetAttributesApi = crudApi<AssetAttribute>('asset-attributes')
export const lifecyclePhasesApi = crudApi<LifecyclePhase>('lifecycle-phases')
export const importExportRecordsApi = crudApi<ImportExportRecord>('import-export-records')

// ── Security API clients ──
export const accessLogsApi = crudApi<AccessLog>('access-logs')
export const firewallEventsApi = crudApi<FirewallEvent>('firewall-events')
export const idsAlertsApi = crudApi<IdsAlert>('ids-alerts')
export const blockedIpsApi = crudApi<BlockedIp>('blocked-ips')
export const accessRolesApi = crudApi<AccessRole>('access-roles')
export const accessRoleUsersApi = crudApi<AccessRoleUser>('access-role-users')
export const cctvCamerasApi = crudApi<CctvCamera>('cctv-cameras')
export const recordingStatsApi = crudApi<RecordingStat>('recording-stats')
export const securityAuditsApi = crudApi<SecurityAudit>('security-audits')

// ── Alarms API clients ──
export const alarmHistoriesApi = crudApi<AlarmHistory>('alarm-histories')
export const notificationRecordsApi = crudApi<NotificationRecord>('notification-records')
export const emailRecipientsApi = crudApi<EmailRecipient>('email-recipients')
export const smsRecipientsApi = crudApi<SmsRecipient>('sms-recipients')
export const pushChannelsApi = crudApi<PushChannel>('push-channels')
export const alertRulesApi = crudApi<AlertRule>('alert-rules')
export const escalationPoliciesApi = crudApi<EscalationPolicy>('escalation-policies')
export const maintenanceWindowsApi = crudApi<MaintenanceWindow>('maintenance-windows')
export const warningPatternsApi = crudApi<WarningPattern>('warning-patterns')

// ── Changes API clients ──
export const completedRequestsApi = crudApi<CompletedRequest>('completed-requests')
export const requestTypesApi = crudApi<RequestType>('request-types')
export const workOrdersApi = crudApi<WorkOrder>('work-orders')
export const changeAuditsApi = crudApi<ChangeAudit>('change-audits')
export const approvalWorkflowsApi = crudApi<ApprovalWorkflow>('approval-workflows')

// ── Reports API clients ──
export const pueTrendsApi = crudApi<PueTrend>('pue-trends')
export const capacityReportsApi = crudApi<CapacityReport>('capacity-reports')
export const exportCategoriesApi = crudApi<ExportCategory>('export-categories')
export const exportTemplatesApi = crudApi<ExportTemplate>('export-templates')
export const scheduledReportsApi = crudApi<ScheduledReport>('scheduled-reports')
export const complianceSectionsApi = crudApi<ComplianceSection>('compliance-sections')
export const reportMetricsApi = crudApi<ReportMetric>('report-metrics')
export const heatmapDataApi = crudApi<HeatmapData>('heatmap-data')

// ── Settings API clients ──
export const rbacRolesApi = crudApi<RbacRole>('rbac-roles')
export const systemSettingsApi = crudApi<SystemSetting>('system-settings')
export const siteConfigsApi = crudApi<SiteConfig>('site-configs')
export const integrationsApi = crudApi<Integration>('integrations')
export const apiKeysApi = crudApi<ApiKey>('api-keys')
export const webhookConfigsApi = crudApi<WebhookConfig>('webhook-configs')
export const settingsAuditsApi = crudApi<SettingsAudit>('settings-audits')
export const languageConfigsApi = crudApi<LanguageConfig>('language-configs')

// ── Visualization API clients ──
export const visRacksApi = crudApi<VisRack>('vis-racks')
export const networkDevicesApi = crudApi<NetworkDevice>('network-devices')
export const visLinksApi = crudApi<VisLink>('vis-links')
export const widgetsApi = crudApi<Widget>('widgets')
export const weatherLocationsApi = crudApi<WeatherLocation>('weather-locations')

// ── Environmental (shared) API clients ──
export const sustainabilityMetricsApi = crudApi<SustainabilityMetric>('sustainability-metrics')

export async function seedDatabase() {
  return fetchApi<{ seeded: boolean }>('/seed', { method: 'POST' })
}
