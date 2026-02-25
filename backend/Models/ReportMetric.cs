namespace backend.Models;
public class ReportMetric
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
    public required string Unit { get; set; }
    public required string Color { get; set; }
    public required string Category { get; set; }
}
