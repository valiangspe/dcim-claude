namespace backend.Models;
public class Pdu
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Chain { get; set; }
    public double Input { get; set; }
    public double Output { get; set; }
    public double Load { get; set; }
    public double Capacity { get; set; }
    public int Breakers { get; set; }
}
