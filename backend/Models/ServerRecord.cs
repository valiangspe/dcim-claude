namespace backend.Models;
public class ServerRecord
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Rack { get; set; }
    public required string Os { get; set; }
    public required string Uptime { get; set; }
    public required string Status { get; set; }
}
