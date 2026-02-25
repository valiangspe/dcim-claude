namespace backend.Models;
public class CabinetHumidity
{
    public int Id { get; set; }
    public required string CabinetId { get; set; }
    public required string Location { get; set; }
    public double Rh { get; set; }
    public double Dewpoint { get; set; }
    public bool Alarm { get; set; }
}
