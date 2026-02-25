namespace backend.Models;
public class LifecyclePhase
{
    public int Id { get; set; }
    public required string Phase { get; set; }
    public int Count { get; set; }
    public required string Items { get; set; }
    public required string Eta { get; set; }
}
