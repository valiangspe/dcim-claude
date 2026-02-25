namespace backend.Models;
public class VisLink
{
    public int Id { get; set; }
    public required string Source { get; set; }
    public required string Destination { get; set; }
    public required string Status { get; set; }
    public double Utilization { get; set; }
    public required string Speed { get; set; }
}
