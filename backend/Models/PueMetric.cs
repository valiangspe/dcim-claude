namespace backend.Models;
public class PueMetric
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Value { get; set; }
    public double Target { get; set; }
    public double StdDev { get; set; }
}
