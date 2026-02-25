namespace backend.Models;
public class SupplyReturnUnit
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double SupplyTemp { get; set; }
    public double ReturnTemp { get; set; }
    public double SupplyFlow { get; set; }
    public double ReturnFlow { get; set; }
    public double DeltaT { get; set; }
    public required string Status { get; set; }
}
