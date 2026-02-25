namespace backend.Models;
public class VisRack
{
    public int Id { get; set; }
    public required string RackId { get; set; }
    public required string Hall { get; set; }
    public required string Status { get; set; }
    public double Utilization { get; set; }
}
