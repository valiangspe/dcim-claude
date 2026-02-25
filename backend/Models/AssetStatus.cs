namespace backend.Models;
public class AssetStatus
{
    public int Id { get; set; }
    public required string Status { get; set; }
    public int Count { get; set; }
    public double Pct { get; set; }
    public required string Color { get; set; }
}
