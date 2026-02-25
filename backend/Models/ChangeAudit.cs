namespace backend.Models;
public class ChangeAudit
{
    public int Id { get; set; }
    public required string AuditId { get; set; }
    public required string ChangeRequest { get; set; }
    public required string Action { get; set; }
    public required string User { get; set; }
    public required string Timestamp { get; set; }
    public required string Details { get; set; }
}
