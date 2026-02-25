namespace backend.Models;
public class AccessLog
{
    public int Id { get; set; }
    public required string Timestamp { get; set; }
    public required string Door { get; set; }
    public required string Person { get; set; }
    public required string Method { get; set; }
    public required string Result { get; set; }
}
