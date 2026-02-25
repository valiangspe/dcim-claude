namespace backend.Models;
public class PortCapacityRecord
{
    public int Id { get; set; }
    public required string PduName { get; set; }
    public int Total { get; set; }
    public int Used { get; set; }
    public int Reserved { get; set; }
}
