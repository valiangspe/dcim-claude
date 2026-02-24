namespace backend.Models;

public class SecurityIncident
{
    public int Id { get; set; }
    public required string Type { get; set; }
    public required string Timestamp { get; set; }
    public required string Location { get; set; }
    public required string Severity { get; set; }
    public required string Status { get; set; }
}
