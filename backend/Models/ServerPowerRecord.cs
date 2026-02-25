namespace backend.Models;
public class ServerPowerRecord
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Rack { get; set; }
    public double Power { get; set; }
    public double Cpu { get; set; }
    public required string Psu { get; set; }
    public required string Status { get; set; }
}
