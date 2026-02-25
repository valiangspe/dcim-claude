namespace backend.Models;
public class BreakerRecord
{
    public int Id { get; set; }
    public required string BreakerId { get; set; }
    public required string Pdu { get; set; }
    public double Rating { get; set; }
    public double Load { get; set; }
    public int Circuits { get; set; }
    public bool Trip { get; set; }
}
