namespace backend.Models;

public class Alarm
{
    public int Id { get; set; }
    public required string Source { get; set; }
    public required string Severity { get; set; }
    public required string Message { get; set; }
    public DateTime Time { get; set; }
    public bool Acknowledged { get; set; }
}
