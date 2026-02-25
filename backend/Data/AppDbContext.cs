using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Original entities
    public DbSet<Item> Items => Set<Item>();
    public DbSet<PowerFeed> PowerFeeds => Set<PowerFeed>();
    public DbSet<CoolingUnit> CoolingUnits => Set<CoolingUnit>();
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<NetworkLink> NetworkLinks => Set<NetworkLink>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Alarm> Alarms => Set<Alarm>();
    public DbSet<ChangeRequest> ChangeRequests => Set<ChangeRequest>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<SecurityIncident> SecurityIncidents => Set<SecurityIncident>();
    public DbSet<EnvironmentalMetric> EnvironmentalMetrics => Set<EnvironmentalMetric>();
    public DbSet<Sensor> Sensors => Set<Sensor>();

    // Dashboard
    public DbSet<HealthScoreCategory> HealthScoreCategories => Set<HealthScoreCategory>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<CustomDashboard> CustomDashboards => Set<CustomDashboard>();

    // Power
    public DbSet<PueReading> PueReadings => Set<PueReading>();
    public DbSet<PowerBudget> PowerBudgets => Set<PowerBudget>();
    public DbSet<Cabinet> Cabinets => Set<Cabinet>();
    public DbSet<PowerChain> PowerChains => Set<PowerChain>();
    public DbSet<Pdu> Pdus => Set<Pdu>();
    public DbSet<StrandedPower> StrandedPowers => Set<StrandedPower>();
    public DbSet<Rectifier> Rectifiers => Set<Rectifier>();
    public DbSet<DcBus> DcBuses => Set<DcBus>();
    public DbSet<BatteryBank> BatteryBanks => Set<BatteryBank>();
    public DbSet<PowerPhase> PowerPhases => Set<PowerPhase>();
    public DbSet<PowerEvent> PowerEvents => Set<PowerEvent>();
    public DbSet<ServerPowerRecord> ServerPowerRecords => Set<ServerPowerRecord>();
    public DbSet<PortCapacityRecord> PortCapacityRecords => Set<PortCapacityRecord>();
    public DbSet<EnergyCostRecord> EnergyCostRecords => Set<EnergyCostRecord>();
    public DbSet<UpsUnit> UpsUnits => Set<UpsUnit>();
    public DbSet<BreakerRecord> BreakerRecords => Set<BreakerRecord>();
    public DbSet<PueMetric> PueMetrics => Set<PueMetric>();

    // Cooling
    public DbSet<CoolingMetric> CoolingMetrics => Set<CoolingMetric>();
    public DbSet<CabinetTemperature> CabinetTemperatures => Set<CabinetTemperature>();
    public DbSet<CabinetHumidity> CabinetHumidities => Set<CabinetHumidity>();
    public DbSet<SensorTrend> SensorTrends => Set<SensorTrend>();
    public DbSet<CoolingCapacity> CoolingCapacities => Set<CoolingCapacity>();
    public DbSet<SupplyReturnUnit> SupplyReturnUnits => Set<SupplyReturnUnit>();
    public DbSet<WaterUsageRecord> WaterUsageRecords => Set<WaterUsageRecord>();
    public DbSet<CoolingSystem> CoolingSystems => Set<CoolingSystem>();
    public DbSet<Chiller> Chillers => Set<Chiller>();
    public DbSet<CabinetAirflow> CabinetAirflows => Set<CabinetAirflow>();

    // Space
    public DbSet<Rack> Racks => Set<Rack>();
    public DbSet<RackGap> RackGaps => Set<RackGap>();
    public DbSet<CabinetGroup> CabinetGroups => Set<CabinetGroup>();
    public DbSet<FloorPlanRack> FloorPlanRacks => Set<FloorPlanRack>();
    public DbSet<RackDevice> RackDevices => Set<RackDevice>();
    public DbSet<Scenario> Scenarios => Set<Scenario>();
    public DbSet<SiteNode> SiteNodes => Set<SiteNode>();
    public DbSet<StorageSystem> StorageSystems => Set<StorageSystem>();
    public DbSet<PendingAsset> PendingAssets => Set<PendingAsset>();

    // Network
    public DbSet<TrafficRecord> TrafficRecords => Set<TrafficRecord>();
    public DbSet<DataSwitch> DataSwitches => Set<DataSwitch>();
    public DbSet<PowerPortRecord> PowerPortRecords => Set<PowerPortRecord>();
    public DbSet<PortRecord> PortRecords => Set<PortRecord>();
    public DbSet<Cable> Cables => Set<Cable>();
    public DbSet<ContainerRecord> ContainerRecords => Set<ContainerRecord>();
    public DbSet<NetworkLayer> NetworkLayers => Set<NetworkLayer>();

    // Assets
    public DbSet<ServerMetric> ServerMetrics => Set<ServerMetric>();
    public DbSet<ServerRecord> ServerRecords => Set<ServerRecord>();
    public DbSet<HardwareModel> HardwareModels => Set<HardwareModel>();
    public DbSet<AssetStatus> AssetStatuses => Set<AssetStatus>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<VmHost> VmHosts => Set<VmHost>();
    public DbSet<OsRecord> OsRecords => Set<OsRecord>();
    public DbSet<ItemAge> ItemAges => Set<ItemAge>();
    public DbSet<AssetAttribute> AssetAttributes => Set<AssetAttribute>();
    public DbSet<LifecyclePhase> LifecyclePhases => Set<LifecyclePhase>();
    public DbSet<ImportExportRecord> ImportExportRecords => Set<ImportExportRecord>();

    // Security
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();
    public DbSet<FirewallEvent> FirewallEvents => Set<FirewallEvent>();
    public DbSet<IdsAlert> IdsAlerts => Set<IdsAlert>();
    public DbSet<BlockedIp> BlockedIps => Set<BlockedIp>();
    public DbSet<AccessRole> AccessRoles => Set<AccessRole>();
    public DbSet<AccessRoleUser> AccessRoleUsers => Set<AccessRoleUser>();
    public DbSet<CctvCamera> CctvCameras => Set<CctvCamera>();
    public DbSet<RecordingStat> RecordingStats => Set<RecordingStat>();
    public DbSet<SecurityAudit> SecurityAudits => Set<SecurityAudit>();

    // Alarms
    public DbSet<AlarmHistory> AlarmHistories => Set<AlarmHistory>();
    public DbSet<NotificationRecord> NotificationRecords => Set<NotificationRecord>();
    public DbSet<EmailRecipient> EmailRecipients => Set<EmailRecipient>();
    public DbSet<SmsRecipient> SmsRecipients => Set<SmsRecipient>();
    public DbSet<PushChannel> PushChannels => Set<PushChannel>();
    public DbSet<AlertRule> AlertRules => Set<AlertRule>();
    public DbSet<EscalationPolicy> EscalationPolicies => Set<EscalationPolicy>();
    public DbSet<MaintenanceWindow> MaintenanceWindows => Set<MaintenanceWindow>();
    public DbSet<WarningPattern> WarningPatterns => Set<WarningPattern>();

    // Changes
    public DbSet<CompletedRequest> CompletedRequests => Set<CompletedRequest>();
    public DbSet<RequestType> RequestTypes => Set<RequestType>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<ChangeAudit> ChangeAudits => Set<ChangeAudit>();
    public DbSet<ApprovalWorkflow> ApprovalWorkflows => Set<ApprovalWorkflow>();

    // Reports
    public DbSet<PueTrend> PueTrends => Set<PueTrend>();
    public DbSet<CapacityReport> CapacityReports => Set<CapacityReport>();
    public DbSet<ExportCategory> ExportCategories => Set<ExportCategory>();
    public DbSet<ExportTemplate> ExportTemplates => Set<ExportTemplate>();
    public DbSet<ScheduledReport> ScheduledReports => Set<ScheduledReport>();
    public DbSet<ComplianceSection> ComplianceSections => Set<ComplianceSection>();
    public DbSet<ReportMetric> ReportMetrics => Set<ReportMetric>();
    public DbSet<HeatmapData> HeatmapDatas => Set<HeatmapData>();

    // Settings
    public DbSet<RbacRole> RbacRoles => Set<RbacRole>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<SiteConfig> SiteConfigs => Set<SiteConfig>();
    public DbSet<Integration> Integrations => Set<Integration>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<WebhookConfig> WebhookConfigs => Set<WebhookConfig>();
    public DbSet<SettingsAudit> SettingsAudits => Set<SettingsAudit>();
    public DbSet<LanguageConfig> LanguageConfigs => Set<LanguageConfig>();

    // Visualization
    public DbSet<VisRack> VisRacks => Set<VisRack>();
    public DbSet<NetworkDevice> NetworkDevices => Set<NetworkDevice>();
    public DbSet<VisLink> VisLinks => Set<VisLink>();
    public DbSet<Widget> Widgets => Set<Widget>();
    public DbSet<WeatherLocation> WeatherLocations => Set<WeatherLocation>();

    // Environmental (shared)
    public DbSet<SustainabilityMetric> SustainabilityMetrics => Set<SustainabilityMetric>();
}
