namespace backend.Models;
public class DataSwitch
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Total { get; set; }
    public int Used { get; set; }
    public required string Speed { get; set; }
    public required string Status { get; set; }
}
