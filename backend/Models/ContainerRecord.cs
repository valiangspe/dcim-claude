namespace backend.Models;
public class ContainerRecord
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Ip { get; set; }
    public int Pods { get; set; }
    public double Cpu { get; set; }
    public double Mem { get; set; }
    public required string Network { get; set; }
    public required string Status { get; set; }
}
