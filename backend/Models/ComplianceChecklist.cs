namespace backend.Models;

public class ComplianceChecklist
{
    public int Id { get; set; }
    public required string Category { get; set; }
    public required string Requirement { get; set; }
    public required string IsoReference { get; set; }
    public string Description { get; set; } = "";
    public required string Status { get; set; }
    public string AssignedTo { get; set; } = "";
    public string DueDate { get; set; } = "";
    public string Evidence { get; set; } = "";
    public string Notes { get; set; } = "";
}
