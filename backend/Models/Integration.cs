namespace backend.Models;
public class Integration
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Endpoint { get; set; }
    public required string Status { get; set; }
    public required string LastSync { get; set; }
    public int Metrics { get; set; }
}
