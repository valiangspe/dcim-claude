namespace backend.Models;
public class CoolingMetric
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Value { get; set; }
    public required string Unit { get; set; }
    public double Target { get; set; }
    public required string Description { get; set; }
}
