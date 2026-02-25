namespace backend.Models;
public class NetworkDevice
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Type { get; set; }
    public required string Layer { get; set; }
}
