namespace backend.Models;

public class UptimeRecord
{
    public int Id { get; set; }
    public required string Site { get; set; }
    public required string System { get; set; }
    public int UptimeMinutes { get; set; }
    public int DowntimeMinutes { get; set; }
    public required string PeriodStart { get; set; }
    public required string PeriodEnd { get; set; }
    public double SlaTarget { get; set; }
    public int Incidents { get; set; }
    public string Notes { get; set; } = "";
}
