namespace backend.Models;
public class ScheduledReport
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Frequency { get; set; }
    public required string Recipients { get; set; }
    public required string LastSent { get; set; }
    public required string NextRun { get; set; }
    public required string Status { get; set; }
}
