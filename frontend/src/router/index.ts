import { createRouter, createWebHashHistory } from 'vue-router'
import MainLayout from '../layouts/MainLayout.vue'
import DashboardOverview from '../views/dashboard/DashboardOverview.vue'
import HealthScore from '../views/dashboard/HealthScore.vue'
import GlobalMap from '../views/dashboard/GlobalMap.vue'
import CustomDashboards from '../views/dashboard/CustomDashboards.vue'
import HelicopterView from '../views/dashboard/HelicopterView.vue'
import PowerOverview from '../views/power/PowerOverview.vue'
import PueMonitoring from '../views/power/PueMonitoring.vue'
import PueAdvanced from '../views/power/PueAdvanced.vue'
import BudgetCapacity from '../views/power/BudgetCapacity.vue'
import ActivePower from '../views/power/ActivePower.vue'
import Distribution from '../views/power/Distribution.vue'
import StrandedAnalysis from '../views/power/StrandedAnalysis.vue'
import DcPowerControl from '../views/power/DcPowerControl.vue'
import BatteryManagement from '../views/power/BatteryManagement.vue'
import PowerQuality from '../views/power/PowerQuality.vue'
import ServerPower from '../views/power/ServerPower.vue'
import ServerHeatmap from '../views/power/ServerHeatmap.vue'
import PortCapacity from '../views/power/PortCapacity.vue'
import EnergyCost from '../views/power/EnergyCost.vue'
import UpsMonitoring from '../views/power/UpsMonitoring.vue'
import BreakersUtilization from '../views/power/BreakersUtilization.vue'
import CoolingOverview from '../views/cooling/CoolingOverview.vue'
import CoolingEfficiency from '../views/cooling/CoolingEfficiency.vue'
import Temperature from '../views/cooling/Temperature.vue'
import Humidity from '../views/cooling/Humidity.vue'
import SensorsTrend from '../views/cooling/SensorsTrend.vue'
import CapacityPlanning from '../views/cooling/CapacityPlanning.vue'
import SupplyReturn from '../views/cooling/SupplyReturn.vue'
import WaterUsage from '../views/cooling/WaterUsage.vue'
import SystemStatus from '../views/cooling/SystemStatus.vue'
import ChillerEfficiency from '../views/cooling/ChillerEfficiency.vue'
import AirflowMonitoring from '../views/cooling/AirflowMonitoring.vue'
import SensorManagement from '../views/cooling/SensorManagement.vue'
import SpaceOverview from '../views/space/SpaceOverview.vue'
import RackUtilization from '../views/space/RackUtilization.vue'
import GapAnalysis from '../views/space/GapAnalysis.vue'
import CabinetRemaining from '../views/space/CabinetRemaining.vue'
import FloorPlan from '../views/space/FloorPlan.vue'
import RackElevation from '../views/space/RackElevation.vue'
import WhatIfPlanning from '../views/space/WhatIfPlanning.vue'
import SiteHierarchy from '../views/space/SiteHierarchy.vue'
import StorageTracking from '../views/space/StorageTracking.vue'
import AssetPlacement from '../views/space/AssetPlacement.vue'
// Network
import NetworkOverview from '../views/network/NetworkOverview.vue'
import Traffic from '../views/network/Traffic.vue'
import DataPorts from '../views/network/DataPorts.vue'
import PowerPorts from '../views/network/PowerPorts.vue'
import PortProperties from '../views/network/PortProperties.vue'
import CableTracing from '../views/network/CableTracing.vue'
import Containers from '../views/network/Containers.vue'
import Topology from '../views/network/Topology.vue'
// Assets
import ServerMetrics from '../views/assets/ServerMetrics.vue'
import AssetsServerHeatmap from '../views/assets/AssetsServerHeatmap.vue'
import ServerStatus from '../views/assets/ServerStatus.vue'
import Inventory from '../views/assets/Inventory.vue'
import ByModel from '../views/assets/ByModel.vue'
import ByStatus from '../views/assets/ByStatus.vue'
import PartsInventory from '../views/assets/PartsInventory.vue'
import PhysicalVirtual from '../views/assets/PhysicalVirtual.vue'
import ByOS from '../views/assets/ByOS.vue'
import ItemAge from '../views/assets/ItemAge.vue'
import ByAttributes from '../views/assets/ByAttributes.vue'
import Lifecycle from '../views/assets/Lifecycle.vue'
import BulkImportExport from '../views/assets/BulkImportExport.vue'
// Environmental
import CarbonFootprint from '../views/environmental/CarbonFootprint.vue'
import Sustainability from '../views/environmental/Sustainability.vue'
import Renewables from '../views/environmental/Renewables.vue'
import CarbonOffset from '../views/environmental/CarbonOffset.vue'
import ERE from '../views/environmental/ERE.vue'
import Emissions from '../views/environmental/Emissions.vue'
import EnergyStorage from '../views/environmental/EnergyStorage.vue'
import DcoiCompliance from '../views/environmental/DcoiCompliance.vue'
import EsgReporting from '../views/environmental/EsgReporting.vue'
// Security
import SecurityOverview from '../views/security/SecurityOverview.vue'
import PhysicalSecurity from '../views/security/PhysicalSecurity.vue'
import Cybersecurity from '../views/security/Cybersecurity.vue'
import AccessControl from '../views/security/AccessControl.vue'
import CctvIntegration from '../views/security/CctvIntegration.vue'
import SecurityAuditLog from '../views/security/SecurityAuditLog.vue'
// Alarms
import ActiveAlarms from '../views/alarms/ActiveAlarms.vue'
import AlarmHistory from '../views/alarms/AlarmHistory.vue'
import NotificationCenter from '../views/alarms/NotificationCenter.vue'
import EmailAlerts from '../views/alarms/EmailAlerts.vue'
import SmsNotifications from '../views/alarms/SmsNotifications.vue'
import PushNotifications from '../views/alarms/PushNotifications.vue'
import CustomRules from '../views/alarms/CustomRules.vue'
import EscalationPolicies from '../views/alarms/EscalationPolicies.vue'
import MaintenanceScheduling from '../views/alarms/MaintenanceScheduling.vue'
import AnalyzeWarnings from '../views/alarms/AnalyzeWarnings.vue'
// Changes
import PendingRequests from '../views/changes/PendingRequests.vue'
import CompletedRequests from '../views/changes/CompletedRequests.vue'
import RequestTypes from '../views/changes/RequestTypes.vue'
import WorkOrders from '../views/changes/WorkOrders.vue'
import ChangesAuditLog from '../views/changes/ChangesAuditLog.vue'
import ApprovalWorkflows from '../views/changes/ApprovalWorkflows.vue'
// Reports
import StandardReports from '../views/reports/StandardReports.vue'
import PueTrends from '../views/reports/PueTrends.vue'
import CapacityReports from '../views/reports/CapacityReports.vue'
import ExportCsv from '../views/reports/ExportCsv.vue'
import ExportPdf from '../views/reports/ExportPdf.vue'
import ScheduledReports from '../views/reports/ScheduledReports.vue'
import IsoCompliance from '../views/reports/IsoCompliance.vue'
import ModeToggle from '../views/reports/ModeToggle.vue'
import HeatmapReports from '../views/reports/HeatmapReports.vue'
// Settings
import UserManagement from '../views/settings/UserManagement.vue'
import Rbac from '../views/settings/Rbac.vue'
import SystemConfig from '../views/settings/SystemConfig.vue'
import SiteManagement from '../views/settings/SiteManagement.vue'
import Integrations from '../views/settings/Integrations.vue'
import ApiAccess from '../views/settings/ApiAccess.vue'
import Webhooks from '../views/settings/Webhooks.vue'
import SensorConfig from '../views/settings/SensorConfig.vue'
import SsoLdap from '../views/settings/SsoLdap.vue'
import SettingsAuditLogs from '../views/settings/SettingsAuditLogs.vue'
import MultiLanguage from '../views/settings/MultiLanguage.vue'
// Visualization
import VisFloorPlan from '../views/visualization/VisFloorPlan.vue'
import View3d from '../views/visualization/View3d.vue'
import VisRackElevation from '../views/visualization/VisRackElevation.vue'
import PowerHeatmap from '../views/visualization/PowerHeatmap.vue'
import TempHeatmap from '../views/visualization/TempHeatmap.vue'
import UtilHeatmap from '../views/visualization/UtilHeatmap.vue'
import VisTopology from '../views/visualization/VisTopology.vue'
import CustomWidgets from '../views/visualization/CustomWidgets.vue'
import WeatherWidgets from '../views/visualization/WeatherWidgets.vue'
import ContainerMap from '../views/visualization/ContainerMap.vue'

