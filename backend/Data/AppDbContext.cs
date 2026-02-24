using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
}
