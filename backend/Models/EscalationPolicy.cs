namespace backend.Models;
public class EscalationPolicy
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool Active { get; set; }
    public required string Levels { get; set; }
}
