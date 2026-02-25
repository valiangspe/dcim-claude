namespace backend.Models;
public class RequestType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Color { get; set; }
    public int Count { get; set; }
}
