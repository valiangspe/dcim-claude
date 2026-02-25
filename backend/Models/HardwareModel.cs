namespace backend.Models;
public class HardwareModel
{
    public int Id { get; set; }
    public required string Model { get; set; }
    public required string Vendor { get; set; }
    public required string Category { get; set; }
    public int Count { get; set; }
    public required string AvgAge { get; set; }
}
