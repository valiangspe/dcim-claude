namespace backend.Models;
public class PueTrend
{
    public int Id { get; set; }
    public required string Month { get; set; }
    public double PueValue { get; set; }
    public required string Trend { get; set; }
    public double ProgressPercent { get; set; }
    public required string Status { get; set; }
}
