namespace backend.Models;
public class ApprovalWorkflow
{
    public int Id { get; set; }
    public required string WorkflowId { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Priority { get; set; }
    public required string Steps { get; set; }
}
