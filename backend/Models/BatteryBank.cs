namespace backend.Models;
public class BatteryBank
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public int Cells { get; set; }
    public double Soc { get; set; }
    public double Soh { get; set; }
    public double Voltage { get; set; }
    public double Temp { get; set; }
    public double Runtime { get; set; }
    public required string Status { get; set; }
}
