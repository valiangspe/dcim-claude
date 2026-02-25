namespace backend.Models;
public class ExportTemplate
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Pages { get; set; }
    public required string Description { get; set; }
}
