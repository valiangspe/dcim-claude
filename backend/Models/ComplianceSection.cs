namespace backend.Models;
public class ComplianceSection
{
    public int Id { get; set; }
    public required string SectionId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Items { get; set; }
}
