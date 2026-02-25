namespace backend.Models;
public class Rectifier
{
    public int Id { get; set; }
    public required string RectifierId { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Power { get; set; }
    public double Efficiency { get; set; }
    public required string Status { get; set; }
}
