namespace backend.Models;
public class BlockedIp
{
    public int Id { get; set; }
    public required string Ip { get; set; }
    public required string Reason { get; set; }
    public int Incidents { get; set; }
    public required string FirstSeen { get; set; }
    public required string Status { get; set; }
}
