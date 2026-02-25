namespace backend.Models;
public class RackDevice
{
    public int Id { get; set; }
    public required string RackId { get; set; }
    public required string Name { get; set; }
    public int StartU { get; set; }
    public int SizeU { get; set; }
    public required string Type { get; set; }
    public required string Color { get; set; }
}
