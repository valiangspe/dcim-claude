namespace backend.Models;
public class SiteNode
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public int? ParentId { get; set; }
}
