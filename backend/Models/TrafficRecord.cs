namespace backend.Models;
public class TrafficRecord
{
    public int Id { get; set; }
    public required string Time { get; set; }
    public double Inbound { get; set; }
    public double Outbound { get; set; }
    public double Total { get; set; }
}
