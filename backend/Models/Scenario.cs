namespace backend.Models;
public class Scenario
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Racks { get; set; }
    public double Power { get; set; }
    public double Cooling { get; set; }
    public double Space { get; set; }
    public required string Feasibility { get; set; }
    public required string Notes { get; set; }
}
