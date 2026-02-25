namespace backend.Models;
public class Rack
{
    public int Id { get; set; }
    public required string RackId { get; set; }
    public required string Hall { get; set; }
    public int TotalU { get; set; }
    public int UsedU { get; set; }
    public double Power { get; set; }
    public double Weight { get; set; }
}
