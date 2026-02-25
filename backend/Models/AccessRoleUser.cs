namespace backend.Models;
public class AccessRoleUser
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }
    public required string Department { get; set; }
    public required string LastLogin { get; set; }
    public required string Status { get; set; }
}
