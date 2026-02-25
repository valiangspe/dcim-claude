namespace backend.Models;
public class StrandedPower
{
    public int Id { get; set; }
    public required string Location { get; set; }
    public double Allocated { get; set; }
    public double Actual { get; set; }
    public double Stranded { get; set; }
    public double Pct { get; set; }
}
