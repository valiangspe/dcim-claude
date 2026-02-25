namespace backend.Models;
public class CustomDashboard
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Widgets { get; set; }
    public required string Modified { get; set; }
    public bool Favorite { get; set; }
}
