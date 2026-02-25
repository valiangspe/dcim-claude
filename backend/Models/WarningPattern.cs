namespace backend.Models;
public class WarningPattern
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Frequency { get; set; }
    public required string LastOccurrence { get; set; }
    public required string Trend { get; set; }
}
