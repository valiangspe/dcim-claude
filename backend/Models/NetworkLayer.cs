namespace backend.Models;
public class NetworkLayer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Devices { get; set; }
    public required string Connections { get; set; }
}
