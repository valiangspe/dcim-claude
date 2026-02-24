namespace backend.Models;

public class EnvironmentalMetric
{
    public int Id { get; set; }
    public required string Category { get; set; }
    public required string Name { get; set; }
    public double Value { get; set; }
    public required string Unit { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
