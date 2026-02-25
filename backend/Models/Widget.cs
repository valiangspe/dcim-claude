namespace backend.Models;
public class Widget
{
    public int Id { get; set; }
    public required string Severity { get; set; }
    public required string Message { get; set; }
    public required string Time { get; set; }
}
