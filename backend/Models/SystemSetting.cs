namespace backend.Models;
public class SystemSetting
{
    public int Id { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public required string Category { get; set; }
}
