namespace backend.Models;
public class PowerEvent
{
    public int Id { get; set; }
    public required string Time { get; set; }
    public required string Type { get; set; }
    public required string Phase { get; set; }
    public required string Duration { get; set; }
    public required string Magnitude { get; set; }
}
