namespace backend.Models;
public class WaterUsageRecord
{
    public int Id { get; set; }
    public required string Month { get; set; }
    public double WaterL { get; set; }
    public double ItKwh { get; set; }
    public double Wue { get; set; }
}
