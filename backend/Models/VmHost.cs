namespace backend.Models;
public class VmHost
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public bool Physical { get; set; }
    public int Vms { get; set; }
    public int Vcpus { get; set; }
    public int Vmem { get; set; }
}
