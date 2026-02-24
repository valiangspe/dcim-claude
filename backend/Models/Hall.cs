namespace backend.Models;

public class Hall
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Sqft { get; set; }
    public int Racks { get; set; }
    public int Occupied { get; set; }
    public int TotalU { get; set; }
    public int UsedU { get; set; }
    public double Power { get; set; }
}
