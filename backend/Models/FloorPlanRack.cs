namespace backend.Models;
public class FloorPlanRack
{
    public int Id { get; set; }
    public required string RackId { get; set; }
    public required string RowLabel { get; set; }
    public int Position { get; set; }
    public double Utilization { get; set; }
}
