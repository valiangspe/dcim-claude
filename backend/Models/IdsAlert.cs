namespace backend.Models;
public class IdsAlert
{
    public int Id { get; set; }
    public required string Timestamp { get; set; }
    public required string Alert { get; set; }
    public required string Protocol { get; set; }
    public double Confidence { get; set; }
    public required string Status { get; set; }
}
