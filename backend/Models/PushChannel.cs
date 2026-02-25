namespace backend.Models;
public class PushChannel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Endpoint { get; set; }
    public required string AlertTypes { get; set; }
    public bool Active { get; set; }
}
