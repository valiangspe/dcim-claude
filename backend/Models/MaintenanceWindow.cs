namespace backend.Models;
public class MaintenanceWindow
{
    public int Id { get; set; }
    public required string Component { get; set; }
    public required string StartTime { get; set; }
    public required string EndTime { get; set; }
    public required string Impact { get; set; }
    public required string Status { get; set; }
}
