namespace backend.Models;
public class CctvCamera
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Status { get; set; }
    public required string Recording { get; set; }
    public required string Resolution { get; set; }
    public required string Storage { get; set; }
    public required string LastFrame { get; set; }
}
