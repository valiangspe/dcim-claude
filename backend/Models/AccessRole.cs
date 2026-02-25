namespace backend.Models;
public class AccessRole
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int PermissionCount { get; set; }
    public int UserCount { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }
}
