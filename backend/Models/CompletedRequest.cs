namespace backend.Models;
public class CompletedRequest
{
    public int Id { get; set; }
    public required string RequestId { get; set; }
    public required string Requester { get; set; }
    public required string Type { get; set; }
    public required string Priority { get; set; }
    public required string RequestedDate { get; set; }
    public required string CompletionDate { get; set; }
    public int Duration { get; set; }
}
