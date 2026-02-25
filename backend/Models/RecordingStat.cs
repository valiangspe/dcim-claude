namespace backend.Models;
public class RecordingStat
{
    public int Id { get; set; }
    public required string Location { get; set; }
    public int Cameras { get; set; }
    public required string TotalStorage { get; set; }
    public required string UsedStorage { get; set; }
    public required string Retention { get; set; }
}
