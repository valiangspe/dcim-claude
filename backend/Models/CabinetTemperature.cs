namespace backend.Models;
public class CabinetTemperature
{
    public int Id { get; set; }
    public required string CabinetId { get; set; }
    public required string Location { get; set; }
    public double Inlet { get; set; }
    public double Outlet { get; set; }
    public double Delta { get; set; }
    public bool Alarm { get; set; }
}
