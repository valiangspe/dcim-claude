namespace backend.Models;
public class SustainabilityMetric
{
    public int Id { get; set; }
    public required string Category { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
    public required string Unit { get; set; }
    public required string Target { get; set; }
    public required string Status { get; set; }
    public required string Page { get; set; }
}
