namespace backend.Models;
public class Chiller
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public double Capacity { get; set; }
    public double Load { get; set; }
    public double Cop { get; set; }
    public double Eer { get; set; }
    public double SupplyW { get; set; }
    public double ReturnW { get; set; }
    public double Flow { get; set; }
    public required string Status { get; set; }
}
