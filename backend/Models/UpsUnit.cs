namespace backend.Models;
public class UpsUnit
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Capacity { get; set; }
    public double Load { get; set; }
    public double LoadPct { get; set; }
    public double Input { get; set; }
    public double Output { get; set; }
    public double Battery { get; set; }
    public double Runtime { get; set; }
    public required string Mode { get; set; }
    public required string Status { get; set; }
}
