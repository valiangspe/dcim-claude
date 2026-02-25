namespace backend.Models;
public class CapacityReport
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
    public double CurrentUsage { get; set; }
    public required string Available { get; set; }
    public required string Total { get; set; }
    public required string Forecast { get; set; }
    public int ForecastDays { get; set; }
}
