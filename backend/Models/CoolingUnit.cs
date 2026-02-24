namespace backend.Models;

public class CoolingUnit
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public double Capacity { get; set; }
    public double Load { get; set; }
    public double SupplyTemp { get; set; }
    public double ReturnTemp { get; set; }
    public int FanSpeed { get; set; }
    public required string Status { get; set; }
}
