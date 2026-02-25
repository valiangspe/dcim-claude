namespace backend.Models;
public class HealthScoreCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Score { get; set; }
    public required string Color { get; set; }
}
