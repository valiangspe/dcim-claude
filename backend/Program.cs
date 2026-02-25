using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db"
    )
);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// Auto-create database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ── Helper: map standard CRUD for an entity ──
void MapCrud<T>(string route, Func<AppDbContext, DbSet<T>> dbSet) where T : class
{
    var group = app.MapGroup(route);

    group.MapGet("/", async (AppDbContext db) => await dbSet(db).ToListAsync());

    group.MapGet("/{id}", async (int id, AppDbContext db) =>
        await dbSet(db).FindAsync(id) is T entity ? Results.Ok(entity) : Results.NotFound());

    group.MapPost("/", async (T entity, AppDbContext db) =>
    {
        dbSet(db).Add(entity);
        await db.SaveChangesAsync();
        return Results.Created($"{route}/{db.Entry(entity).Property("Id").CurrentValue}", entity);
    });

    group.MapPut("/{id}", async (int id, T input, AppDbContext db) =>
    {
        var entity = await dbSet(db).FindAsync(id);
        if (entity is null) return Results.NotFound();
        var entry = db.Entry(entity);
        foreach (var prop in entry.Properties)
        {
            if (prop.Metadata.Name == "Id") continue;
            prop.CurrentValue = db.Entry(input).Property(prop.Metadata.Name).CurrentValue;
        }
        await db.SaveChangesAsync();
        return Results.Ok(entity);
    });

    group.MapDelete("/{id}", async (int id, AppDbContext db) =>
    {
        var entity = await dbSet(db).FindAsync(id);
        if (entity is null) return Results.NotFound();
        dbSet(db).Remove(entity);
        await db.SaveChangesAsync();
        return Results.NoContent();
    });
}

// ── CRUD endpoints (original) ──
MapCrud<Item>("/api/items", db => db.Items);
MapCrud<PowerFeed>("/api/power-feeds", db => db.PowerFeeds);
MapCrud<CoolingUnit>("/api/cooling-units", db => db.CoolingUnits);
MapCrud<Hall>("/api/halls", db => db.Halls);
MapCrud<NetworkLink>("/api/network-links", db => db.NetworkLinks);
MapCrud<Asset>("/api/assets", db => db.Assets);
MapCrud<Alarm>("/api/alarms", db => db.Alarms);
MapCrud<ChangeRequest>("/api/change-requests", db => db.ChangeRequests);
MapCrud<User>("/api/users", db => db.Users);
MapCrud<Report>("/api/reports", db => db.Reports);
MapCrud<SecurityIncident>("/api/security-incidents", db => db.SecurityIncidents);
MapCrud<EnvironmentalMetric>("/api/environmental-metrics", db => db.EnvironmentalMetrics);
MapCrud<Sensor>("/api/sensors", db => db.Sensors);

// ── CRUD endpoints (dashboard) ──
MapCrud<HealthScoreCategory>("/api/health-score-categories", db => db.HealthScoreCategories);
MapCrud<Site>("/api/sites", db => db.Sites);
MapCrud<CustomDashboard>("/api/custom-dashboards", db => db.CustomDashboards);

// ── CRUD endpoints (power) ──
MapCrud<PueReading>("/api/pue-readings", db => db.PueReadings);
MapCrud<PowerBudget>("/api/power-budgets", db => db.PowerBudgets);
MapCrud<Cabinet>("/api/cabinets", db => db.Cabinets);
MapCrud<PowerChain>("/api/power-chains", db => db.PowerChains);
MapCrud<Pdu>("/api/pdus", db => db.Pdus);
MapCrud<StrandedPower>("/api/stranded-powers", db => db.StrandedPowers);
MapCrud<Rectifier>("/api/rectifiers", db => db.Rectifiers);
MapCrud<DcBus>("/api/dc-buses", db => db.DcBuses);
MapCrud<BatteryBank>("/api/battery-banks", db => db.BatteryBanks);
MapCrud<PowerPhase>("/api/power-phases", db => db.PowerPhases);
MapCrud<PowerEvent>("/api/power-events", db => db.PowerEvents);
MapCrud<ServerPowerRecord>("/api/server-power-records", db => db.ServerPowerRecords);
MapCrud<PortCapacityRecord>("/api/port-capacity-records", db => db.PortCapacityRecords);
MapCrud<EnergyCostRecord>("/api/energy-cost-records", db => db.EnergyCostRecords);
MapCrud<UpsUnit>("/api/ups-units", db => db.UpsUnits);
MapCrud<BreakerRecord>("/api/breaker-records", db => db.BreakerRecords);
MapCrud<PueMetric>("/api/pue-metrics", db => db.PueMetrics);

// ── CRUD endpoints (cooling) ──
MapCrud<CoolingMetric>("/api/cooling-metrics", db => db.CoolingMetrics);
MapCrud<CabinetTemperature>("/api/cabinet-temperatures", db => db.CabinetTemperatures);
MapCrud<CabinetHumidity>("/api/cabinet-humidities", db => db.CabinetHumidities);
MapCrud<SensorTrend>("/api/sensor-trends", db => db.SensorTrends);
MapCrud<CoolingCapacity>("/api/cooling-capacities", db => db.CoolingCapacities);
MapCrud<SupplyReturnUnit>("/api/supply-return-units", db => db.SupplyReturnUnits);
MapCrud<WaterUsageRecord>("/api/water-usage-records", db => db.WaterUsageRecords);
MapCrud<CoolingSystem>("/api/cooling-systems", db => db.CoolingSystems);
MapCrud<Chiller>("/api/chillers", db => db.Chillers);
MapCrud<CabinetAirflow>("/api/cabinet-airflows", db => db.CabinetAirflows);

// ── CRUD endpoints (space) ──
MapCrud<Rack>("/api/racks", db => db.Racks);
MapCrud<RackGap>("/api/rack-gaps", db => db.RackGaps);
MapCrud<CabinetGroup>("/api/cabinet-groups", db => db.CabinetGroups);
MapCrud<FloorPlanRack>("/api/floor-plan-racks", db => db.FloorPlanRacks);
MapCrud<RackDevice>("/api/rack-devices", db => db.RackDevices);
MapCrud<Scenario>("/api/scenarios", db => db.Scenarios);
MapCrud<SiteNode>("/api/site-nodes", db => db.SiteNodes);
MapCrud<StorageSystem>("/api/storage-systems", db => db.StorageSystems);
MapCrud<PendingAsset>("/api/pending-assets", db => db.PendingAssets);

// ── CRUD endpoints (network) ──
MapCrud<TrafficRecord>("/api/traffic-records", db => db.TrafficRecords);
MapCrud<DataSwitch>("/api/data-switches", db => db.DataSwitches);
MapCrud<PowerPortRecord>("/api/power-port-records", db => db.PowerPortRecords);
MapCrud<PortRecord>("/api/port-records", db => db.PortRecords);
MapCrud<Cable>("/api/cables", db => db.Cables);
MapCrud<ContainerRecord>("/api/container-records", db => db.ContainerRecords);
MapCrud<NetworkLayer>("/api/network-layers", db => db.NetworkLayers);

// ── CRUD endpoints (assets) ──
MapCrud<ServerMetric>("/api/server-metrics", db => db.ServerMetrics);
MapCrud<ServerRecord>("/api/server-records", db => db.ServerRecords);
MapCrud<HardwareModel>("/api/hardware-models", db => db.HardwareModels);
MapCrud<AssetStatus>("/api/asset-statuses", db => db.AssetStatuses);
MapCrud<Part>("/api/parts", db => db.Parts);
MapCrud<VmHost>("/api/vm-hosts", db => db.VmHosts);
MapCrud<OsRecord>("/api/os-records", db => db.OsRecords);
MapCrud<ItemAge>("/api/item-ages", db => db.ItemAges);
MapCrud<AssetAttribute>("/api/asset-attributes", db => db.AssetAttributes);
MapCrud<LifecyclePhase>("/api/lifecycle-phases", db => db.LifecyclePhases);
MapCrud<ImportExportRecord>("/api/import-export-records", db => db.ImportExportRecords);

// ── CRUD endpoints (security) ──
MapCrud<AccessLog>("/api/access-logs", db => db.AccessLogs);
MapCrud<FirewallEvent>("/api/firewall-events", db => db.FirewallEvents);
MapCrud<IdsAlert>("/api/ids-alerts", db => db.IdsAlerts);
MapCrud<BlockedIp>("/api/blocked-ips", db => db.BlockedIps);
MapCrud<AccessRole>("/api/access-roles", db => db.AccessRoles);
MapCrud<AccessRoleUser>("/api/access-role-users", db => db.AccessRoleUsers);
MapCrud<CctvCamera>("/api/cctv-cameras", db => db.CctvCameras);
MapCrud<RecordingStat>("/api/recording-stats", db => db.RecordingStats);
MapCrud<SecurityAudit>("/api/security-audits", db => db.SecurityAudits);

// ── CRUD endpoints (alarms) ──
MapCrud<AlarmHistory>("/api/alarm-histories", db => db.AlarmHistories);
MapCrud<NotificationRecord>("/api/notification-records", db => db.NotificationRecords);
MapCrud<EmailRecipient>("/api/email-recipients", db => db.EmailRecipients);
MapCrud<SmsRecipient>("/api/sms-recipients", db => db.SmsRecipients);
MapCrud<PushChannel>("/api/push-channels", db => db.PushChannels);
MapCrud<AlertRule>("/api/alert-rules", db => db.AlertRules);
MapCrud<EscalationPolicy>("/api/escalation-policies", db => db.EscalationPolicies);
MapCrud<MaintenanceWindow>("/api/maintenance-windows", db => db.MaintenanceWindows);
MapCrud<WarningPattern>("/api/warning-patterns", db => db.WarningPatterns);

// ── CRUD endpoints (changes) ──
MapCrud<CompletedRequest>("/api/completed-requests", db => db.CompletedRequests);
MapCrud<RequestType>("/api/request-types", db => db.RequestTypes);
MapCrud<WorkOrder>("/api/work-orders", db => db.WorkOrders);
MapCrud<ChangeAudit>("/api/change-audits", db => db.ChangeAudits);
MapCrud<ApprovalWorkflow>("/api/approval-workflows", db => db.ApprovalWorkflows);

// ── CRUD endpoints (reports) ──
MapCrud<PueTrend>("/api/pue-trends", db => db.PueTrends);
MapCrud<CapacityReport>("/api/capacity-reports", db => db.CapacityReports);
MapCrud<ExportCategory>("/api/export-categories", db => db.ExportCategories);
MapCrud<ExportTemplate>("/api/export-templates", db => db.ExportTemplates);
MapCrud<ScheduledReport>("/api/scheduled-reports", db => db.ScheduledReports);
MapCrud<ComplianceSection>("/api/compliance-sections", db => db.ComplianceSections);
MapCrud<ReportMetric>("/api/report-metrics", db => db.ReportMetrics);
MapCrud<HeatmapData>("/api/heatmap-data", db => db.HeatmapDatas);

// ── CRUD endpoints (settings) ──
MapCrud<RbacRole>("/api/rbac-roles", db => db.RbacRoles);
MapCrud<SystemSetting>("/api/system-settings", db => db.SystemSettings);
MapCrud<SiteConfig>("/api/site-configs", db => db.SiteConfigs);
MapCrud<Integration>("/api/integrations", db => db.Integrations);
MapCrud<ApiKey>("/api/api-keys", db => db.ApiKeys);
MapCrud<WebhookConfig>("/api/webhook-configs", db => db.WebhookConfigs);
MapCrud<SettingsAudit>("/api/settings-audits", db => db.SettingsAudits);
MapCrud<LanguageConfig>("/api/language-configs", db => db.LanguageConfigs);

// ── CRUD endpoints (visualization) ──
MapCrud<VisRack>("/api/vis-racks", db => db.VisRacks);
MapCrud<NetworkDevice>("/api/network-devices", db => db.NetworkDevices);
MapCrud<VisLink>("/api/vis-links", db => db.VisLinks);
MapCrud<Widget>("/api/widgets", db => db.Widgets);
MapCrud<WeatherLocation>("/api/weather-locations", db => db.WeatherLocations);

// ── CRUD endpoints (environmental shared) ──
MapCrud<SustainabilityMetric>("/api/sustainability-metrics", db => db.SustainabilityMetrics);

