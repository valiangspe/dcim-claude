namespace backend.Models;

public class DcIncident
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }
    public required string Severity { get; set; }
    public required string Status { get; set; }
    public required string AffectedSystem { get; set; }
    public required string Site { get; set; }
    public string DetectedAt { get; set; } = "";
    public string ResolvedAt { get; set; } = "";
    public string ReportedBy { get; set; } = "";
    public string AssignedTo { get; set; } = "";
    public string RootCause { get; set; } = "";
    public string Impact { get; set; } = "";
    public string Resolution { get; set; } = "";
    public int DowntimeMinutes { get; set; }
    public string IsoClass { get; set; } = "";
}
