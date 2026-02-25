namespace backend.Models;
public class WorkOrder
{
    public int Id { get; set; }
    public required string WorkOrderId { get; set; }
    public required string ChangeRequest { get; set; }
    public required string Technician { get; set; }
    public required string Priority { get; set; }
    public required string Status { get; set; }
    public required string DueDate { get; set; }
}
