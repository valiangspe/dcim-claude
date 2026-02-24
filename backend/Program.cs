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

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
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

// ── CRUD endpoints ──
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

// ── Seed endpoint ──
app.MapPost("/api/seed", async (AppDbContext db) =>
{
    // Clear all tables
    db.Sensors.RemoveRange(db.Sensors);
    db.EnvironmentalMetrics.RemoveRange(db.EnvironmentalMetrics);
    db.SecurityIncidents.RemoveRange(db.SecurityIncidents);
    db.Reports.RemoveRange(db.Reports);
    db.Users.RemoveRange(db.Users);
    db.ChangeRequests.RemoveRange(db.ChangeRequests);
    db.Alarms.RemoveRange(db.Alarms);
    db.Assets.RemoveRange(db.Assets);
    db.NetworkLinks.RemoveRange(db.NetworkLinks);
    db.Halls.RemoveRange(db.Halls);
    db.CoolingUnits.RemoveRange(db.CoolingUnits);
    db.PowerFeeds.RemoveRange(db.PowerFeeds);
    db.Items.RemoveRange(db.Items);
    await db.SaveChangesAsync();

    // Power Feeds
    db.PowerFeeds.AddRange(
        new PowerFeed { Name = "Utility Feed A", Capacity = 2100, Load = 1480, Status = "active" },
        new PowerFeed { Name = "Utility Feed B", Capacity = 2100, Load = 1367, Status = "active" },
        new PowerFeed { Name = "Generator G1", Capacity = 2000, Load = 0, Status = "standby" },
        new PowerFeed { Name = "Generator G2", Capacity = 2000, Load = 0, Status = "standby" }
    );

    // Cooling Units
    db.CoolingUnits.AddRange(
        new CoolingUnit { Name = "CRAH-A1", Type = "CRAH", Capacity = 600, Load = 482, SupplyTemp = 13.8, ReturnTemp = 28.2, FanSpeed = 78, Status = "running" },
        new CoolingUnit { Name = "CRAH-A2", Type = "CRAH", Capacity = 600, Load = 510, SupplyTemp = 14.1, ReturnTemp = 29.0, FanSpeed = 82, Status = "running" },
        new CoolingUnit { Name = "CRAH-B1", Type = "CRAH", Capacity = 600, Load = 445, SupplyTemp = 14.5, ReturnTemp = 28.4, FanSpeed = 72, Status = "running" },
        new CoolingUnit { Name = "CRAH-B2", Type = "CRAH", Capacity = 600, Load = 498, SupplyTemp = 14.0, ReturnTemp = 28.8, FanSpeed = 80, Status = "running" },
        new CoolingUnit { Name = "CRAC-C1", Type = "CRAC", Capacity = 400, Load = 312, SupplyTemp = 14.6, ReturnTemp = 27.9, FanSpeed = 68, Status = "running" },
        new CoolingUnit { Name = "CRAC-C2", Type = "CRAC", Capacity = 400, Load = 185, SupplyTemp = 14.3, ReturnTemp = 27.5, FanSpeed = 55, Status = "running" },
        new CoolingUnit { Name = "CRAH-D1", Type = "CRAH", Capacity = 600, Load = 0, SupplyTemp = 0, ReturnTemp = 0, FanSpeed = 0, Status = "standby" }
    );

    // Halls
    db.Halls.AddRange(
        new Hall { Name = "Hall A", Sqft = 16000, Racks = 72, Occupied = 68, TotalU = 3024, UsedU = 2480, Power = 1240 },
        new Hall { Name = "Hall B", Sqft = 14000, Racks = 62, Occupied = 58, TotalU = 2604, UsedU = 2120, Power = 980 },
        new Hall { Name = "Hall C", Sqft = 8000, Racks = 32, Occupied = 24, TotalU = 1344, UsedU = 820, Power = 412 },
        new Hall { Name = "Hall D", Sqft = 4000, Racks = 20, Occupied = 14, TotalU = 1308, UsedU = 472, Power = 215 }
    );

    // Network Links
    db.NetworkLinks.AddRange(
        new NetworkLink { Name = "ISP-A (Primary)", Capacity = 200, Usage = 142, Latency = 1.2, Loss = 0.01, Status = "active" },
        new NetworkLink { Name = "ISP-B (Secondary)", Capacity = 200, Usage = 105, Latency = 1.8, Loss = 0.02, Status = "active" },
        new NetworkLink { Name = "DC-East ↔ DC-West", Capacity = 100, Usage = 38, Latency = 18.4, Loss = 0.00, Status = "active" },
        new NetworkLink { Name = "DC-East ↔ DC-EU", Capacity = 50, Usage = 22, Latency = 82.1, Loss = 0.03, Status = "active" }
    );

    // Assets
    db.Assets.AddRange(
        new Asset { Name = "Servers", Class = "Compute", Installed = 124, Planned = 12, Archived = 8 },
        new Asset { Name = "Network Switches", Class = "Network", Installed = 32, Planned = 4, Archived = 2 },
        new Asset { Name = "Storage Arrays", Class = "Storage", Installed = 8, Planned = 2, Archived = 1 },
        new Asset { Name = "PDUs", Class = "Power", Installed = 24, Planned = 2, Archived = 0 },
        new Asset { Name = "UPS Units", Class = "Power", Installed = 5, Planned = 1, Archived = 0 },
        new Asset { Name = "Patch Panels", Class = "Network", Installed = 18, Planned = 0, Archived = 2 },
        new Asset { Name = "Cooling Units", Class = "Cooling", Installed = 7, Planned = 1, Archived = 0 }
    );

    // Alarms
    db.Alarms.AddRange(
        new Alarm { Source = "Server Rack A-01", Severity = "Critical", Message = "Temperature threshold exceeded: 42°C", Time = DateTime.UtcNow.AddMinutes(-5), Acknowledged = false },
        new Alarm { Source = "PDU-B-03", Severity = "Critical", Message = "Power consumption anomaly detected", Time = DateTime.UtcNow.AddMinutes(-23), Acknowledged = false },
        new Alarm { Source = "Cooling Unit C-02", Severity = "High", Message = "Fan speed degraded — running at 45%", Time = DateTime.UtcNow.AddMinutes(-47), Acknowledged = false },
        new Alarm { Source = "Network Switch D-01", Severity = "High", Message = "Port channel flapping detected", Time = DateTime.UtcNow.AddHours(-2), Acknowledged = false },
        new Alarm { Source = "UPS Battery Bank", Severity = "Medium", Message = "Battery health check: 1 cell below threshold", Time = DateTime.UtcNow.AddHours(-6), Acknowledged = false }
    );

    // Change Requests
    db.ChangeRequests.AddRange(
        new ChangeRequest { RequestId = "CHG-001", Requester = "John Smith", Type = "Hardware", Priority = "High", Status = "Pending Review", RequestedDate = "2026-02-18" },
        new ChangeRequest { RequestId = "CHG-002", Requester = "Sarah Johnson", Type = "Network", Priority = "Medium", Status = "Waiting Approval", RequestedDate = "2026-02-17" },
        new ChangeRequest { RequestId = "CHG-003", Requester = "Mike Chen", Type = "Software", Priority = "Low", Status = "Pending Review", RequestedDate = "2026-02-16" },
        new ChangeRequest { RequestId = "CHG-004", Requester = "Emma Wilson", Type = "Access", Priority = "High", Status = "Waiting Approval", RequestedDate = "2026-02-15" },
        new ChangeRequest { RequestId = "CHG-005", Requester = "David Brown", Type = "Emergency", Priority = "Critical", Status = "In Progress", RequestedDate = "2026-02-14" }
    );

    // Users
    db.Users.AddRange(
        new User { Name = "John Administrator", Email = "john.admin@datacenter.local", Role = "Administrator", LastLogin = "2026-02-20 14:32 UTC", Status = "Active" },
        new User { Name = "Sarah Operations", Email = "sarah.ops@datacenter.local", Role = "Operator", LastLogin = "2026-02-20 08:15 UTC", Status = "Active" },
        new User { Name = "Mike Engineer", Email = "mike.eng@datacenter.local", Role = "Engineer", LastLogin = "2026-02-19 23:45 UTC", Status = "Active" },
        new User { Name = "Lisa Viewer", Email = "lisa.view@datacenter.local", Role = "Viewer", LastLogin = "2026-02-15 10:22 UTC", Status = "Active" },
        new User { Name = "Robert Inactive", Email = "robert.inactive@datacenter.local", Role = "Operator", LastLogin = "2025-12-01 16:00 UTC", Status = "Inactive" }
    );

    // Reports
    db.Reports.AddRange(
        new Report { Name = "Power Consumption Report", Description = "Monthly power usage, costs, and efficiency metrics", Color = "#ffc107", LastRun = "2026-02-18" },
        new Report { Name = "Cooling Performance Report", Description = "Temperature monitoring and cooling efficiency analysis", Color = "#0dcaf0", LastRun = "2026-02-19" },
        new Report { Name = "Space Utilization Report", Description = "Rack and floor space capacity analysis", Color = "#198754", LastRun = "2026-02-17" },
        new Report { Name = "Equipment Inventory Report", Description = "Complete asset listing and depreciation tracking", Color = "#6c757d", LastRun = "2026-02-15" },
        new Report { Name = "Network Performance Report", Description = "Bandwidth utilization and network health metrics", Color = "#0d6efd", LastRun = "2026-02-16" },
        new Report { Name = "Compliance Report", Description = "ISO 22237 and regulatory compliance summary", Color = "#dc3545", LastRun = "2026-02-14" }
    );

    // Security Incidents
    db.SecurityIncidents.AddRange(
        new SecurityIncident { Type = "Unauthorized Access Attempt", Timestamp = "2026-02-20 14:32", Location = "Data Center A", Severity = "high", Status = "resolved" },
        new SecurityIncident { Type = "Configuration Change", Timestamp = "2026-02-20 13:15", Location = "Security Control Room", Severity = "medium", Status = "investigating" },
        new SecurityIncident { Type = "Access Control Bypass", Timestamp = "2026-02-20 11:42", Location = "Server Room B", Severity = "critical", Status = "active" },
        new SecurityIncident { Type = "Policy Violation", Timestamp = "2026-02-19 16:28", Location = "Network Operations", Severity = "low", Status = "resolved" },
        new SecurityIncident { Type = "Unusual Activity Pattern", Timestamp = "2026-02-19 09:10", Location = "Data Center C", Severity = "medium", Status = "resolved" }
    );

    // Environmental Metrics (monthly CO2 data)
    db.EnvironmentalMetrics.AddRange(
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 13.2, Unit = "tonnes", Month = 1, Year = 2026 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 12.8, Unit = "tonnes", Month = 2, Year = 2026 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 11.5, Unit = "tonnes", Month = 3, Year = 2025 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 10.9, Unit = "tonnes", Month = 4, Year = 2025 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 11.2, Unit = "tonnes", Month = 5, Year = 2025 },
        new EnvironmentalMetric { Category = "carbon", Name = "CO2 Emissions", Value = 10.8, Unit = "tonnes", Month = 6, Year = 2025 }
    );

    // Sensors
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

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        seeded = true,
        counts = new
        {
            powerFeeds = await db.PowerFeeds.CountAsync(),
            coolingUnits = await db.CoolingUnits.CountAsync(),
            halls = await db.Halls.CountAsync(),
            networkLinks = await db.NetworkLinks.CountAsync(),
            assets = await db.Assets.CountAsync(),
            alarms = await db.Alarms.CountAsync(),
            changeRequests = await db.ChangeRequests.CountAsync(),
            users = await db.Users.CountAsync(),
            reports = await db.Reports.CountAsync(),
            securityIncidents = await db.SecurityIncidents.CountAsync(),
            environmentalMetrics = await db.EnvironmentalMetrics.CountAsync(),
            sensors = await db.Sensors.CountAsync()
        }
    });
});

// SPA fallback: serve index.html for non-API routes
app.MapFallbackToFile("index.html");

app.Run();
