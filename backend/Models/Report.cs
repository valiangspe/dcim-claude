namespace backend.Models;

public class Report
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Color { get; set; }
    public required string LastRun { get; set; }
}
