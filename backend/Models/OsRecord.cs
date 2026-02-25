namespace backend.Models;
public class OsRecord
{
    public int Id { get; set; }
    public required string Os { get; set; }
    public int Count { get; set; }
    public double Pct { get; set; }
}
