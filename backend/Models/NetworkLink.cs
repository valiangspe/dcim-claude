namespace backend.Models;

public class NetworkLink
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Capacity { get; set; }
    public double Usage { get; set; }
    public double Latency { get; set; }
    public double Loss { get; set; }
    public required string Status { get; set; }
}
