namespace backend.Models;
public class CabinetGroup
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Racks { get; set; }
    public int TotalU { get; set; }
    public int UsedU { get; set; }
    public int Remaining { get; set; }
}
