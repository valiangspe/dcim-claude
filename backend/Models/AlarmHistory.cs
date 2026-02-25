namespace backend.Models;
public class AlarmHistory
{
    public int Id { get; set; }
    public required string Triggered { get; set; }
    public required string Resolved { get; set; }
    public required string Source { get; set; }
    public required string Severity { get; set; }
    public required string Message { get; set; }
}
