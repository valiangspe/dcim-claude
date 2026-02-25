namespace backend.Models;
public class DcBus
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Power { get; set; }
    public double Temp { get; set; }
}
