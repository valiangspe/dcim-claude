namespace backend.Models;
public class AlertRule
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Condition { get; set; }
    public required string Threshold { get; set; }
    public required string Action { get; set; }
    public bool Enabled { get; set; }
}
