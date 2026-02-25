namespace backend.Models;
public class Cable
{
    public int Id { get; set; }
    public required string CableId { get; set; }
    public required string FromEnd { get; set; }
    public required string ToEnd { get; set; }
    public required string Type { get; set; }
    public required string Length { get; set; }
    public required string Color { get; set; }
    public required string Path { get; set; }
}
