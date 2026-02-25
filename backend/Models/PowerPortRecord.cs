namespace backend.Models;
public class PowerPortRecord
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Total { get; set; }
    public int Used { get; set; }
    public int Reserved { get; set; }
    public required string Type { get; set; }
}