// ── Seed endpoint ──
app.MapPost("/api/seed", async (AppDbContext db) =>
{
    // Clear all tables
    db.Items.RemoveRange(db.Items);
    db.PowerFeeds.RemoveRange(db.PowerFeeds);
    db.CoolingUnits.RemoveRange(db.CoolingUnits);
    db.Halls.RemoveRange(db.Halls);
    db.NetworkLinks.RemoveRange(db.NetworkLinks);
    db.Assets.RemoveRange(db.Assets);
    db.Alarms.RemoveRange(db.Alarms);
    db.ChangeRequests.RemoveRange(db.ChangeRequests);
    db.Users.RemoveRange(db.Users);
    db.Reports.RemoveRange(db.Reports);
    db.SecurityIncidents.RemoveRange(db.SecurityIncidents);
    db.EnvironmentalMetrics.RemoveRange(db.EnvironmentalMetrics);
    db.Sensors.RemoveRange(db.Sensors);
    db.HealthScoreCategories.RemoveRange(db.HealthScoreCategories);
    db.Sites.RemoveRange(db.Sites);
    db.CustomDashboards.RemoveRange(db.CustomDashboards);
    db.PueReadings.RemoveRange(db.PueReadings);
    db.PowerBudgets.RemoveRange(db.PowerBudgets);
    db.Cabinets.RemoveRange(db.Cabinets);
    db.PowerChains.RemoveRange(db.PowerChains);
    db.Pdus.RemoveRange(db.Pdus);
    db.StrandedPowers.RemoveRange(db.StrandedPowers);
    db.Rectifiers.RemoveRange(db.Rectifiers);
    db.DcBuses.RemoveRange(db.DcBuses);
    db.BatteryBanks.RemoveRange(db.BatteryBanks);
    db.PowerPhases.RemoveRange(db.PowerPhases);
    db.PowerEvents.RemoveRange(db.PowerEvents);
    db.ServerPowerRecords.RemoveRange(db.ServerPowerRecords);
    db.PortCapacityRecords.RemoveRange(db.PortCapacityRecords);
    db.EnergyCostRecords.RemoveRange(db.EnergyCostRecords);
    db.UpsUnits.RemoveRange(db.UpsUnits);
    db.BreakerRecords.RemoveRange(db.BreakerRecords);
    db.PueMetrics.RemoveRange(db.PueMetrics);
    db.CoolingMetrics.RemoveRange(db.CoolingMetrics);
    db.CabinetTemperatures.RemoveRange(db.CabinetTemperatures);
    db.CabinetHumidities.RemoveRange(db.CabinetHumidities);
    db.SensorTrends.RemoveRange(db.SensorTrends);
    db.CoolingCapacities.RemoveRange(db.CoolingCapacities);
    db.SupplyReturnUnits.RemoveRange(db.SupplyReturnUnits);
    db.WaterUsageRecords.RemoveRange(db.WaterUsageRecords);
    db.CoolingSystems.RemoveRange(db.CoolingSystems);
    db.Chillers.RemoveRange(db.Chillers);
    db.CabinetAirflows.RemoveRange(db.CabinetAirflows);
    db.Racks.RemoveRange(db.Racks);
    db.RackGaps.RemoveRange(db.RackGaps);
    db.CabinetGroups.RemoveRange(db.CabinetGroups);
    db.FloorPlanRacks.RemoveRange(db.FloorPlanRacks);
    db.RackDevices.RemoveRange(db.RackDevices);
    db.Scenarios.RemoveRange(db.Scenarios);
    db.SiteNodes.RemoveRange(db.SiteNodes);
    db.StorageSystems.RemoveRange(db.StorageSystems);
    db.PendingAssets.RemoveRange(db.PendingAssets);
    db.TrafficRecords.RemoveRange(db.TrafficRecords);
    db.DataSwitches.RemoveRange(db.DataSwitches);
    db.PowerPortRecords.RemoveRange(db.PowerPortRecords);
    db.PortRecords.RemoveRange(db.PortRecords);
    db.Cables.RemoveRange(db.Cables);
    db.ContainerRecords.RemoveRange(db.ContainerRecords);
    db.NetworkLayers.RemoveRange(db.NetworkLayers);
    db.ServerMetrics.RemoveRange(db.ServerMetrics);
    db.ServerRecords.RemoveRange(db.ServerRecords);
    db.HardwareModels.RemoveRange(db.HardwareModels);
    db.AssetStatuses.RemoveRange(db.AssetStatuses);
    db.Parts.RemoveRange(db.Parts);
    db.VmHosts.RemoveRange(db.VmHosts);
    db.OsRecords.RemoveRange(db.OsRecords);
    db.ItemAges.RemoveRange(db.ItemAges);
    db.AssetAttributes.RemoveRange(db.AssetAttributes);
    db.LifecyclePhases.RemoveRange(db.LifecyclePhases);
    db.ImportExportRecords.RemoveRange(db.ImportExportRecords);
    db.AccessLogs.RemoveRange(db.AccessLogs);
    db.FirewallEvents.RemoveRange(db.FirewallEvents);
    db.IdsAlerts.RemoveRange(db.IdsAlerts);
    db.BlockedIps.RemoveRange(db.BlockedIps);
    db.AccessRoles.RemoveRange(db.AccessRoles);
    db.AccessRoleUsers.RemoveRange(db.AccessRoleUsers);
    db.CctvCameras.RemoveRange(db.CctvCameras);
    db.RecordingStats.RemoveRange(db.RecordingStats);
    db.SecurityAudits.RemoveRange(db.SecurityAudits);
    db.AlarmHistories.RemoveRange(db.AlarmHistories);
    db.NotificationRecords.RemoveRange(db.NotificationRecords);
    db.EmailRecipients.RemoveRange(db.EmailRecipients);
    db.SmsRecipients.RemoveRange(db.SmsRecipients);
    db.PushChannels.RemoveRange(db.PushChannels);
    db.AlertRules.RemoveRange(db.AlertRules);
    db.EscalationPolicies.RemoveRange(db.EscalationPolicies);
    db.MaintenanceWindows.RemoveRange(db.MaintenanceWindows);
    db.WarningPatterns.RemoveRange(db.WarningPatterns);
    db.CompletedRequests.RemoveRange(db.CompletedRequests);
    db.RequestTypes.RemoveRange(db.RequestTypes);
    db.WorkOrders.RemoveRange(db.WorkOrders);
    db.ChangeAudits.RemoveRange(db.ChangeAudits);
    db.ApprovalWorkflows.RemoveRange(db.ApprovalWorkflows);
    db.PueTrends.RemoveRange(db.PueTrends);
    db.CapacityReports.RemoveRange(db.CapacityReports);
    db.ExportCategories.RemoveRange(db.ExportCategories);
    db.ExportTemplates.RemoveRange(db.ExportTemplates);
    db.ScheduledReports.RemoveRange(db.ScheduledReports);
    db.ComplianceSections.RemoveRange(db.ComplianceSections);
    db.ReportMetrics.RemoveRange(db.ReportMetrics);
    db.HeatmapDatas.RemoveRange(db.HeatmapDatas);
    db.RbacRoles.RemoveRange(db.RbacRoles);
    db.SystemSettings.RemoveRange(db.SystemSettings);
    db.SiteConfigs.RemoveRange(db.SiteConfigs);
    db.Integrations.RemoveRange(db.Integrations);
    db.ApiKeys.RemoveRange(db.ApiKeys);
    db.WebhookConfigs.RemoveRange(db.WebhookConfigs);
    db.SettingsAudits.RemoveRange(db.SettingsAudits);
    db.LanguageConfigs.RemoveRange(db.LanguageConfigs);
    db.VisRacks.RemoveRange(db.VisRacks);
    db.NetworkDevices.RemoveRange(db.NetworkDevices);
    db.VisLinks.RemoveRange(db.VisLinks);
    db.Widgets.RemoveRange(db.Widgets);
    db.WeatherLocations.RemoveRange(db.WeatherLocations);
    db.SustainabilityMetrics.RemoveRange(db.SustainabilityMetrics);
    await db.SaveChangesAsync();

    // ── Original seed data ──
    db.PowerFeeds.AddRange(
        new PowerFeed { Name = "Utility Feed A", Capacity = 2100, Load = 1480, Status = "active" },
        new PowerFeed { Name = "Utility Feed B", Capacity = 2100, Load = 1367, Status = "active" },
        new PowerFeed { Name = "Generator G1", Capacity = 2000, Load = 0, Status = "standby" },
        new PowerFeed { Name = "Generator G2", Capacity = 2000, Load = 0, Status = "standby" }
    );
    db.CoolingUnits.AddRange(
        new CoolingUnit { Name = "CRAH-A1", Type = "CRAH", Capacity = 600, Load = 482, SupplyTemp = 13.8, ReturnTemp = 28.2, FanSpeed = 78, Status = "running" },
        new CoolingUnit { Name = "CRAH-A2", Type = "CRAH", Capacity = 600, Load = 510, SupplyTemp = 14.1, ReturnTemp = 29.0, FanSpeed = 82, Status = "running" },
        new CoolingUnit { Name = "CRAH-B1", Type = "CRAH", Capacity = 600, Load = 445, SupplyTemp = 14.5, ReturnTemp = 28.4, FanSpeed = 72, Status = "running" },
        new CoolingUnit { Name = "CRAH-B2", Type = "CRAH", Capacity = 600, Load = 498, SupplyTemp = 14.0, ReturnTemp = 28.8, FanSpeed = 80, Status = "running" },
        new CoolingUnit { Name = "CRAC-C1", Type = "CRAC", Capacity = 400, Load = 312, SupplyTemp = 14.6, ReturnTemp = 27.9, FanSpeed = 68, Status = "running" },
        new CoolingUnit { Name = "CRAC-C2", Type = "CRAC", Capacity = 400, Load = 185, SupplyTemp = 14.3, ReturnTemp = 27.5, FanSpeed = 55, Status = "running" },
        new CoolingUnit { Name = "CRAH-D1", Type = "CRAH", Capacity = 600, Load = 0, SupplyTemp = 0, ReturnTemp = 0, FanSpeed = 0, Status = "standby" }
    );
    db.Halls.AddRange(
        new Hall { Name = "Hall A", Sqft = 16000, Racks = 72, Occupied = 68, TotalU = 3024, UsedU = 2480, Power = 1240 },
        new Hall { Name = "Hall B", Sqft = 14000, Racks = 62, Occupied = 58, TotalU = 2604, UsedU = 2120, Power = 980 },
        new Hall { Name = "Hall C", Sqft = 8000, Racks = 32, Occupied = 24, TotalU = 1344, UsedU = 820, Power = 412 },
        new Hall { Name = "Hall D", Sqft = 4000, Racks = 20, Occupied = 14, TotalU = 1308, UsedU = 472, Power = 215 }
    );
    db.NetworkLinks.AddRange(
        new NetworkLink { Name = "ISP-A (Primary)", Capacity = 200, Usage = 142, Latency = 1.2, Loss = 0.01, Status = "active" },
        new NetworkLink { Name = "ISP-B (Secondary)", Capacity = 200, Usage = 105, Latency = 1.8, Loss = 0.02, Status = "active" },
        new NetworkLink { Name = "DC-East ↔ DC-West", Capacity = 100, Usage = 38, Latency = 18.4, Loss = 0.00, Status = "active" },
        new NetworkLink { Name = "DC-East ↔ DC-EU", Capacity = 50, Usage = 22, Latency = 82.1, Loss = 0.03, Status = "active" }
    );
    db.Assets.AddRange(
        new Asset { Name = "Servers", Class = "Compute", Installed = 124, Planned = 12, Archived = 8 },
        new Asset { Name = "Network Switches", Class = "Network", Installed = 32, Planned = 4, Archived = 2 },
        new Asset { Name = "Storage Arrays", Class = "Storage", Installed = 8, Planned = 2, Archived = 1 },
        new Asset { Name = "PDUs", Class = "Power", Installed = 24, Planned = 2, Archived = 0 },
        new Asset { Name = "UPS Units", Class = "Power", Installed = 5, Planned = 1, Archived = 0 },
        new Asset { Name = "Patch Panels", Class = "Network", Installed = 18, Planned = 0, Archived = 2 },
        new Asset { Name = "Cooling Units", Class = "Cooling", Installed = 7, Planned = 1, Archived = 0 }
    );
    db.Alarms.AddRange(
        new Alarm { Source = "Server Rack A-01", Severity = "Critical", Message = "Temperature threshold exceeded: 42°C", Time = DateTime.UtcNow.AddMinutes(-5), Acknowledged = false },
        new Alarm { Source = "PDU-B-03", Severity = "Critical", Message = "Power consumption anomaly detected", Time = DateTime.UtcNow.AddMinutes(-23), Acknowledged = false },
        new Alarm { Source = "Cooling Unit C-02", Severity = "High", Message = "Fan speed degraded — running at 45%", Time = DateTime.UtcNow.AddMinutes(-47), Acknowledged = false },
        new Alarm { Source = "Network Switch D-01", Severity = "High", Message = "Port channel flapping detected", Time = DateTime.UtcNow.AddHours(-2), Acknowledged = false },
        new Alarm { Source = "UPS Battery Bank", Severity = "Medium", Message = "Battery health check: 1 cell below threshold", Time = DateTime.UtcNow.AddHours(-6), Acknowledged = false }
    );
    db.ChangeRequests.AddRange(
        new ChangeRequest { RequestId = "CHG-001", Requester = "John Smith", Type = "Hardware", Priority = "High", Status = "Pending Review", RequestedDate = "2026-02-18" },
        new ChangeRequest { RequestId = "CHG-002", Requester = "Sarah Johnson", Type = "Network", Priority = "Medium", Status = "Waiting Approval", RequestedDate = "2026-02-17" },
        new ChangeRequest { RequestId = "CHG-003", Requester = "Mike Chen", Type = "Software", Priority = "Low", Status = "Pending Review", RequestedDate = "2026-02-16" },
        new ChangeRequest { RequestId = "CHG-004", Requester = "Emma Wilson", Type = "Access", Priority = "High", Status = "Waiting Approval", RequestedDate = "2026-02-15" },
        new ChangeRequest { RequestId = "CHG-005", Requester = "David Brown", Type = "Emergency", Priority = "Critical", Status = "In Progress", RequestedDate = "2026-02-14" }
    );
    db.Users.AddRange(
        new User { Name = "John Administrator", Email = "john.admin@datacenter.local", Role = "Administrator", LastLogin = "2026-02-20 14:32 UTC", Status = "Active" },
        new User { Name = "Sarah Operations", Email = "sarah.ops@datacenter.local", Role = "Operator", LastLogin = "2026-02-20 08:15 UTC", Status = "Active" },
        new User { Name = "Mike Engineer", Email = "mike.eng@datacenter.local", Role = "Engineer", LastLogin = "2026-02-19 23:45 UTC", Status = "Active" },
        new User { Name = "Lisa Viewer", Email = "lisa.view@datacenter.local", Role = "Viewer", LastLogin = "2026-02-15 10:22 UTC", Status = "Active" },
        new User { Name = "Robert Inactive", Email = "robert.inactive@datacenter.local", Role = "Operator", LastLogin = "2025-12-01 16:00 UTC", Status = "Inactive" }
    );
    db.Reports.AddRange(
        new Report { Name = "Power Consumption Report", Description = "Monthly power usage, costs, and efficiency metrics", Color = "#ffc107", LastRun = "2026-02-18" },
        new Report { Name = "Cooling Performance Report", Description = "Temperature monitoring and cooling efficiency analysis", Color = "#0dcaf0", LastRun = "2026-02-19" },
        new Report { Name = "Space Utilization Report", Description = "Rack and floor space capacity analysis", Color = "#198754", LastRun = "2026-02-17" },
        new Report { Name = "Equipment Inventory Report", Description = "Complete asset listing and depreciation tracking", Color = "#6c757d", LastRun = "2026-02-15" },
        new Report { Name = "Network Performance Report", Description = "Bandwidth utilization and network health metrics", Color = "#0d6efd", LastRun = "2026-02-16" },
        new Report { Name = "Compliance Report", Description = "ISO 22237 and regulatory compliance summary", Color = "#dc3545", LastRun = "2026-02-14" }
    );
    db.SecurityIncidents.AddRange(
        new SecurityIncident { Type = "Unauthorized Access Attempt", Timestamp = "2026-02-20 14:32", Location = "Data Center A", Severity = "high", Status = "resolved" },
        new SecurityIncident { Type = "Configuration Change", Timestamp = "2026-02-20 13:15", Location = "Security Control Room", Severity = "medium", Status = "investigating" },
        new SecurityIncident { Type = "Access Control Bypass", Timestamp = "2026-02-20 11:42", Location = "Server Room B", Severity = "critical", Status = "active" },
        new SecurityIncident { Type = "Policy Violation", Timestamp = "2026-02-19 16:28", Location = "Network Operations", Severity = "low", Status = "resolved" },
        new SecurityIncident { Type = "Unusual Activity Pattern", Timestamp = "2026-02-19 09:10", Location = "Data Center C", Severity = "medium", Status = "resolved" }
    );
    db.EnvironmentalMetrics.AddRange(
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 13.2, Unit = "tonnes", Month = 1, Year = 2026 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 12.8, Unit = "tonnes", Month = 2, Year = 2026 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 11.5, Unit = "tonnes", Month = 3, Year = 2025 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 10.9, Unit = "tonnes", Month = 4, Year = 2025 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 11.2, Unit = "tonnes", Month = 5, Year = 2025 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 10.8, Unit = "tonnes", Month = 6, Year = 2025 }
    );
    db.Sensors.AddRange(
        new Sensor { SensorId = "TEMP-HALL-A-01", Name = "Hall A Inlet Temp", Type = "Temperature", Location = "Hall A Inlet", Status = "Online", MinThreshold = "18°C", MaxThreshold = "27°C", PollingInterval = "30 seconds" },
        new Sensor { SensorId = "TEMP-HALL-B-01", Name = "Hall B Outlet Temp", Type = "Temperature", Location = "Hall B Outlet", Status = "Online", MinThreshold = "18°C", MaxThreshold = "35°C", PollingInterval = "30 seconds" },
        new Sensor { SensorId = "HUMID-HALL-A-01", Name = "Hall A Humidity", Type = "Humidity", Location = "Hall A", Status = "Online", MinThreshold = "20%", MaxThreshold = "80%", PollingInterval = "60 seconds" },
        new Sensor { SensorId = "HUMID-HALL-B-01", Name = "Hall B Humidity", Type = "Humidity", Location = "Hall B", Status = "Online", MinThreshold = "20%", MaxThreshold = "80%", PollingInterval = "60 seconds" },
        new Sensor { SensorId = "POWER-PDU-01", Name = "PDU Row 1 Power", Type = "Power", Location = "PDU Row 1", Status = "Online", MinThreshold = "0 kW", MaxThreshold = "120 kW", PollingInterval = "5 seconds" },
        new Sensor { SensorId = "AIRFLOW-A-01", Name = "CRAC Unit A Airflow", Type = "Air Flow", Location = "CRAC Unit A", Status = "Online", MinThreshold = "1000 CFM", MaxThreshold = "5000 CFM", PollingInterval = "30 seconds" },
        new Sensor { SensorId = "PRESSURE-ROOM-01", Name = "Cold Aisle 1 Pressure", Type = "Pressure", Location = "Cold Aisle 1", Status = "Online", MinThreshold = "-12 Pa", MaxThreshold = "12 Pa", PollingInterval = "120 seconds" },
        new Sensor { SensorId = "WATER-LEAK-01", Name = "Cooling Loop A Leak", Type = "Water Leak Detection", Location = "Cooling Loop A", Status = "Online", MinThreshold = "None", MaxThreshold = "Alert", PollingInterval = "30 seconds" }
    );

    // ── Dashboard seed ──
    db.HealthScoreCategories.AddRange(
        new HealthScoreCategory { Name = "Power", Score = 92, Color = "success" },
        new HealthScoreCategory { Name = "Cooling", Score = 88, Color = "success" },
        new HealthScoreCategory { Name = "Network", Score = 78, Color = "warning" },
        new HealthScoreCategory { Name = "Security", Score = 95, Color = "success" },
        new HealthScoreCategory { Name = "Space", Score = 72, Color = "warning" },
        new HealthScoreCategory { Name = "Compliance", Score = 90, Color = "success" }
    );
    db.Sites.AddRange(
        new Site { Name = "DC-East", Location = "New York, NY", Lat = 40.7128, Lng = -74.006, Status = "operational", Power = 2847, Pue = 1.38, Racks = 186, Alarms = 3 },
        new Site { Name = "DC-West", Location = "San Jose, CA", Lat = 37.3382, Lng = -121.8863, Status = "operational", Power = 1920, Pue = 1.42, Racks = 124, Alarms = 1 },
        new Site { Name = "DC-EU", Location = "Amsterdam, NL", Lat = 52.3676, Lng = 4.9041, Status = "operational", Power = 1450, Pue = 1.35, Racks = 96, Alarms = 0 },
        new Site { Name = "DC-Edge-1", Location = "Dallas, TX", Lat = 32.7767, Lng = -96.797, Status = "maintenance", Power = 320, Pue = 1.55, Racks = 20, Alarms = 2 }
    );
    db.CustomDashboards.AddRange(
        new CustomDashboard { Name = "NOC Overview", Widgets = 8, Modified = "2026-02-20", Favorite = true },
        new CustomDashboard { Name = "Power Focus", Widgets = 6, Modified = "2026-02-18", Favorite = false },
        new CustomDashboard { Name = "Cooling Analysis", Widgets = 5, Modified = "2026-02-15", Favorite = true },
        new CustomDashboard { Name = "Executive Summary", Widgets = 4, Modified = "2026-02-10", Favorite = false }
    );

    // ── Power seed ──
    db.PueReadings.AddRange(
        new PueReading { Date = "2026-02-01", Pue = 1.42, Component = "IT Load", Power = 2100, Pct = 70.4 },
        new PueReading { Date = "2026-02-05", Pue = 1.39, Component = "Cooling", Power = 580, Pct = 19.5 },
        new PueReading { Date = "2026-02-10", Pue = 1.37, Component = "Lighting & Other", Power = 120, Pct = 4.0 },
        new PueReading { Date = "2026-02-15", Pue = 1.38, Component = "Power Distribution", Power = 182, Pct = 6.1 }
    );
    db.PowerBudgets.AddRange(
        new PowerBudget { Name = "Hall A", Budget = 1500, Actual = 1240, Reserved = 120 },
        new PowerBudget { Name = "Hall B", Budget = 1200, Actual = 980, Reserved = 80 },
        new PowerBudget { Name = "Hall C", Budget = 600, Actual = 412, Reserved = 50 },
        new PowerBudget { Name = "Hall D", Budget = 400, Actual = 215, Reserved = 30 }
    );
    db.Cabinets.AddRange(
        new Cabinet { CabinetId = "CAB-A01", Location = "Hall A Row 1", Power = 8.2, Capacity = 12.0, Phase = "3-phase" },
        new Cabinet { CabinetId = "CAB-A02", Location = "Hall A Row 1", Power = 6.8, Capacity = 12.0, Phase = "3-phase" },
        new Cabinet { CabinetId = "CAB-B01", Location = "Hall B Row 1", Power = 10.1, Capacity = 12.0, Phase = "3-phase" },
        new Cabinet { CabinetId = "CAB-B02", Location = "Hall B Row 1", Power = 4.5, Capacity = 8.0, Phase = "single" },
        new Cabinet { CabinetId = "CAB-C01", Location = "Hall C Row 1", Power = 7.4, Capacity = 10.0, Phase = "3-phase" }
    );
    db.PowerChains.AddRange(
        new PowerChain { Name = "Chain-A", Source = "Utility Feed A", Capacity = 2100, Load = 1480, Redundancy = "N+1", Status = "active" },
        new PowerChain { Name = "Chain-B", Source = "Utility Feed B", Capacity = 2100, Load = 1367, Redundancy = "N+1", Status = "active" },
        new PowerChain { Name = "Chain-G", Source = "Generator G1", Capacity = 2000, Load = 0, Redundancy = "2N", Status = "standby" }
    );
    db.Pdus.AddRange(
        new Pdu { Name = "PDU-A1", Chain = "Chain-A", Input = 480, Output = 208, Load = 72, Capacity = 100, Breakers = 42 },
        new Pdu { Name = "PDU-A2", Chain = "Chain-A", Input = 480, Output = 208, Load = 68, Capacity = 100, Breakers = 42 },
        new Pdu { Name = "PDU-B1", Chain = "Chain-B", Input = 480, Output = 208, Load = 81, Capacity = 100, Breakers = 42 },
        new Pdu { Name = "PDU-B2", Chain = "Chain-B", Input = 480, Output = 208, Load = 65, Capacity = 100, Breakers = 42 }
    );
    db.StrandedPowers.AddRange(
        new StrandedPower { Location = "Hall A", Allocated = 1500, Actual = 1240, Stranded = 260, Pct = 17.3 },
        new StrandedPower { Location = "Hall B", Allocated = 1200, Actual = 980, Stranded = 220, Pct = 18.3 },
        new StrandedPower { Location = "Hall C", Allocated = 600, Actual = 412, Stranded = 188, Pct = 31.3 },
        new StrandedPower { Location = "Hall D", Allocated = 400, Actual = 215, Stranded = 185, Pct = 46.3 }
    );
    db.Rectifiers.AddRange(
        new Rectifier { RectifierId = "RECT-01", Voltage = 48.2, Current = 125, Power = 6025, Efficiency = 96.2, Status = "active" },
        new Rectifier { RectifierId = "RECT-02", Voltage = 48.1, Current = 118, Power = 5675, Efficiency = 95.8, Status = "active" },
        new Rectifier { RectifierId = "RECT-03", Voltage = 48.0, Current = 0, Power = 0, Efficiency = 0, Status = "standby" }
    );
    db.DcBuses.AddRange(
        new DcBus { Name = "DC Bus A", Voltage = 48.2, Current = 243, Power = 11713, Temp = 32.5 },
        new DcBus { Name = "DC Bus B", Voltage = 48.1, Current = 198, Power = 9524, Temp = 31.2 }
    );
    db.BatteryBanks.AddRange(
        new BatteryBank { Name = "Bank A", Type = "Li-ion", Cells = 120, Soc = 98, Soh = 96, Voltage = 432, Temp = 24.5, Runtime = 18.5, Status = "charging" },
        new BatteryBank { Name = "Bank B", Type = "Li-ion", Cells = 120, Soc = 95, Soh = 94, Voltage = 430, Temp = 25.1, Runtime = 17.2, Status = "charging" },
        new BatteryBank { Name = "Bank C", Type = "VRLA", Cells = 240, Soc = 100, Soh = 88, Voltage = 540, Temp = 23.8, Runtime = 22.0, Status = "float" }
    );
    db.PowerPhases.AddRange(
        new PowerPhase { Name = "L1", Voltage = 480.2, Current = 312, Frequency = 60.01, Thd = 2.1, Pf = 0.98 },
        new PowerPhase { Name = "L2", Voltage = 479.8, Current = 308, Frequency = 60.01, Thd = 2.3, Pf = 0.97 },
        new PowerPhase { Name = "L3", Voltage = 480.5, Current = 315, Frequency = 60.00, Thd = 1.9, Pf = 0.98 }
    );
    db.PowerEvents.AddRange(
        new PowerEvent { Time = "2026-02-20 14:32", Type = "Sag", Phase = "L1", Duration = "120ms", Magnitude = "−12%" },
        new PowerEvent { Time = "2026-02-19 03:15", Type = "Swell", Phase = "L2", Duration = "45ms", Magnitude = "+8%" },
        new PowerEvent { Time = "2026-02-18 22:01", Type = "Transient", Phase = "L3", Duration = "2ms", Magnitude = "+22%" }
    );
    db.ServerPowerRecords.AddRange(
        new ServerPowerRecord { Name = "srv-web-01", Rack = "A01", Power = 420, Cpu = 62, Psu = "2x750W", Status = "running" },
        new ServerPowerRecord { Name = "srv-db-01", Rack = "A02", Power = 680, Cpu = 78, Psu = "2x1200W", Status = "running" },
        new ServerPowerRecord { Name = "srv-gpu-01", Rack = "B01", Power = 2100, Cpu = 85, Psu = "4x2000W", Status = "running" },
        new ServerPowerRecord { Name = "srv-app-01", Rack = "B02", Power = 350, Cpu = 45, Psu = "2x750W", Status = "running" }
    );
    db.PortCapacityRecords.AddRange(
        new PortCapacityRecord { PduName = "PDU-A1", Total = 42, Used = 38, Reserved = 2 },
        new PortCapacityRecord { PduName = "PDU-A2", Total = 42, Used = 35, Reserved = 4 },
        new PortCapacityRecord { PduName = "PDU-B1", Total = 42, Used = 40, Reserved = 1 },
        new PortCapacityRecord { PduName = "PDU-B2", Total = 42, Used = 30, Reserved = 5 }
    );
    db.EnergyCostRecords.AddRange(
        new EnergyCostRecord { Month = "Jan", Kwh = 2180000, Cost = 189660, Rate = 0.087 },
        new EnergyCostRecord { Month = "Feb", Kwh = 2050000, Cost = 178350, Rate = 0.087 },
        new EnergyCostRecord { Month = "Mar", Kwh = 2120000, Cost = 184440, Rate = 0.087 },
        new EnergyCostRecord { Month = "Apr", Kwh = 2210000, Cost = 185640, Rate = 0.084 }
    );
    db.UpsUnits.AddRange(
        new UpsUnit { Name = "UPS-A1", Capacity = 500, Load = 382, LoadPct = 76.4, Input = 480, Output = 480, Battery = 98, Runtime = 18.5, Mode = "Online", Status = "normal" },
        new UpsUnit { Name = "UPS-A2", Capacity = 500, Load = 345, LoadPct = 69.0, Input = 480, Output = 480, Battery = 95, Runtime = 22.1, Mode = "Online", Status = "normal" },
        new UpsUnit { Name = "UPS-B1", Capacity = 300, Load = 218, LoadPct = 72.7, Input = 480, Output = 480, Battery = 100, Runtime = 15.8, Mode = "Online", Status = "normal" }
    );
    db.BreakerRecords.AddRange(
        new BreakerRecord { BreakerId = "BRK-A1-01", Pdu = "PDU-A1", Rating = 30, Load = 24.5, Circuits = 6, Trip = false },
        new BreakerRecord { BreakerId = "BRK-A1-02", Pdu = "PDU-A1", Rating = 30, Load = 28.2, Circuits = 6, Trip = false },
        new BreakerRecord { BreakerId = "BRK-B1-01", Pdu = "PDU-B1", Rating = 20, Load = 19.8, Circuits = 4, Trip = true },
        new BreakerRecord { BreakerId = "BRK-B1-02", Pdu = "PDU-B1", Rating = 20, Load = 12.1, Circuits = 4, Trip = false }
    );
    db.PueMetrics.AddRange(
        new PueMetric { Name = "PUE1 (Total)", Value = 1.38, Target = 1.40, StdDev = 0.04 },
        new PueMetric { Name = "PUE2 (IT Only)", Value = 1.25, Target = 1.20, StdDev = 0.03 },
        new PueMetric { Name = "PUE3 (Partial)", Value = 1.15, Target = 1.10, StdDev = 0.02 }
    );

    // ── Cooling seed ──
    db.CoolingMetrics.AddRange(
        new CoolingMetric { Name = "CER", Value = 0.72, Unit = "ratio", Target = 0.65, Description = "Cooling Efficiency Ratio" },
        new CoolingMetric { Name = "PUEc", Value = 1.12, Unit = "ratio", Target = 1.10, Description = "Partial PUE (cooling)" },
        new CoolingMetric { Name = "EER", Value = 12.5, Unit = "BTU/Wh", Target = 14.0, Description = "Energy Efficiency Ratio" },
        new CoolingMetric { Name = "COP", Value = 4.2, Unit = "ratio", Target = 4.5, Description = "Coefficient of Performance" }
    );
    db.CabinetTemperatures.AddRange(
        new CabinetTemperature { CabinetId = "CAB-A01", Location = "Hall A Row 1", Inlet = 22.4, Outlet = 34.8, Delta = 12.4, Alarm = false },
        new CabinetTemperature { CabinetId = "CAB-A02", Location = "Hall A Row 1", Inlet = 21.8, Outlet = 33.2, Delta = 11.4, Alarm = false },
        new CabinetTemperature { CabinetId = "CAB-B01", Location = "Hall B Row 1", Inlet = 24.1, Outlet = 38.5, Delta = 14.4, Alarm = true },
        new CabinetTemperature { CabinetId = "CAB-B02", Location = "Hall B Row 1", Inlet = 23.2, Outlet = 35.1, Delta = 11.9, Alarm = false }
    );
    db.CabinetHumidities.AddRange(
        new CabinetHumidity { CabinetId = "CAB-A01", Location = "Hall A Row 1", Rh = 45.2, Dewpoint = 12.1, Alarm = false },
        new CabinetHumidity { CabinetId = "CAB-A02", Location = "Hall A Row 1", Rh = 48.5, Dewpoint = 13.4, Alarm = false },
        new CabinetHumidity { CabinetId = "CAB-B01", Location = "Hall B Row 1", Rh = 62.1, Dewpoint = 18.2, Alarm = true },
        new CabinetHumidity { CabinetId = "CAB-B02", Location = "Hall B Row 1", Rh = 44.8, Dewpoint = 11.9, Alarm = false }
    );
    db.SensorTrends.AddRange(
        new SensorTrend { SensorId = "TEMP-A01", Cabinet = "CAB-A01", Type = "temperature", Time = "08:00", Value = 22.1 },
        new SensorTrend { SensorId = "TEMP-A01", Cabinet = "CAB-A01", Type = "temperature", Time = "12:00", Value = 23.4 },
        new SensorTrend { SensorId = "TEMP-A01", Cabinet = "CAB-A01", Type = "temperature", Time = "16:00", Value = 24.2 },
        new SensorTrend { SensorId = "TEMP-A01", Cabinet = "CAB-A01", Type = "temperature", Time = "20:00", Value = 22.8 }
    );
    db.CoolingCapacities.AddRange(
        new CoolingCapacity { Name = "Hall A", Current = 1920, Max = 2400, Planned = 200, Available = 280, Growth = 8.5 },
        new CoolingCapacity { Name = "Hall B", Current = 1500, Max = 2400, Planned = 150, Available = 750, Growth = 5.2 },
        new CoolingCapacity { Name = "Hall C", Current = 712, Max = 1200, Planned = 100, Available = 388, Growth = 12.1 }
    );
    db.SupplyReturnUnits.AddRange(
        new SupplyReturnUnit { Name = "CRAH-A1", SupplyTemp = 13.8, ReturnTemp = 28.2, SupplyFlow = 12500, ReturnFlow = 12480, DeltaT = 14.4, Status = "running" },
        new SupplyReturnUnit { Name = "CRAH-A2", SupplyTemp = 14.1, ReturnTemp = 29.0, SupplyFlow = 12200, ReturnFlow = 12190, DeltaT = 14.9, Status = "running" },
        new SupplyReturnUnit { Name = "CRAH-B1", SupplyTemp = 14.5, ReturnTemp = 28.4, SupplyFlow = 11800, ReturnFlow = 11780, DeltaT = 13.9, Status = "running" }
    );
    db.WaterUsageRecords.AddRange(
        new WaterUsageRecord { Month = "Jan", WaterL = 245000, ItKwh = 172000, Wue = 1.42 },
        new WaterUsageRecord { Month = "Feb", WaterL = 232000, ItKwh = 168000, Wue = 1.38 },
        new WaterUsageRecord { Month = "Mar", WaterL = 228000, ItKwh = 170000, Wue = 1.34 }
    );
    db.CoolingSystems.AddRange(
        new CoolingSystem { Name = "Chiller Plant A", Status = "running", Uptime = "99.98%", LastMaint = "2026-01-15", NextMaint = "2026-04-15", Downtime = "0.02%" },
        new CoolingSystem { Name = "Chiller Plant B", Status = "running", Uptime = "99.95%", LastMaint = "2026-02-01", NextMaint = "2026-05-01", Downtime = "0.05%" },
        new CoolingSystem { Name = "Free Cooling Tower", Status = "standby", Uptime = "N/A", LastMaint = "2025-11-10", NextMaint = "2026-03-10", Downtime = "N/A" }
    );
    db.Chillers.AddRange(
        new Chiller { Name = "CH-01", Type = "Centrifugal", Capacity = 500, Load = 385, Cop = 5.8, Eer = 19.8, SupplyW = 6.7, ReturnW = 12.2, Flow = 950, Status = "running" },
        new Chiller { Name = "CH-02", Type = "Centrifugal", Capacity = 500, Load = 340, Cop = 5.5, Eer = 18.8, SupplyW = 7.0, ReturnW = 12.5, Flow = 920, Status = "running" },
        new Chiller { Name = "CH-03", Type = "Screw", Capacity = 300, Load = 0, Cop = 0, Eer = 0, SupplyW = 0, ReturnW = 0, Flow = 0, Status = "standby" }
    );
    db.CabinetAirflows.AddRange(
        new CabinetAirflow { CabinetId = "CAB-A01", Cfm = 850, Kw = 8.2, CfmPerKw = 103.7, Pressure = 0.02, Leakage = 2.1 },
        new CabinetAirflow { CabinetId = "CAB-A02", Cfm = 720, Kw = 6.8, CfmPerKw = 105.9, Pressure = 0.018, Leakage = 1.8 },
        new CabinetAirflow { CabinetId = "CAB-B01", Cfm = 1100, Kw = 10.1, CfmPerKw = 108.9, Pressure = 0.025, Leakage = 3.2 }
    );

    // ── Space seed ──
    db.Racks.AddRange(
        new Rack { RackId = "A-01", Hall = "Hall A", TotalU = 42, UsedU = 38, Power = 8.2, Weight = 420 },
        new Rack { RackId = "A-02", Hall = "Hall A", TotalU = 42, UsedU = 35, Power = 6.8, Weight = 380 },
        new Rack { RackId = "B-01", Hall = "Hall B", TotalU = 42, UsedU = 40, Power = 10.1, Weight = 510 },
        new Rack { RackId = "B-02", Hall = "Hall B", TotalU = 42, UsedU = 28, Power = 4.5, Weight = 290 },
        new Rack { RackId = "C-01", Hall = "Hall C", TotalU = 42, UsedU = 22, Power = 7.4, Weight = 340 }
    );
    db.RackGaps.AddRange(
        new RackGap { RackId = "A-01", TotalU = 42, FreeU = 4, GapStart = 39, GapSize = 4 },
        new RackGap { RackId = "A-02", TotalU = 42, FreeU = 7, GapStart = 18, GapSize = 3 },
        new RackGap { RackId = "A-02", TotalU = 42, FreeU = 7, GapStart = 36, GapSize = 4 },
        new RackGap { RackId = "B-02", TotalU = 42, FreeU = 14, GapStart = 29, GapSize = 14 }
    );
    db.CabinetGroups.AddRange(
        new CabinetGroup { Name = "Web Servers", Racks = "A-01,A-02,A-03", TotalU = 126, UsedU = 108, Remaining = 18 },
        new CabinetGroup { Name = "Database Cluster", Racks = "B-01,B-02", TotalU = 84, UsedU = 68, Remaining = 16 },
        new CabinetGroup { Name = "Network Core", Racks = "C-01", TotalU = 42, UsedU = 22, Remaining = 20 }
    );
    db.FloorPlanRacks.AddRange(
        new FloorPlanRack { RackId = "A-01", RowLabel = "Row A", Position = 1, Utilization = 90.5 },
        new FloorPlanRack { RackId = "A-02", RowLabel = "Row A", Position = 2, Utilization = 83.3 },
        new FloorPlanRack { RackId = "B-01", RowLabel = "Row B", Position = 1, Utilization = 95.2 },
        new FloorPlanRack { RackId = "B-02", RowLabel = "Row B", Position = 2, Utilization = 66.7 }
    );
    db.RackDevices.AddRange(
        new RackDevice { RackId = "A-01", Name = "Core Switch", StartU = 1, SizeU = 2, Type = "network", Color = "#0d6efd" },
        new RackDevice { RackId = "A-01", Name = "Server Cluster 1", StartU = 3, SizeU = 4, Type = "server", Color = "#198754" },
        new RackDevice { RackId = "A-01", Name = "Storage Array", StartU = 7, SizeU = 3, Type = "storage", Color = "#ffc107" },
        new RackDevice { RackId = "B-01", Name = "GPU Server", StartU = 1, SizeU = 4, Type = "server", Color = "#198754" },
        new RackDevice { RackId = "B-01", Name = "DB Server", StartU = 5, SizeU = 2, Type = "server", Color = "#198754" }
    );
    db.Scenarios.AddRange(
        new Scenario { Name = "Add 20 GPU Servers", Racks = 10, Power = 420, Cooling = 180, Space = 80, Feasibility = "Feasible", Notes = "Requires Hall D expansion" },
        new Scenario { Name = "Migrate DB to NVMe", Racks = 2, Power = 25, Cooling = 10, Space = 4, Feasibility = "Feasible", Notes = "In-place upgrade" },
        new Scenario { Name = "New Edge Pod", Racks = 4, Power = 80, Cooling = 35, Space = 16, Feasibility = "Partial", Notes = "Cooling capacity may be insufficient" }
    );
    db.SiteNodes.AddRange(
        new SiteNode { Name = "DC-East Campus", Type = "campus", ParentId = null },
        new SiteNode { Name = "Building 1", Type = "building", ParentId = 1 },
        new SiteNode { Name = "Hall A", Type = "hall", ParentId = 2 },
        new SiteNode { Name = "Hall B", Type = "hall", ParentId = 2 },
        new SiteNode { Name = "Building 2", Type = "building", ParentId = 1 },
        new SiteNode { Name = "Hall C", Type = "hall", ParentId = 5 }
    );
    db.StorageSystems.AddRange(
        new StorageSystem { Name = "NetApp FAS8200", Type = "NAS", TotalTB = 120, UsedTB = 84, Iops = 45000, Throughput = "2.4 GB/s", Status = "healthy" },
        new StorageSystem { Name = "Pure FlashArray", Type = "SAN", TotalTB = 64, UsedTB = 42, Iops = 180000, Throughput = "8.5 GB/s", Status = "healthy" },
        new StorageSystem { Name = "Dell PowerScale", Type = "Object", TotalTB = 500, UsedTB = 320, Iops = 12000, Throughput = "5.2 GB/s", Status = "warning" }
    );
    db.PendingAssets.AddRange(
        new PendingAsset { Name = "Dell R760", Size = 2, Type = "server", TargetRack = "CAB-B03" },
        new PendingAsset { Name = "Cisco C9300", Size = 1, Type = "network", TargetRack = "CAB-A01" },
        new PendingAsset { Name = "APC PDU", Size = 0, Type = "power", TargetRack = "CAB-C01" }
    );

    // ── Network seed ──
    db.TrafficRecords.AddRange(
        new TrafficRecord { Time = "00:00", Inbound = 120, Outbound = 95, Total = 215 },
        new TrafficRecord { Time = "04:00", Inbound = 85, Outbound = 72, Total = 157 },
        new TrafficRecord { Time = "08:00", Inbound = 210, Outbound = 180, Total = 390 },
        new TrafficRecord { Time = "12:00", Inbound = 245, Outbound = 198, Total = 443 },
        new TrafficRecord { Time = "16:00", Inbound = 232, Outbound = 195, Total = 427 },
        new TrafficRecord { Time = "20:00", Inbound = 178, Outbound = 142, Total = 320 }
    );
    db.DataSwitches.AddRange(
        new DataSwitch { Name = "Core-SW-01", Total = 48, Used = 42, Speed = "100GbE", Status = "active" },
        new DataSwitch { Name = "Core-SW-02", Total = 48, Used = 38, Speed = "100GbE", Status = "active" },
        new DataSwitch { Name = "Agg-SW-A1", Total = 48, Used = 44, Speed = "25GbE", Status = "active" },
        new DataSwitch { Name = "Agg-SW-B1", Total = 48, Used = 36, Speed = "25GbE", Status = "active" },
        new DataSwitch { Name = "ToR-A01", Total = 48, Used = 46, Speed = "10GbE", Status = "active" },
        new DataSwitch { Name = "ToR-A02", Total = 48, Used = 40, Speed = "10GbE", Status = "active" }
    );
    db.PowerPortRecords.AddRange(
        new PowerPortRecord { Name = "PDU-A1", Total = 42, Used = 38, Reserved = 2, Type = "C13" },
        new PowerPortRecord { Name = "PDU-A2", Total = 42, Used = 35, Reserved = 4, Type = "C13" },
        new PowerPortRecord { Name = "PDU-B1", Total = 24, Used = 20, Reserved = 2, Type = "C19" },
        new PowerPortRecord { Name = "PDU-B2", Total = 24, Used = 18, Reserved = 3, Type = "C19" },
        new PowerPortRecord { Name = "PDU-C1", Total = 42, Used = 22, Reserved = 5, Type = "C13" }
    );
    db.PortRecords.AddRange(
        new PortRecord { PortId = "Gi1/0/1", Speed = "1GbE", Vlan = 100, Color = "#198754", Connected = "srv-web-01", Status = "up" },
        new PortRecord { PortId = "Gi1/0/2", Speed = "10GbE", Vlan = 200, Color = "#0d6efd", Connected = "srv-db-01", Status = "up" },
        new PortRecord { PortId = "Gi1/0/3", Speed = "1GbE", Vlan = 100, Color = "#6c757d", Connected = "---", Status = "free" },
        new PortRecord { PortId = "Gi1/0/4", Speed = "10GbE", Vlan = 300, Color = "#dc3545", Connected = "srv-gpu-01", Status = "down" }
    );
    db.Cables.AddRange(
        new Cable { CableId = "CBL-0001", FromEnd = "Core-SW-01:1", ToEnd = "Agg-SW-A1:49", Type = "SM Fiber", Length = "15m", Color = "yellow", Path = "Tray A → Ladder 1" },
        new Cable { CableId = "CBL-0002", FromEnd = "Agg-SW-A1:1", ToEnd = "ToR-A01:49", Type = "DAC", Length = "3m", Color = "black", Path = "Cabinet A01" },
        new Cable { CableId = "CBL-0003", FromEnd = "ToR-A01:1", ToEnd = "srv-web-01:eth0", Type = "Cat6a", Length = "2m", Color = "blue", Path = "Cabinet A01" }
    );
    db.ContainerRecords.AddRange(
        new ContainerRecord { Name = "k8s-node-01", Ip = "10.0.1.10", Pods = 42, Cpu = 72.5, Mem = 68.2, Network = "10GbE", Status = "Ready" },
        new ContainerRecord { Name = "k8s-node-02", Ip = "10.0.1.11", Pods = 38, Cpu = 65.1, Mem = 71.8, Network = "10GbE", Status = "Ready" },
        new ContainerRecord { Name = "k8s-node-03", Ip = "10.0.1.12", Pods = 45, Cpu = 81.3, Mem = 74.5, Network = "10GbE", Status = "Ready" },
        new ContainerRecord { Name = "docker-host-01", Ip = "10.0.2.10", Pods = 18, Cpu = 45.2, Mem = 52.1, Network = "25GbE", Status = "Ready" }
    );
    db.NetworkLayers.AddRange(
        new NetworkLayer { Name = "Core Layer", Devices = "Core-SW-01,Core-SW-02", Connections = "4x100GbE uplinks" },
        new NetworkLayer { Name = "Aggregation", Devices = "Agg-SW-A1,Agg-SW-B1", Connections = "2x40GbE to Core" },
        new NetworkLayer { Name = "Access - Hall A", Devices = "ToR-A01,ToR-A02,ToR-A03", Connections = "2x25GbE to Agg" },
        new NetworkLayer { Name = "Access - Hall B", Devices = "ToR-B01,ToR-B02", Connections = "2x25GbE to Agg" }
    );

    // ── Assets seed ──
    db.ServerMetrics.AddRange(
        new ServerMetric { Name = "srv-web-01", Cpu = 62, Gpu = 0, Mem = 71, Disk = 45, NetIn = 120, NetOut = 85, Power = 420 },
        new ServerMetric { Name = "srv-db-01", Cpu = 78, Gpu = 0, Mem = 82, Disk = 68, NetIn = 95, NetOut = 210, Power = 680 },
        new ServerMetric { Name = "srv-gpu-01", Cpu = 45, Gpu = 92, Mem = 64, Disk = 32, NetIn = 250, NetOut = 180, Power = 2100 },
        new ServerMetric { Name = "srv-app-01", Cpu = 55, Gpu = 0, Mem = 48, Disk = 28, NetIn = 80, NetOut = 65, Power = 350 }
    );
    db.ServerRecords.AddRange(
        new ServerRecord { Name = "srv-web-01", Rack = "A-01", Os = "Ubuntu 22.04", Uptime = "45d 12h", Status = "online" },
        new ServerRecord { Name = "srv-db-01", Rack = "A-02", Os = "RHEL 9.2", Uptime = "120d 8h", Status = "online" },
        new ServerRecord { Name = "srv-gpu-01", Rack = "B-01", Os = "Ubuntu 22.04", Uptime = "30d 4h", Status = "online" },
        new ServerRecord { Name = "srv-app-01", Rack = "B-02", Os = "Debian 12", Uptime = "90d 16h", Status = "online" },
        new ServerRecord { Name = "srv-backup-01", Rack = "C-01", Os = "FreeBSD 14", Uptime = "0d 0h", Status = "offline" }
    );
    db.HardwareModels.AddRange(
        new HardwareModel { Model = "PowerEdge R760", Vendor = "Dell", Category = "Server", Count = 48, AvgAge = "1.2y" },
        new HardwareModel { Model = "ProLiant DL380 G11", Vendor = "HPE", Category = "Server", Count = 32, AvgAge = "2.1y" },
        new HardwareModel { Model = "DGX A100", Vendor = "NVIDIA", Category = "GPU Server", Count = 8, AvgAge = "0.8y" },
        new HardwareModel { Model = "Nexus 9300", Vendor = "Cisco", Category = "Switch", Count = 12, AvgAge = "3.2y" },
        new HardwareModel { Model = "7280R3", Vendor = "Arista", Category = "Switch", Count = 8, AvgAge = "1.5y" }
    );
    db.AssetStatuses.AddRange(
        new AssetStatus { Status = "Installed", Count = 220, Pct = 78.6, Color = "success" },
        new AssetStatus { Status = "Planned", Count = 22, Pct = 7.9, Color = "primary" },
        new AssetStatus { Status = "Archived", Count = 13, Pct = 4.6, Color = "secondary" },
        new AssetStatus { Status = "In Transit", Count = 25, Pct = 8.9, Color = "warning" }
    );
    db.Parts.AddRange(
        new Part { Name = "DDR5 64GB DIMM", Category = "Memory", InStock = 24, Reserved = 8, Reorder = 10, Unit = "pcs" },
        new Part { Name = "15.36TB NVMe SSD", Category = "Storage", InStock = 12, Reserved = 4, Reorder = 6, Unit = "pcs" },
        new Part { Name = "25GbE SFP28", Category = "Network", InStock = 48, Reserved = 12, Reorder = 20, Unit = "pcs" },
        new Part { Name = "PDU C13 Cable 2m", Category = "Power", InStock = 100, Reserved = 0, Reorder = 50, Unit = "pcs" }
    );
    db.VmHosts.AddRange(
        new VmHost { Name = "esxi-host-01", Type = "VMware ESXi 8.0", Physical = true, Vms = 24, Vcpus = 128, Vmem = 512 },
        new VmHost { Name = "kvm-host-01", Type = "KVM (RHEL 9)", Physical = true, Vms = 18, Vcpus = 96, Vmem = 384 },
        new VmHost { Name = "hyperv-host-01", Type = "Hyper-V 2022", Physical = true, Vms = 12, Vcpus = 64, Vmem = 256 }
    );
    db.OsRecords.AddRange(
        new OsRecord { Os = "Ubuntu 22.04 LTS", Count = 42, Pct = 33.9 },
        new OsRecord { Os = "RHEL 9.x", Count = 28, Pct = 22.6 },
        new OsRecord { Os = "Debian 12", Count = 18, Pct = 14.5 },
        new OsRecord { Os = "Windows Server 2022", Count = 15, Pct = 12.1 },
        new OsRecord { Os = "VMware ESXi 8.0", Count = 12, Pct = 9.7 },
        new OsRecord { Os = "FreeBSD 14", Count = 9, Pct = 7.3 }
    );
    db.ItemAges.AddRange(
        new ItemAge { Name = "UPS-A1", Customer = "Internal", Purchased = "2021-03-15", Age = "4.9y", Warranty = "Expired", Status = "active" },
        new ItemAge { Name = "R630 Cluster", Customer = "Internal", Purchased = "2019-08-22", Age = "6.5y", Warranty = "Expired", Status = "eol" },
        new ItemAge { Name = "Nexus 9300", Customer = "Internal", Purchased = "2023-01-10", Age = "3.1y", Warranty = "Active", Status = "active" },
        new ItemAge { Name = "DGX A100", Customer = "Internal", Purchased = "2025-06-01", Age = "0.7y", Warranty = "Active", Status = "active" }
    );
    db.AssetAttributes.AddRange(
        new AssetAttribute { Attribute = "Form Factor", Value = "1U", Count = 68 },
        new AssetAttribute { Attribute = "Form Factor", Value = "2U", Count = 42 },
        new AssetAttribute { Attribute = "Form Factor", Value = "4U", Count = 14 },
        new AssetAttribute { Attribute = "CPU Architecture", Value = "x86_64", Count = 112 },
        new AssetAttribute { Attribute = "CPU Architecture", Value = "ARM64", Count = 12 },
        new AssetAttribute { Attribute = "Network Speed", Value = "10GbE", Count = 82 },
        new AssetAttribute { Attribute = "Network Speed", Value = "25GbE", Count = 38 },
        new AssetAttribute { Attribute = "Power Redundancy", Value = "2N", Count = 95 },
        new AssetAttribute { Attribute = "Power Redundancy", Value = "N+1", Count = 29 }
    );
    db.LifecyclePhases.AddRange(
        new LifecyclePhase { Phase = "Procurement", Count = 8, Items = "Dell R760 x4, Arista 7280 x4", Eta = "2026-03-15" },
        new LifecyclePhase { Phase = "Staging", Count = 5, Items = "HPE DL380 x3, NVMe SSD x12", Eta = "2026-02-28" },
        new LifecyclePhase { Phase = "Deployment", Count = 3, Items = "Cisco Nexus x2, PDU x1", Eta = "2026-02-25" },
        new LifecyclePhase { Phase = "Production", Count = 220, Items = "All active assets", Eta = "Ongoing" },
        new LifecyclePhase { Phase = "End of Life", Count = 12, Items = "R630 Cluster, Old UPS", Eta = "2026-06-01" },
        new LifecyclePhase { Phase = "Decommission", Count = 4, Items = "Legacy switches", Eta = "2026-04-01" }
    );
    db.ImportExportRecords.AddRange(
        new ImportExportRecord { Date = "2026-02-20", Type = "Import", File = "asset_update_feb.csv", Records = 45, Status = "completed" },
        new ImportExportRecord { Date = "2026-02-18", Type = "Export", File = "full_inventory.csv", Records = 280, Status = "completed" },
        new ImportExportRecord { Date = "2026-02-15", Type = "Import", File = "new_servers.csv", Records = 12, Status = "completed" }
    );

    // ── Security seed ──
    db.AccessLogs.AddRange(
        new AccessLog { Timestamp = "2026-02-20 14:32", Door = "Main Entrance", Person = "John Smith", Method = "RFID", Result = "allowed" },
        new AccessLog { Timestamp = "2026-02-20 14:28", Door = "Server Room A", Person = "Sarah Johnson", Method = "Fingerprint", Result = "allowed" },
        new AccessLog { Timestamp = "2026-02-20 14:15", Door = "Server Room B", Person = "Unknown", Method = "Card", Result = "denied" },
        new AccessLog { Timestamp = "2026-02-20 13:45", Door = "NOC", Person = "Mike Chen", Method = "RFID", Result = "allowed" }
    );
    db.FirewallEvents.AddRange(
        new FirewallEvent { Timestamp = "2026-02-20 14:30", Type = "Blocked", SourceIP = "185.220.101.42", DestPort = 443, Action = "DROP", Severity = "high" },
        new FirewallEvent { Timestamp = "2026-02-20 14:22", Type = "Alert", SourceIP = "10.0.1.55", DestPort = 22, Action = "ALLOW", Severity = "medium" },
        new FirewallEvent { Timestamp = "2026-02-20 14:10", Type = "Blocked", SourceIP = "91.240.118.10", DestPort = 3389, Action = "DROP", Severity = "critical" }
    );
    db.IdsAlerts.AddRange(
        new IdsAlert { Timestamp = "2026-02-20 14:25", Alert = "Port scan detected", Protocol = "TCP", Confidence = 92.5, Status = "active" },
        new IdsAlert { Timestamp = "2026-02-20 13:48", Alert = "Brute force SSH", Protocol = "TCP", Confidence = 88.0, Status = "investigating" },
        new IdsAlert { Timestamp = "2026-02-20 12:30", Alert = "DNS tunneling", Protocol = "UDP", Confidence = 75.2, Status = "resolved" }
    );
    db.BlockedIps.AddRange(
        new BlockedIp { Ip = "185.220.101.42", Reason = "Tor exit node", Incidents = 15, FirstSeen = "2026-02-18", Status = "blocked" },
        new BlockedIp { Ip = "91.240.118.10", Reason = "RDP brute force", Incidents = 42, FirstSeen = "2026-02-15", Status = "blocked" },
        new BlockedIp { Ip = "45.155.205.233", Reason = "Known botnet", Incidents = 8, FirstSeen = "2026-02-19", Status = "blocked" }
    );
    db.AccessRoles.AddRange(
        new AccessRole { Name = "Administrator", PermissionCount = 42, UserCount = 3, Description = "Full system access", Status = "active" },
        new AccessRole { Name = "Operator", PermissionCount = 28, UserCount = 8, Description = "Day-to-day operations", Status = "active" },
        new AccessRole { Name = "Engineer", PermissionCount = 22, UserCount = 5, Description = "Technical maintenance", Status = "active" },
        new AccessRole { Name = "Viewer", PermissionCount = 8, UserCount = 12, Description = "Read-only access", Status = "active" },
        new AccessRole { Name = "Auditor", PermissionCount = 15, UserCount = 2, Description = "Compliance auditing", Status = "active" }
    );
    db.AccessRoleUsers.AddRange(
        new AccessRoleUser { Name = "John Smith", Role = "Administrator", Department = "IT Ops", LastLogin = "2026-02-20 14:32", Status = "active" },
        new AccessRoleUser { Name = "Sarah Johnson", Role = "Operator", Department = "NOC", LastLogin = "2026-02-20 08:15", Status = "active" },
        new AccessRoleUser { Name = "Mike Chen", Role = "Engineer", Department = "Engineering", LastLogin = "2026-02-19 23:45", Status = "active" },
        new AccessRoleUser { Name = "Lisa Wang", Role = "Viewer", Department = "Management", LastLogin = "2026-02-15 10:22", Status = "active" }
    );
    db.CctvCameras.AddRange(
        new CctvCamera { Name = "CAM-ENTRANCE-01", Location = "Main Entrance", Status = "online", Recording = "continuous", Resolution = "4K", Storage = "2.1 TB", LastFrame = "2026-02-20 14:35" },
        new CctvCamera { Name = "CAM-HALL-A-01", Location = "Hall A", Status = "online", Recording = "continuous", Resolution = "1080p", Storage = "1.4 TB", LastFrame = "2026-02-20 14:35" },
        new CctvCamera { Name = "CAM-HALL-B-01", Location = "Hall B", Status = "online", Recording = "motion", Resolution = "1080p", Storage = "0.8 TB", LastFrame = "2026-02-20 14:30" },
        new CctvCamera { Name = "CAM-NOC-01", Location = "NOC", Status = "offline", Recording = "stopped", Resolution = "1080p", Storage = "0.2 TB", LastFrame = "2026-02-20 12:00" }
    );
    db.RecordingStats.AddRange(
        new RecordingStat { Location = "Main Entrance", Cameras = 2, TotalStorage = "4 TB", UsedStorage = "3.2 TB", Retention = "30 days" },
        new RecordingStat { Location = "Server Halls", Cameras = 4, TotalStorage = "8 TB", UsedStorage = "5.8 TB", Retention = "14 days" },
        new RecordingStat { Location = "NOC/Offices", Cameras = 2, TotalStorage = "2 TB", UsedStorage = "0.8 TB", Retention = "7 days" }
    );
    db.SecurityAudits.AddRange(
        new SecurityAudit { Timestamp = "2026-02-20 14:32", User = "john.admin", Action = "login", Resource = "Dashboard", Result = "success", Details = "MFA verified" },
        new SecurityAudit { Timestamp = "2026-02-20 14:15", User = "unknown", Action = "login", Resource = "Admin Panel", Result = "failed", Details = "Invalid credentials" },
        new SecurityAudit { Timestamp = "2026-02-20 13:45", User = "sarah.ops", Action = "update", Resource = "Alarm Config", Result = "success", Details = "Threshold changed" }
    );

    // ── Alarms seed ──
    db.AlarmHistories.AddRange(
        new AlarmHistory { Triggered = "2026-02-19 08:30", Resolved = "2026-02-19 09:15", Source = "CRAH-A2", Severity = "High", Message = "Supply temperature above threshold" },
        new AlarmHistory { Triggered = "2026-02-18 22:10", Resolved = "2026-02-18 23:45", Source = "PDU-B2", Severity = "Critical", Message = "Overload detected on circuit 3" },
        new AlarmHistory { Triggered = "2026-02-18 14:00", Resolved = "2026-02-18 14:30", Source = "UPS-A1", Severity = "Medium", Message = "Battery test completed with warnings" }
    );
    db.NotificationRecords.AddRange(
        new NotificationRecord { Type = "Alert", Message = "Temperature threshold exceeded in Hall A", Time = "2026-02-20 14:32", Read = false },
        new NotificationRecord { Type = "Warning", Message = "UPS battery health degraded", Time = "2026-02-20 12:15", Read = false },
        new NotificationRecord { Type = "Info", Message = "Scheduled maintenance window starts in 2 hours", Time = "2026-02-20 10:00", Read = true },
        new NotificationRecord { Type = "Maintenance", Message = "Chiller CH-03 maintenance completed", Time = "2026-02-19 16:00", Read = true }
    );
    db.EmailRecipients.AddRange(
        new EmailRecipient { Email = "noc@datacenter.local", Frequency = "Immediate", AlertTypes = "Critical,High", Active = true },
        new EmailRecipient { Email = "ops-team@datacenter.local", Frequency = "Hourly Digest", AlertTypes = "Critical,High,Medium", Active = true },
        new EmailRecipient { Email = "management@datacenter.local", Frequency = "Daily Summary", AlertTypes = "Critical", Active = true }
    );
    db.SmsRecipients.AddRange(
        new SmsRecipient { Name = "John Smith", Phone = "+1-555-0101", Severity = "Critical", Enabled = true },
        new SmsRecipient { Name = "Sarah Johnson", Phone = "+1-555-0102", Severity = "Critical", Enabled = true },
        new SmsRecipient { Name = "On-Call Engineer", Phone = "+1-555-0199", Severity = "High", Enabled = true }
    );
    db.PushChannels.AddRange(
        new PushChannel { Name = "Slack #alerts", Type = "Slack", Endpoint = "https://hooks.slack.com/xxx", AlertTypes = "Critical,High", Active = true },
        new PushChannel { Name = "PagerDuty", Type = "PagerDuty", Endpoint = "https://events.pagerduty.com/xxx", AlertTypes = "Critical", Active = true },
        new PushChannel { Name = "Teams NOC", Type = "MS Teams", Endpoint = "https://outlook.office.com/webhook/xxx", AlertTypes = "Critical,High,Medium", Active = true }
    );
    db.AlertRules.AddRange(
        new AlertRule { Name = "High Temp Alert", Condition = "temperature > threshold", Threshold = "35°C", Action = "Send notification", Enabled = true },
        new AlertRule { Name = "Power Overload", Condition = "load > 90% capacity", Threshold = "90%", Action = "Send alert + auto-failover", Enabled = true },
        new AlertRule { Name = "Humidity Warning", Condition = "humidity > 60%", Threshold = "60% RH", Action = "Send warning", Enabled = true },
        new AlertRule { Name = "UPS Battery Low", Condition = "battery < 20%", Threshold = "20%", Action = "Critical alert", Enabled = true },
        new AlertRule { Name = "Network Packet Loss", Condition = "loss > 1%", Threshold = "1%", Action = "Send alert", Enabled = false }
    );
    db.EscalationPolicies.AddRange(
        new EscalationPolicy { Name = "Critical Infrastructure", Description = "For power and cooling failures", Active = true, Levels = "[{\"contact\":\"NOC Team\",\"waitTime\":\"5 min\",\"method\":\"Slack\"},{\"contact\":\"On-Call Engineer\",\"waitTime\":\"15 min\",\"method\":\"Phone\"},{\"contact\":\"VP Operations\",\"waitTime\":\"30 min\",\"method\":\"Phone\"}]" },
        new EscalationPolicy { Name = "Security Breach", Description = "For unauthorized access", Active = true, Levels = "[{\"contact\":\"Security Team\",\"waitTime\":\"Immediate\",\"method\":\"SMS\"},{\"contact\":\"CISO\",\"waitTime\":\"10 min\",\"method\":\"Phone\"}]" }
    );
    db.MaintenanceWindows.AddRange(
        new MaintenanceWindow { Component = "Chiller CH-03", StartTime = "2026-02-25 02:00", EndTime = "2026-02-25 06:00", Impact = "Medium", Status = "Scheduled" },
        new MaintenanceWindow { Component = "UPS-B1 Battery", StartTime = "2026-02-26 22:00", EndTime = "2026-02-27 02:00", Impact = "High", Status = "Scheduled" },
        new MaintenanceWindow { Component = "Core-SW-02 Firmware", StartTime = "2026-03-01 03:00", EndTime = "2026-03-01 04:00", Impact = "Critical", Status = "Scheduled" }
    );
    db.WarningPatterns.AddRange(
        new WarningPattern { Name = "CRAH-A2 Fan Degradation", Frequency = "3x/week", LastOccurrence = "2026-02-20 08:30", Trend = "Increasing" },
        new WarningPattern { Name = "PDU-B2 Load Spikes", Frequency = "Daily", LastOccurrence = "2026-02-20 14:00", Trend = "Stable" },
        new WarningPattern { Name = "Hall C Humidity Drift", Frequency = "2x/month", LastOccurrence = "2026-02-15 10:00", Trend = "Decreasing" }
    );

    // ── Changes seed ──
    db.CompletedRequests.AddRange(
        new CompletedRequest { RequestId = "CHG-098", Requester = "John Smith", Type = "Hardware", Priority = "High", RequestedDate = "2026-02-10", CompletionDate = "2026-02-14", Duration = 4 },
        new CompletedRequest { RequestId = "CHG-097", Requester = "Emma Wilson", Type = "Network", Priority = "Medium", RequestedDate = "2026-02-05", CompletionDate = "2026-02-12", Duration = 7 },
        new CompletedRequest { RequestId = "CHG-096", Requester = "Mike Chen", Type = "Software", Priority = "Low", RequestedDate = "2026-02-01", CompletionDate = "2026-02-08", Duration = 7 }
    );
    db.RequestTypes.AddRange(
        new RequestType { Name = "Hardware", Description = "Physical equipment changes", Color = "#0d6efd", Count = 45 },
        new RequestType { Name = "Network", Description = "Network configuration changes", Color = "#198754", Count = 32 },
        new RequestType { Name = "Software", Description = "Software updates and patches", Color = "#ffc107", Count = 28 },
        new RequestType { Name = "Access", Description = "Access permission changes", Color = "#6c757d", Count = 18 },
        new RequestType { Name = "Emergency", Description = "Urgent unplanned changes", Color = "#dc3545", Count = 5 }
    );
    db.WorkOrders.AddRange(
        new WorkOrder { WorkOrderId = "WO-201", ChangeRequest = "CHG-001", Technician = "Mike Chen", Priority = "High", Status = "In Progress", DueDate = "2026-02-25" },
        new WorkOrder { WorkOrderId = "WO-202", ChangeRequest = "CHG-002", Technician = "Sarah Johnson", Priority = "Medium", Status = "Pending", DueDate = "2026-02-28" },
        new WorkOrder { WorkOrderId = "WO-203", ChangeRequest = "CHG-005", Technician = "David Brown", Priority = "Critical", Status = "In Progress", DueDate = "2026-02-22" }
    );
    db.ChangeAudits.AddRange(
        new ChangeAudit { AuditId = "AUD-501", ChangeRequest = "CHG-001", Action = "Created", User = "John Smith", Timestamp = "2026-02-18 09:00", Details = "New hardware change request" },
        new ChangeAudit { AuditId = "AUD-502", ChangeRequest = "CHG-001", Action = "Approved", User = "Sarah Johnson", Timestamp = "2026-02-18 14:30", Details = "Approved by NOC lead" },
        new ChangeAudit { AuditId = "AUD-503", ChangeRequest = "CHG-005", Action = "Created", User = "David Brown", Timestamp = "2026-02-14 08:00", Details = "Emergency change" },
        new ChangeAudit { AuditId = "AUD-504", ChangeRequest = "CHG-005", Action = "Escalated", User = "System", Timestamp = "2026-02-14 08:05", Details = "Auto-escalated: emergency priority" }
    );
    db.ApprovalWorkflows.AddRange(
        new ApprovalWorkflow { WorkflowId = "WF-01", Name = "Standard Change", Type = "Hardware", Priority = "Medium", Steps = "[{\"order\":1,\"role\":\"NOC Lead\",\"approver\":\"Sarah Johnson\",\"status\":\"Approved\"},{\"order\":2,\"role\":\"Manager\",\"approver\":\"Lisa Wang\",\"status\":\"Pending\"}]" },
        new ApprovalWorkflow { WorkflowId = "WF-02", Name = "Emergency Change", Type = "Emergency", Priority = "Critical", Steps = "[{\"order\":1,\"role\":\"On-Call Engineer\",\"approver\":\"Auto-Approved\",\"status\":\"Approved\"}]" }
    );

    // ── Reports seed ──
    db.PueTrends.AddRange(
        new PueTrend { Month = "Oct 2025", PueValue = 1.42, Trend = "↓", ProgressPercent = 85, Status = "On Track" },
        new PueTrend { Month = "Nov 2025", PueValue = 1.40, Trend = "↓", ProgressPercent = 88, Status = "On Track" },
        new PueTrend { Month = "Dec 2025", PueValue = 1.39, Trend = "↓", ProgressPercent = 90, Status = "Good" },
        new PueTrend { Month = "Jan 2026", PueValue = 1.38, Trend = "↓", ProgressPercent = 92, Status = "Good" },
        new PueTrend { Month = "Feb 2026", PueValue = 1.38, Trend = "→", ProgressPercent = 92, Status = "Good" }
    );
    db.CapacityReports.AddRange(
        new CapacityReport { Name = "Power", Color = "#ffc107", CurrentUsage = 76, Available = "720 kW", Total = "3,000 kW", Forecast = "85%", ForecastDays = 90 },
        new CapacityReport { Name = "Cooling", Color = "#0dcaf0", CurrentUsage = 68, Available = "960 kW", Total = "3,000 kW", Forecast = "74%", ForecastDays = 90 },
        new CapacityReport { Name = "Space", Color = "#198754", CurrentUsage = 82, Available = "1,080 U", Total = "6,000 U", Forecast = "90%", ForecastDays = 180 },
        new CapacityReport { Name = "Network", Color = "#0d6efd", CurrentUsage = 55, Available = "225 Gbps", Total = "500 Gbps", Forecast = "62%", ForecastDays = 90 }
    );
    db.ExportCategories.AddRange(
        new ExportCategory { Name = "Asset Inventory", Description = "Full list of all assets with status and location" },
        new ExportCategory { Name = "Power Metrics", Description = "Power consumption and PUE data" },
        new ExportCategory { Name = "Cooling Data", Description = "Temperature and cooling efficiency metrics" },
        new ExportCategory { Name = "Alarm History", Description = "Historical alarm and incident data" }
    );
    db.ExportTemplates.AddRange(
        new ExportTemplate { Name = "Executive Summary", Pages = 4, Description = "High-level overview for management" },
        new ExportTemplate { Name = "Full Audit Report", Pages = 28, Description = "Comprehensive facility audit" },
        new ExportTemplate { Name = "Compliance Report", Pages = 12, Description = "ISO 22237 compliance status" }
    );
    db.ScheduledReports.AddRange(
        new ScheduledReport { Name = "Daily Power Summary", Frequency = "Daily", Recipients = "ops-team@datacenter.local", LastSent = "2026-02-20 06:00", NextRun = "2026-02-21 06:00", Status = "active" },
        new ScheduledReport { Name = "Weekly Capacity", Frequency = "Weekly", Recipients = "management@datacenter.local", LastSent = "2026-02-17 08:00", NextRun = "2026-02-24 08:00", Status = "active" },
        new ScheduledReport { Name = "Monthly Compliance", Frequency = "Monthly", Recipients = "compliance@datacenter.local", LastSent = "2026-02-01 09:00", NextRun = "2026-03-01 09:00", Status = "active" }
    );
    db.ComplianceSections.AddRange(
        new ComplianceSection { SectionId = "ISO-3.1", Name = "Physical Security", Description = "Access control and perimeter security", Items = "[{\"id\":\"3.1.1\",\"name\":\"Perimeter fencing\",\"detail\":\"Complete perimeter fence with CCTV\",\"status\":\"compliant\"},{\"id\":\"3.1.2\",\"name\":\"Access control\",\"detail\":\"Multi-factor authentication at all entry points\",\"status\":\"compliant\"}]" },
        new ComplianceSection { SectionId = "ISO-4.2", Name = "Power Infrastructure", Description = "Power redundancy and UPS requirements", Items = "[{\"id\":\"4.2.1\",\"name\":\"UPS Systems\",\"detail\":\"N+1 UPS redundancy\",\"status\":\"compliant\"},{\"id\":\"4.2.2\",\"name\":\"Generator backup\",\"detail\":\"Diesel generators with 72h fuel\",\"status\":\"partial\"}]" }
    );
    db.ReportMetrics.AddRange(
        new ReportMetric { Name = "Total Power", Value = "2,847 kW", Unit = "kW", Color = "#ffc107", Category = "concise" },
        new ReportMetric { Name = "PUE", Value = "1.38", Unit = "", Color = "#198754", Category = "concise" },
        new ReportMetric { Name = "Avg Temp", Value = "23.2°C", Unit = "°C", Color = "#0dcaf0", Category = "concise" },
        new ReportMetric { Name = "Uptime", Value = "99.99%", Unit = "%", Color = "#198754", Category = "concise" },
        new ReportMetric { Name = "Total Power", Value = "2,847", Unit = "kW", Color = "#ffc107", Category = "detailed" },
        new ReportMetric { Name = "Peak Power", Value = "2,920", Unit = "kW", Color = "#dc3545", Category = "detailed" }
    );
    db.HeatmapDatas.AddRange(
        new HeatmapData { Category = "power", RowLabel = "Hall A", ColLabel = "00:00", Value = 1240, DisplayValue = "1240 kW" },
        new HeatmapData { Category = "power", RowLabel = "Hall A", ColLabel = "12:00", Value = 1380, DisplayValue = "1380 kW" },
        new HeatmapData { Category = "power", RowLabel = "Hall B", ColLabel = "00:00", Value = 960, DisplayValue = "960 kW" },
        new HeatmapData { Category = "power", RowLabel = "Hall B", ColLabel = "12:00", Value = 1050, DisplayValue = "1050 kW" },
        new HeatmapData { Category = "temperature", RowLabel = "CAB-A01", ColLabel = "00:00", Value = 21.5, DisplayValue = "21.5°C" },
        new HeatmapData { Category = "temperature", RowLabel = "CAB-A01", ColLabel = "12:00", Value = 24.8, DisplayValue = "24.8°C" }
    );

    // ── Settings seed ──
    db.RbacRoles.AddRange(
        new RbacRole { Name = "Admin", UserCount = 3, Dashboard = true, Power = true, Cooling = true, Space = true, Security = true },
        new RbacRole { Name = "Operator", UserCount = 8, Dashboard = true, Power = true, Cooling = true, Space = true, Security = false },
        new RbacRole { Name = "Engineer", UserCount = 5, Dashboard = true, Power = true, Cooling = true, Space = false, Security = false },
        new RbacRole { Name = "Viewer", UserCount = 12, Dashboard = true, Power = false, Cooling = false, Space = false, Security = false }
    );
    db.SystemSettings.AddRange(
        new SystemSetting { Key = "timezone", Value = "UTC", Category = "general" },
        new SystemSetting { Key = "tempUnit", Value = "Celsius", Category = "general" },
        new SystemSetting { Key = "powerUnit", Value = "kW", Category = "general" },
        new SystemSetting { Key = "metricsRetention", Value = "365", Category = "retention" },
        new SystemSetting { Key = "logsRetention", Value = "90", Category = "retention" },
        new SystemSetting { Key = "sessionTimeout", Value = "30", Category = "security" },
        new SystemSetting { Key = "mfaRequired", Value = "true", Category = "security" }
    );
    db.SiteConfigs.AddRange(
        new SiteConfig { Name = "DC-East", Location = "New York, NY", Address = "100 Data Center Way", ContactName = "John Smith", ContactPhone = "+1-555-0100", Capacity = 3000, Status = "operational" },
        new SiteConfig { Name = "DC-West", Location = "San Jose, CA", Address = "200 Server Lane", ContactName = "Sarah Johnson", ContactPhone = "+1-555-0200", Capacity = 2000, Status = "operational" }
    );
    db.Integrations.AddRange(
        new Integration { Name = "ServiceNow", Type = "ITSM", Endpoint = "https://instance.service-now.com", Status = "connected", LastSync = "2026-02-20 14:00", Metrics = 1250 },
        new Integration { Name = "Splunk", Type = "SIEM", Endpoint = "https://splunk.local:8089", Status = "connected", LastSync = "2026-02-20 14:30", Metrics = 45000 },
        new Integration { Name = "Prometheus", Type = "Monitoring", Endpoint = "https://prometheus.local:9090", Status = "connected", LastSync = "2026-02-20 14:35", Metrics = 82000 }
    );
    db.ApiKeys.AddRange(
        new ApiKey { Name = "Monitoring Service", KeyId = "ak_mon_28f4e", Created = "2026-01-15", LastUsed = "2026-02-20 14:35", Permissions = "read:metrics,read:alarms", Active = true },
        new ApiKey { Name = "CI/CD Pipeline", KeyId = "ak_cicd_9a1b2", Created = "2025-11-01", LastUsed = "2026-02-20 12:00", Permissions = "read:assets,write:changes", Active = true },
        new ApiKey { Name = "Legacy System", KeyId = "ak_leg_4c3d1", Created = "2024-06-15", LastUsed = "2025-12-01", Permissions = "read:all", Active = false }
    );
    db.WebhookConfigs.AddRange(
        new WebhookConfig { Name = "Slack Alerts", Url = "https://hooks.slack.com/services/xxx", Events = "alarm.critical,alarm.high", Status = "active", LastEvent = "2026-02-20 14:32", DeliveryRate = 99.8 },
        new WebhookConfig { Name = "PagerDuty", Url = "https://events.pagerduty.com/v2/enqueue", Events = "alarm.critical", Status = "active", LastEvent = "2026-02-20 14:32", DeliveryRate = 100.0 },
        new WebhookConfig { Name = "Custom Dashboard", Url = "https://dashboard.local/webhook", Events = "metrics.update", Status = "inactive", LastEvent = "2026-02-15 08:00", DeliveryRate = 95.2 }
    );
    db.SettingsAudits.AddRange(
        new SettingsAudit { Timestamp = "2026-02-20 14:30", User = "john.admin", EventType = "config_change", Action = "Updated", Resource = "CORS Settings", Result = "success", IpAddress = "10.0.0.15" },
        new SettingsAudit { Timestamp = "2026-02-20 10:15", User = "sarah.ops", EventType = "user_management", Action = "Created", Resource = "API Key", Result = "success", IpAddress = "10.0.0.22" },
        new SettingsAudit { Timestamp = "2026-02-19 16:45", User = "mike.eng", EventType = "config_change", Action = "Updated", Resource = "Alert Threshold", Result = "success", IpAddress = "10.0.0.18" }
    );
    db.LanguageConfigs.AddRange(
        new LanguageConfig { Name = "English", Code = "en", Native = "English", Translated = 2400, Total = 2400, Completion = 100, LastUpdated = "2026-02-20" },
        new LanguageConfig { Name = "German", Code = "de", Native = "Deutsch", Translated = 2280, Total = 2400, Completion = 95, LastUpdated = "2026-02-18" },
        new LanguageConfig { Name = "Japanese", Code = "ja", Native = "日本語", Translated = 2160, Total = 2400, Completion = 90, LastUpdated = "2026-02-15" },
        new LanguageConfig { Name = "Spanish", Code = "es", Native = "Español", Translated = 2040, Total = 2400, Completion = 85, LastUpdated = "2026-02-10" }
    );

    // ── Visualization seed ──
    db.VisRacks.AddRange(
        new VisRack { RackId = "A-01", Hall = "Hall A", Status = "active", Utilization = 90.5 },
        new VisRack { RackId = "A-02", Hall = "Hall A", Status = "active", Utilization = 83.3 },
        new VisRack { RackId = "B-01", Hall = "Hall B", Status = "active", Utilization = 95.2 },
        new VisRack { RackId = "B-02", Hall = "Hall B", Status = "warning", Utilization = 66.7 },
        new VisRack { RackId = "C-01", Hall = "Hall C", Status = "active", Utilization = 52.4 }
    );
    db.NetworkDevices.AddRange(
        new NetworkDevice { Name = "Core-SW-01", Status = "active", Type = "switch", Layer = "core" },
        new NetworkDevice { Name = "Core-SW-02", Status = "active", Type = "switch", Layer = "core" },
        new NetworkDevice { Name = "Agg-SW-A1", Status = "active", Type = "switch", Layer = "aggregation" },
        new NetworkDevice { Name = "Agg-SW-B1", Status = "active", Type = "switch", Layer = "aggregation" },
        new NetworkDevice { Name = "FW-01", Status = "active", Type = "firewall", Layer = "core" }
    );
    db.VisLinks.AddRange(
        new VisLink { Source = "Core-SW-01", Destination = "Agg-SW-A1", Status = "active", Utilization = 72, Speed = "100GbE" },
        new VisLink { Source = "Core-SW-01", Destination = "Agg-SW-B1", Status = "active", Utilization = 65, Speed = "100GbE" },
        new VisLink { Source = "Core-SW-02", Destination = "Agg-SW-A1", Status = "active", Utilization = 48, Speed = "100GbE" },
        new VisLink { Source = "FW-01", Destination = "Core-SW-01", Status = "active", Utilization = 55, Speed = "40GbE" }
    );
    db.Widgets.AddRange(
        new Widget { Severity = "critical", Message = "Temperature threshold exceeded", Time = "2026-02-20 14:32" },
        new Widget { Severity = "warning", Message = "UPS battery degraded", Time = "2026-02-20 12:15" },
        new Widget { Severity = "info", Message = "Maintenance window in 2 hours", Time = "2026-02-20 10:00" }
    );
    db.WeatherLocations.AddRange(
        new WeatherLocation { Name = "DC-East", Region = "New York", Temp = 4.2, FeelsLike = 1.8, Condition = "Cloudy", WindSpeed = 18.5, Humidity = 65, Pressure = 1013.2, Aqi = 42, Impact = "Low", LastUpdated = "2026-02-20 14:00" },
        new WeatherLocation { Name = "DC-West", Region = "San Jose", Temp = 18.5, FeelsLike = 17.2, Condition = "Sunny", WindSpeed = 8.2, Humidity = 45, Pressure = 1018.5, Aqi = 28, Impact = "None", LastUpdated = "2026-02-20 14:00" },
        new WeatherLocation { Name = "DC-EU", Region = "Amsterdam", Temp = 8.1, FeelsLike = 5.4, Condition = "Rain", WindSpeed = 24.1, Humidity = 82, Pressure = 1005.8, Aqi = 35, Impact = "Medium", LastUpdated = "2026-02-20 14:00" }
    );

    // ── Environmental (sustainability) seed ──
    db.SustainabilityMetrics.AddRange(
        new SustainabilityMetric { Category = "sustainability", Name = "Renewable Energy %", Value = "42%", Unit = "%", Target = "50%", Status = "On Track", Page = "sustainability" },
        new SustainabilityMetric { Category = "sustainability", Name = "Water Recycled", Value = "78%", Unit = "%", Target = "80%", Status = "On Track", Page = "sustainability" },
        new SustainabilityMetric { Category = "renewables", Name = "Solar Output", Value = "120 kW", Unit = "kW", Target = "150 kW", Status = "Below Target", Page = "renewables" },
        new SustainabilityMetric { Category = "renewables", Name = "Wind Output", Value = "85 kW", Unit = "kW", Target = "100 kW", Status = "Below Target", Page = "renewables" },
        new SustainabilityMetric { Category = "carbon-offset", Name = "Carbon Credits", Value = "45", Unit = "tonnes", Target = "60", Status = "In Progress", Page = "carbon-offset" },
        new SustainabilityMetric { Category = "carbon-offset", Name = "Tree Planting", Value = "2,400", Unit = "trees", Target = "5,000", Status = "In Progress", Page = "carbon-offset" },
        new SustainabilityMetric { Category = "ere", Name = "ERE", Value = "1.12", Unit = "ratio", Target = "1.10", Status = "Near Target", Page = "ere" },
        new SustainabilityMetric { Category = "emissions", Name = "Scope 1", Value = "45", Unit = "tonnes", Target = "40", Status = "Above Target", Page = "emissions" },
        new SustainabilityMetric { Category = "emissions", Name = "Scope 2", Value = "82", Unit = "tonnes", Target = "75", Status = "Above Target", Page = "emissions" },
        new SustainabilityMetric { Category = "emissions", Name = "Scope 3", Value = "15", Unit = "tonnes", Target = "20", Status = "Good", Page = "emissions" },
        new SustainabilityMetric { Category = "energy-storage", Name = "Battery Capacity", Value = "500 kWh", Unit = "kWh", Target = "500 kWh", Status = "At Target", Page = "energy-storage" },
        new SustainabilityMetric { Category = "energy-storage", Name = "Stored Energy", Value = "380 kWh", Unit = "kWh", Target = "400 kWh", Status = "Near Target", Page = "energy-storage" },
        new SustainabilityMetric { Category = "dcoi", Name = "DCOI Score", Value = "87%", Unit = "%", Target = "90%", Status = "Near Target", Page = "dcoi" },
        new SustainabilityMetric { Category = "dcoi", Name = "Virtualization", Value = "78%", Unit = "%", Target = "80%", Status = "Near Target", Page = "dcoi" },
        new SustainabilityMetric { Category = "esg", Name = "ESG Score", Value = "B+", Unit = "grade", Target = "A", Status = "Improving", Page = "esg" },
        new SustainabilityMetric { Category = "esg", Name = "Social Score", Value = "82%", Unit = "%", Target = "85%", Status = "Near Target", Page = "esg" }
    );

    await db.SaveChangesAsync();

    return Results.Ok(new { seeded = true });
});

// SPA fallback: serve index.html for non-API routes
app.MapFallbackToFile("index.html");

app.Run();
