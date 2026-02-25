namespace backend.Models;
public class PowerBudget
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Budget { get; set; }
    public double Actual { get; set; }
    public double Reserved { get; set; }
}
