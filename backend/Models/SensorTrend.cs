namespace backend.Models;
public class SensorTrend
{
    public int Id { get; set; }
    public required string SensorId { get; set; }
    public required string Cabinet { get; set; }
    public required string Type { get; set; }
    public required string Time { get; set; }
    public double Value { get; set; }
}
