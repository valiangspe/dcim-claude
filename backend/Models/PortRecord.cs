namespace backend.Models;
public class PortRecord
{
    public int Id { get; set; }
    public required string PortId { get; set; }
    public required string Speed { get; set; }
    public int Vlan { get; set; }
    public required string Color { get; set; }
    public required string Connected { get; set; }
    public required string Status { get; set; }
}
