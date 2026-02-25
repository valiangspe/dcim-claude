namespace backend.Models;
public class NotificationRecord
{
    public int Id { get; set; }
    public required string Type { get; set; }
    public required string Message { get; set; }
    public required string Time { get; set; }
    public bool Read { get; set; }
}
