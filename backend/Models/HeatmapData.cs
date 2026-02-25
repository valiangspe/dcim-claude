namespace backend.Models;
public class HeatmapData
{
    public int Id { get; set; }
    public required string Category { get; set; }
    public required string RowLabel { get; set; }
    public required string ColLabel { get; set; }
    public double Value { get; set; }
    public required string DisplayValue { get; set; }
}
