namespace backend.Models;
public class PendingAsset
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Size { get; set; }
    public required string Type { get; set; }
    public required string TargetRack { get; set; }
}
