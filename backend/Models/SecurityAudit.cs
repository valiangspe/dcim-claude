namespace backend.Models;
public class SecurityAudit
{
    public int Id { get; set; }
    public required string Timestamp { get; set; }
    public required string User { get; set; }
    public required string Action { get; set; }
    public required string Resource { get; set; }
    public required string Result { get; set; }
    public required string Details { get; set; }
}
