namespace backend.Models;
public class EnergyCostRecord
{
    public int Id { get; set; }
    public required string Month { get; set; }
    public double Kwh { get; set; }
    public double Cost { get; set; }
    public double Rate { get; set; }
}