const routes = [
  {
    path: '/',
    component: MainLayout,
    children: [
      // Redirect root to dashboard
      { path: '', redirect: '/dashboard' },

      // Dashboard
      { path: 'dashboard', component: DashboardOverview, meta: { title: 'Overview', module: 'Dashboard', requiredTier: 'standard', description: 'Enterprise overview with KPI cards' } },
      { path: 'dashboard/health-score', component: HealthScore, meta: { title: 'Health Score', module: 'Dashboard', requiredTier: 'plus', description: 'Enterprise health score across all systems' } },
      { path: 'dashboard/global-map', component: GlobalMap, meta: { title: 'Global Map', module: 'Dashboard', requiredTier: 'plus', description: 'Multi-site global map view' } },
      { path: 'dashboard/custom', component: CustomDashboards, meta: { title: 'Custom Dashboards', module: 'Dashboard', requiredTier: 'plus', description: 'Drag & drop custom dashboard widgets' } },
      { path: 'dashboard/helicopter', component: HelicopterView, meta: { title: 'Helicopter View', module: 'Dashboard', requiredTier: 'plus', description: "Bird's-eye category selector" } },

      // Power Management
      { path: 'power', component: PowerOverview, meta: { title: 'Power Overview', module: 'Power Management', requiredTier: 'standard', description: 'Total capacity, usage, and efficiency' } },
      { path: 'power/pue', component: PueMonitoring, meta: { title: 'PUE Monitoring', module: 'Power Management', requiredTier: 'standard', description: 'Power Usage Effectiveness (PUE1)' } },
      { path: 'power/pue-advanced', component: PueAdvanced, meta: { title: 'PUE Advanced', module: 'Power Management', requiredTier: 'plus', description: 'PUE1/PUE2/PUE3, target, std deviation' } },
      { path: 'power/budget-capacity', component: BudgetCapacity, meta: { title: 'Budget Capacity', module: 'Power Management', requiredTier: 'plus', description: 'Budget power capacity per site' } },
      { path: 'power/active-power', component: ActivePower, meta: { title: 'Active Power', module: 'Power Management', requiredTier: 'plus', description: 'Actual active power per sub-location/cabinet' } },
      { path: 'power/distribution', component: Distribution, meta: { title: 'Distribution & Redundancy', module: 'Power Management', requiredTier: 'plus', description: 'Power distribution and redundancy overview' } },
      { path: 'power/stranded', component: StrandedAnalysis, meta: { title: 'Stranded Analysis', module: 'Power Management', requiredTier: 'platinum', description: 'Stranded power analysis' } },
      { path: 'power/dpc', component: DcPowerControl, meta: { title: 'DC Power Control', module: 'Power Management', requiredTier: 'plus', description: 'Rectifier and bus data' } },
      { path: 'power/battery', component: BatteryManagement, meta: { title: 'Battery Management', module: 'Power Management', requiredTier: 'plus', description: 'SOC, SOH, cell-level battery data' } },
      { path: 'power/quality', component: PowerQuality, meta: { title: 'Power Quality', module: 'Power Management', requiredTier: 'platinum', description: 'THD, harmonics, sag/swell analysis' } },
      { path: 'power/server-power', component: ServerPower, meta: { title: 'Server Power', module: 'Power Management', requiredTier: 'standard', description: 'Per-server power usage (kW)' } },
      { path: 'power/server-heatmap', component: ServerHeatmap, meta: { title: 'Server Heatmap', module: 'Power Management', requiredTier: 'plus', description: 'Power usage heatmap (time × date)' } },
      { path: 'power/port-capacity', component: PortCapacity, meta: { title: 'Port Capacity', module: 'Power Management', requiredTier: 'plus', description: 'Power ports capacity & trend' } },
      { path: 'power/energy-cost', component: EnergyCost, meta: { title: 'Energy Cost & Billing', module: 'Power Management', requiredTier: 'platinum', description: 'Energy cost tracking and billing' } },
      { path: 'power/ups', component: UpsMonitoring, meta: { title: 'UPS Monitoring', module: 'Power Management', requiredTier: 'standard', description: 'UPS status and analytics' } },
      { path: 'power/breakers', component: BreakersUtilization, meta: { title: 'Breakers Utilization', module: 'Power Management', requiredTier: 'platinum', description: 'Power chain breakers utilization' } },

      // Cooling Management
      { path: 'cooling', component: CoolingOverview, meta: { title: 'Cooling Overview', module: 'Cooling Management', requiredTier: 'standard', description: 'Capacity, usage, and temperature' } },
      { path: 'cooling/efficiency', component: CoolingEfficiency, meta: { title: 'Cooling Efficiency', module: 'Cooling Management', requiredTier: 'plus', description: 'CER, PUEc, EER, COP metrics' } },
      { path: 'cooling/temperature', component: Temperature, meta: { title: 'Temperature', module: 'Cooling Management', requiredTier: 'standard', description: 'Temperature monitoring per cabinet' } },
      { path: 'cooling/humidity', component: Humidity, meta: { title: 'Humidity', module: 'Cooling Management', requiredTier: 'plus', description: 'Humidity monitoring per cabinet' } },
      { path: 'cooling/sensors-trend', component: SensorsTrend, meta: { title: 'Sensors Trend', module: 'Cooling Management', requiredTier: 'plus', description: 'Historical temperature sensor trends' } },
      { path: 'cooling/capacity-planning', component: CapacityPlanning, meta: { title: 'Capacity Planning', module: 'Cooling Management', requiredTier: 'platinum', description: 'Cooling capacity planning' } },
      { path: 'cooling/supply-return', component: SupplyReturn, meta: { title: 'Supply & Return Air', module: 'Cooling Management', requiredTier: 'plus', description: 'Supply and return air monitoring' } },
      { path: 'cooling/wue', component: WaterUsage, meta: { title: 'Water Usage (WUE)', module: 'Cooling Management', requiredTier: 'platinum', description: 'Water Usage Effectiveness' } },
      { path: 'cooling/system-status', component: SystemStatus, meta: { title: 'System Status', module: 'Cooling Management', requiredTier: 'standard', description: 'Cooling system status & downtime' } },
      { path: 'cooling/chiller', component: ChillerEfficiency, meta: { title: 'Chiller Efficiency', module: 'Cooling Management', requiredTier: 'platinum', description: 'Chiller plant efficiency' } },
      { path: 'cooling/airflow', component: AirflowMonitoring, meta: { title: 'Airflow Monitoring', module: 'Cooling Management', requiredTier: 'platinum', description: 'Airflow monitoring (CFM/kW)' } },
      { path: 'cooling/sensors', component: SensorManagement, meta: { title: 'Sensor Management', module: 'Cooling Management', requiredTier: 'plus', description: 'Register and configure sensors' } },

      // Space Management
      { path: 'space', component: SpaceOverview, meta: { title: 'Space Overview', module: 'Space Management', requiredTier: 'standard', description: 'Rack count, available sqft, utilization' } },
      { path: 'space/rack-utilization', component: RackUtilization, meta: { title: 'Rack Utilization', module: 'Space Management', requiredTier: 'standard', description: 'Per-rack visual utilization bars' } },
      { path: 'space/gap-analysis', component: GapAnalysis, meta: { title: 'Gap Analysis', module: 'Space Management', requiredTier: 'plus', description: '1U/2U/3U gap identification' } },
      { path: 'space/cabinet-remaining', component: CabinetRemaining, meta: { title: 'Cabinet Remaining', module: 'Space Management', requiredTier: 'plus', description: 'Cabinet space remaining by group/app' } },
      { path: 'space/floor-plan', component: FloorPlan, meta: { title: 'Floor Plan', module: 'Space Management', requiredTier: 'plus', description: '2D floor plan view' } },
      { path: 'space/rack-elevation', component: RackElevation, meta: { title: 'Rack Elevation', module: 'Space Management', requiredTier: 'plus', description: 'Device-level rack elevation view' } },
      { path: 'space/what-if', component: WhatIfPlanning, meta: { title: 'What-If Planning', module: 'Space Management', requiredTier: 'platinum', description: 'What-if capacity planning scenarios' } },
      { path: 'space/site-hierarchy', component: SiteHierarchy, meta: { title: 'Site Hierarchy', module: 'Space Management', requiredTier: 'standard', description: 'Site / location hierarchy management' } },
      { path: 'space/storage', component: StorageTracking, meta: { title: 'Storage Tracking', module: 'Space Management', requiredTier: 'standard', description: 'Storage usage tracking' } },
      { path: 'space/asset-placement', component: AssetPlacement, meta: { title: 'Asset Placement', module: 'Space Management', requiredTier: 'platinum', description: 'Drag-and-drop asset placement in rack' } },

      // Network & Connectivity
      { path: 'network', component: NetworkOverview, meta: { title: 'Network Overview', module: 'Network & Connectivity', requiredTier: 'standard', description: 'Bandwidth, IP, redundancy overview' } },
      { path: 'network/traffic', component: Traffic, meta: { title: 'Traffic', module: 'Network & Connectivity', requiredTier: 'standard', description: 'Inbound/outbound/total traffic' } },
      { path: 'network/data-ports', component: DataPorts, meta: { title: 'Data Ports', module: 'Network & Connectivity', requiredTier: 'plus', description: 'Data port capacity and trends' } },
      { path: 'network/power-ports', component: PowerPorts, meta: { title: 'Power Ports', module: 'Network & Connectivity', requiredTier: 'plus', description: 'Power port capacity and trends' } },
      { path: 'network/port-properties', component: PortProperties, meta: { title: 'Port Properties', module: 'Network & Connectivity', requiredTier: 'plus', description: 'Port color, connected/free status' } },
      { path: 'network/cable-tracing', component: CableTracing, meta: { title: 'Cable Tracing', module: 'Network & Connectivity', requiredTier: 'platinum', description: 'Connectivity mapping and cable tracing' } },
      { path: 'network/containers', component: Containers, meta: { title: 'Containers', module: 'Network & Connectivity', requiredTier: 'standard', description: 'Container management (IP, status)' } },
      { path: 'network/topology', component: Topology, meta: { title: 'Topology', module: 'Network & Connectivity', requiredTier: 'platinum', description: 'Network topology visualization' } },

      // Server & Assets
      { path: 'assets/server-metrics', component: ServerMetrics, meta: { title: 'Server Metrics', module: 'Server & Assets', requiredTier: 'standard', description: 'CPU, GPU, network, power metrics' } },
      { path: 'assets/server-heatmap', component: AssetsServerHeatmap, meta: { title: 'Server Heatmap', module: 'Server & Assets', requiredTier: 'plus', description: 'Color-coded server metrics heatmap' } },
      { path: 'assets/server-status', component: ServerStatus, meta: { title: 'Server Status', module: 'Server & Assets', requiredTier: 'standard', description: 'Online/offline/maintenance status' } },
      { path: 'assets/inventory', component: Inventory, meta: { title: 'Inventory', module: 'Server & Assets', requiredTier: 'standard', description: 'Items inventory by class' } },
      { path: 'assets/by-model', component: ByModel, meta: { title: 'By Model', module: 'Server & Assets', requiredTier: 'plus', description: 'Items by model attributes' } },
      { path: 'assets/by-status', component: ByStatus, meta: { title: 'By Status', module: 'Server & Assets', requiredTier: 'plus', description: 'Items by status (archived, installed, planned)' } },
      { path: 'assets/parts', component: PartsInventory, meta: { title: 'Parts Inventory', module: 'Server & Assets', requiredTier: 'plus', description: 'Parts inventory management' } },
      { path: 'assets/physical-virtual', component: PhysicalVirtual, meta: { title: 'Physical / Virtual', module: 'Server & Assets', requiredTier: 'plus', description: 'Physical vs virtual tracking' } },
      { path: 'assets/by-os', component: ByOS, meta: { title: 'By OS', module: 'Server & Assets', requiredTier: 'plus', description: 'Hosts per operating system' } },
      { path: 'assets/item-age', component: ItemAge, meta: { title: 'Item Age Tracking', module: 'Server & Assets', requiredTier: 'platinum', description: 'Item age by customer/purchase date' } },
      { path: 'assets/by-attributes', component: ByAttributes, meta: { title: 'By Attributes', module: 'Server & Assets', requiredTier: 'platinum', description: 'Counts by item attributes' } },
      { path: 'assets/lifecycle', component: Lifecycle, meta: { title: 'Lifecycle', module: 'Server & Assets', requiredTier: 'platinum', description: 'Asset lifecycle management' } },
      { path: 'assets/bulk', component: BulkImportExport, meta: { title: 'Bulk Import / Export', module: 'Server & Assets', requiredTier: 'plus', description: 'Bulk import/export via CSV' } },

      // Environmental
      { path: 'environmental', component: CarbonFootprint, meta: { title: 'Carbon Footprint', module: 'Environmental', requiredTier: 'standard', description: 'Carbon footprint (Ton CO2e/year)' } },
      { path: 'environmental/sustainability', component: Sustainability, meta: { title: 'Sustainability', module: 'Environmental', requiredTier: 'plus', description: 'Sustainability metrics (% renewable)' } },
      { path: 'environmental/renewables', component: Renewables, meta: { title: 'Renewables', module: 'Environmental', requiredTier: 'platinum', description: 'Renewable energy breakdown (solar/wind)' } },
      { path: 'environmental/carbon-offset', component: CarbonOffset, meta: { title: 'Carbon Offset', module: 'Environmental', requiredTier: 'platinum', description: 'Carbon offset tracking' } },
      { path: 'environmental/ere', component: ERE, meta: { title: 'ERE', module: 'Environmental', requiredTier: 'platinum', description: 'Energy Reuse Effectiveness' } },
      { path: 'environmental/emissions', component: Emissions, meta: { title: 'Scope 1/2/3 Emissions', module: 'Environmental', requiredTier: 'platinum', description: 'Scope 1/2/3 emissions tracking' } },
      { path: 'environmental/energy-storage', component: EnergyStorage, meta: { title: 'Energy Storage', module: 'Environmental', requiredTier: 'platinum', description: 'Energy storage monitoring' } },
      { path: 'environmental/dcoi', component: DcoiCompliance, meta: { title: 'DCOI Compliance', module: 'Environmental', requiredTier: 'platinum', description: 'DCOI compliance dashboard' } },
      { path: 'environmental/esg', component: EsgReporting, meta: { title: 'ESG Reporting', module: 'Environmental', requiredTier: 'platinum', description: 'ESG reporting' } },

      // Security
      { path: 'security', component: SecurityOverview, meta: { title: 'Security Overview', module: 'Security', requiredTier: 'standard', description: 'Breach and access summary' } },
      { path: 'security/physical', component: PhysicalSecurity, meta: { title: 'Physical Security', module: 'Security', requiredTier: 'plus', description: 'RFID, fingerprint access logs' } },
      { path: 'security/cyber', component: Cybersecurity, meta: { title: 'Cybersecurity', module: 'Security', requiredTier: 'platinum', description: 'Firewall, intrusion detection' } },
      { path: 'security/access-control', component: AccessControl, meta: { title: 'Access Control', module: 'Security', requiredTier: 'standard', description: 'Role-based access control' } },
      { path: 'security/cctv', component: CctvIntegration, meta: { title: 'CCTV Integration', module: 'Security', requiredTier: 'platinum', description: 'Surveillance / CCTV integration' } },
      { path: 'security/audit-log', component: SecurityAuditLog, meta: { title: 'Audit Log', module: 'Security', requiredTier: 'plus', description: 'Security audit log' } },

      // Alarms & Notifications
      { path: 'alarms', component: ActiveAlarms, meta: { title: 'Active Alarms', module: 'Alarms & Notifications', requiredTier: 'standard', description: 'Critical and warning alarm count' } },
      { path: 'alarms/history', component: AlarmHistory, meta: { title: 'Alarm History', module: 'Alarms & Notifications', requiredTier: 'standard', description: 'Historical alarm records' } },
      { path: 'alarms/notifications', component: NotificationCenter, meta: { title: 'Notification Center', module: 'Alarms & Notifications', requiredTier: 'standard', description: 'Notification center with unread count' } },
      { path: 'alarms/email', component: EmailAlerts, meta: { title: 'Email Alerts', module: 'Alarms & Notifications', requiredTier: 'standard', description: 'Email notification settings' } },
      { path: 'alarms/sms', component: SmsNotifications, meta: { title: 'SMS Notifications', module: 'Alarms & Notifications', requiredTier: 'plus', description: 'SMS notification configuration' } },
      { path: 'alarms/push', component: PushNotifications, meta: { title: 'Push Notifications', module: 'Alarms & Notifications', requiredTier: 'platinum', description: 'Push notification configuration' } },
      { path: 'alarms/rules', component: CustomRules, meta: { title: 'Custom Rules', module: 'Alarms & Notifications', requiredTier: 'plus', description: 'Custom alert rules & thresholds' } },
      { path: 'alarms/escalation', component: EscalationPolicies, meta: { title: 'Escalation Policies', module: 'Alarms & Notifications', requiredTier: 'platinum', description: 'Escalation policy management' } },
      { path: 'alarms/maintenance', component: MaintenanceScheduling, meta: { title: 'Maintenance Scheduling', module: 'Alarms & Notifications', requiredTier: 'plus', description: 'Maintenance alert scheduling' } },
      { path: 'alarms/analyze', component: AnalyzeWarnings, meta: { title: 'Analyze Warnings', module: 'Alarms & Notifications', requiredTier: 'platinum', description: 'Automated warning analysis' } },

      // Change Management
      { path: 'changes', component: PendingRequests, meta: { title: 'Pending Requests', module: 'Change Management', requiredTier: 'plus', description: 'View pending change requests' } },
      { path: 'changes/completed', component: CompletedRequests, meta: { title: 'Completed Requests', module: 'Change Management', requiredTier: 'plus', description: 'Completed requests timeline' } },
      { path: 'changes/types', component: RequestTypes, meta: { title: 'Request Types', module: 'Change Management', requiredTier: 'plus', description: 'Request type management' } },
      { path: 'changes/work-orders', component: WorkOrders, meta: { title: 'Work Orders', module: 'Change Management', requiredTier: 'platinum', description: 'Work order creation & tracking' } },
      { path: 'changes/audit-log', component: ChangesAuditLog, meta: { title: 'Audit Log', module: 'Change Management', requiredTier: 'platinum', description: 'Full change audit log' } },
      { path: 'changes/approvals', component: ApprovalWorkflows, meta: { title: 'Approval Workflows', module: 'Change Management', requiredTier: 'platinum', description: 'Approval workflow management' } },

      // Reports & Analytics
      { path: 'reports', component: StandardReports, meta: { title: 'Standard Reports', module: 'Reports & Analytics', requiredTier: 'standard', description: 'Power, cooling, space reports' } },
      { path: 'reports/pue-trends', component: PueTrends, meta: { title: 'PUE Trends', module: 'Reports & Analytics', requiredTier: 'plus', description: 'Historical PUE trends' } },
      { path: 'reports/capacity', component: CapacityReports, meta: { title: 'Capacity Reports', module: 'Reports & Analytics', requiredTier: 'plus', description: 'Capacity analysis reports' } },
      { path: 'reports/export-csv', component: ExportCsv, meta: { title: 'Export CSV', module: 'Reports & Analytics', requiredTier: 'standard', description: 'Export data to CSV' } },
      { path: 'reports/export-pdf', component: ExportPdf, meta: { title: 'Export PDF / Excel', module: 'Reports & Analytics', requiredTier: 'plus', description: 'Export data to PDF or Excel' } },
      { path: 'reports/scheduled', component: ScheduledReports, meta: { title: 'Scheduled Reports', module: 'Reports & Analytics', requiredTier: 'platinum', description: 'Scheduled / automated reports' } },
      { path: 'reports/iso-compliance', component: IsoCompliance, meta: { title: 'ISO 22237 Compliance', module: 'Reports & Analytics', requiredTier: 'platinum', description: 'ISO/IEC 22237 compliance report' } },
      { path: 'reports/mode-toggle', component: ModeToggle, meta: { title: 'Concise / Detailed Mode', module: 'Reports & Analytics', requiredTier: 'plus', description: 'Toggle between concise and detailed report modes' } },
      { path: 'reports/heatmap', component: HeatmapReports, meta: { title: 'Heatmap Reports', module: 'Reports & Analytics', requiredTier: 'platinum', description: 'Heatmap information reports' } },

      // Settings
      { path: 'settings', component: UserManagement, meta: { title: 'User Management', module: 'Settings', requiredTier: 'standard', description: 'Manage users and permissions' } },
      { path: 'settings/rbac', component: Rbac, meta: { title: 'RBAC', module: 'Settings', requiredTier: 'standard', description: 'Role-based access control' } },
      { path: 'settings/system', component: SystemConfig, meta: { title: 'System Config', module: 'Settings', requiredTier: 'standard', description: 'System configuration' } },
      { path: 'settings/sites', component: SiteManagement, meta: { title: 'Site Management', module: 'Settings', requiredTier: 'standard', description: 'Manage data center sites' } },
      { path: 'settings/integrations', component: Integrations, meta: { title: 'Integrations', module: 'Settings', requiredTier: 'platinum', description: 'BMS/SCADA integration' } },
      { path: 'settings/api', component: ApiAccess, meta: { title: 'API Access', module: 'Settings', requiredTier: 'plus', description: 'API access configuration' } },
      { path: 'settings/webhooks', component: Webhooks, meta: { title: 'Webhooks', module: 'Settings', requiredTier: 'platinum', description: 'Webhook configuration' } },
      { path: 'settings/sensors', component: SensorConfig, meta: { title: 'Sensor Config', module: 'Settings', requiredTier: 'standard', description: 'Sensor configuration' } },
      { path: 'settings/sso', component: SsoLdap, meta: { title: 'SSO / LDAP', module: 'Settings', requiredTier: 'platinum', description: 'SSO / LDAP / Active Directory' } },
      { path: 'settings/audit-logs', component: SettingsAuditLogs, meta: { title: 'Audit Logs', module: 'Settings', requiredTier: 'plus', description: 'System audit logs' } },
      { path: 'settings/language', component: MultiLanguage, meta: { title: 'Multi-language', module: 'Settings', requiredTier: 'platinum', description: 'Multi-language support' } },

      // Visualization
      { path: 'visualization', component: VisFloorPlan, meta: { title: 'Floor Plan', module: 'Visualization', requiredTier: 'plus', description: '2D floor plan view' } },
      { path: 'visualization/3d', component: View3d, meta: { title: '3D View', module: 'Visualization', requiredTier: 'platinum', description: '3D data center view' } },
      { path: 'visualization/rack-elevation', component: VisRackElevation, meta: { title: 'Rack Elevation', module: 'Visualization', requiredTier: 'plus', description: 'Rack elevation view' } },
      { path: 'visualization/power-heatmap', component: PowerHeatmap, meta: { title: 'Power Heatmap', module: 'Visualization', requiredTier: 'plus', description: 'Power usage heatmap' } },
      { path: 'visualization/temp-heatmap', component: TempHeatmap, meta: { title: 'Temperature Heatmap', module: 'Visualization', requiredTier: 'platinum', description: 'Temperature heatmap' } },
      { path: 'visualization/util-heatmap', component: UtilHeatmap, meta: { title: 'Utilization Heatmap', module: 'Visualization', requiredTier: 'platinum', description: 'Utilization heatmap' } },
      { path: 'visualization/topology', component: VisTopology, meta: { title: 'Network Topology', module: 'Visualization', requiredTier: 'platinum', description: 'Network topology diagram' } },
      { path: 'visualization/widgets', component: CustomWidgets, meta: { title: 'Custom Widgets', module: 'Visualization', requiredTier: 'platinum', description: 'Custom HTML widgets' } },
      { path: 'visualization/weather', component: WeatherWidgets, meta: { title: 'Weather Widgets', module: 'Visualization', requiredTier: 'platinum', description: 'Weather / external data widgets' } },
      { path: 'visualization/container-map', component: ContainerMap, meta: { title: 'Container Map', module: 'Visualization', requiredTier: 'plus', description: 'Interactive container layout with component-level details' } },
    ],
  },
]

const router = createRouter({
  history: createWebHashHistory(),
  routes,
})

export default router
