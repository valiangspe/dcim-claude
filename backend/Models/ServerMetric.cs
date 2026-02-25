namespace backend.Models;
public class ServerMetric
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Cpu { get; set; }
    public double Gpu { get; set; }
    public double Mem { get; set; }
    public double Disk { get; set; }
    public double NetIn { get; set; }
    public double NetOut { get; set; }
    public double Power { get; set; }
}
