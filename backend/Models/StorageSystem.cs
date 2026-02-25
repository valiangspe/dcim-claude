namespace backend.Models;
public class StorageSystem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public double TotalTB { get; set; }
    public double UsedTB { get; set; }
    public int Iops { get; set; }
    public required string Throughput { get; set; }
    public required string Status { get; set; }
}
