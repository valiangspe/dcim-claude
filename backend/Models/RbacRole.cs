namespace backend.Models;
public class RbacRole
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int UserCount { get; set; }
    public bool Dashboard { get; set; }
    public bool Power { get; set; }
    public bool Cooling { get; set; }
    public bool Space { get; set; }
    public bool Security { get; set; }
}
