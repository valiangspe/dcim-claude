namespace backend.Models;

public class Asset
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Class { get; set; }
    public int Installed { get; set; }
    public int Planned { get; set; }
    public int Archived { get; set; }
}
