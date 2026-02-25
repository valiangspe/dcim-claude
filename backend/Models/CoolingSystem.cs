namespace backend.Models;
public class CoolingSystem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Uptime { get; set; }
    public required string LastMaint { get; set; }
    public required string NextMaint { get; set; }
    public required string Downtime { get; set; }
}
