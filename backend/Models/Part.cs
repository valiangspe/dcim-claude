namespace backend.Models;
public class Part
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public int InStock { get; set; }
    public int Reserved { get; set; }
    public int Reorder { get; set; }
    public required string Unit { get; set; }
}
