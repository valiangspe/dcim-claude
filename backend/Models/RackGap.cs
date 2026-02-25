namespace backend.Models;
public class RackGap
{
    public int Id { get; set; }
    public required string RackId { get; set; }
    public int TotalU { get; set; }
    public int FreeU { get; set; }
    public int GapStart { get; set; }
    public int GapSize { get; set; }
}
