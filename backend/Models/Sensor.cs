namespace backend.Models;

public class Sensor
{
    public int Id { get; set; }
    public required string SensorId { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Location { get; set; }
    public required string Status { get; set; }
    public required string MinThreshold { get; set; }
    public required string MaxThreshold { get; set; }
    public required string PollingInterval { get; set; }
}
