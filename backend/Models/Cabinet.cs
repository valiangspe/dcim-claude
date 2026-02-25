namespace backend.Models;
public class Cabinet
{
    public int Id { get; set; }
    public required string CabinetId { get; set; }
    public required string Location { get; set; }
    public double Power { get; set; }
    public double Capacity { get; set; }
    public required string Phase { get; set; }
}
