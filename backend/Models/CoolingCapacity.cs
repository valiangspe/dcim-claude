namespace backend.Models;
public class CoolingCapacity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Current { get; set; }
    public double Max { get; set; }
    public double Planned { get; set; }
    public double Available { get; set; }
    public double Growth { get; set; }
}
