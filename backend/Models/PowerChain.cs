namespace backend.Models;
public class PowerChain
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Source { get; set; }
    public double Capacity { get; set; }
    public double Load { get; set; }
    public required string Redundancy { get; set; }
    public required string Status { get; set; }
}
