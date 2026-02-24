namespace backend.Models;

public class PowerFeed
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Capacity { get; set; }
    public double Load { get; set; }
    public required string Status { get; set; }
}
