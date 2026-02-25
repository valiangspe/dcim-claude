namespace backend.Models;
public class AssetAttribute
{
    public int Id { get; set; }
    public required string Attribute { get; set; }
    public required string Value { get; set; }
    public int Count { get; set; }
}
