namespace backend.Models;
public class PowerPhase
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Frequency { get; set; }
    public double Thd { get; set; }
    public double Pf { get; set; }
}
