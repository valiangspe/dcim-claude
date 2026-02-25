namespace backend.Models;
public class FirewallEvent
{
    public int Id { get; set; }
    public required string Timestamp { get; set; }
    public required string Type { get; set; }
    public required string SourceIP { get; set; }
    public int DestPort { get; set; }
    public required string Action { get; set; }
    public required string Severity { get; set; }
}
