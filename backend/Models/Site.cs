namespace backend.Models;
public class Site
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public required string Status { get; set; }
    public double Power { get; set; }
    public double Pue { get; set; }
    public int Racks { get; set; }
    public int Alarms { get; set; }
}
