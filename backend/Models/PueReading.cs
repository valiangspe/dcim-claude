namespace backend.Models;
public class PueReading
{
    public int Id { get; set; }
    public required string Date { get; set; }
    public double Pue { get; set; }
    public required string Component { get; set; }
    public double Power { get; set; }
    public double Pct { get; set; }
}
