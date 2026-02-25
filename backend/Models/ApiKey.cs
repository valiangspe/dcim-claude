namespace backend.Models;
public class ApiKey
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string KeyId { get; set; }
    public required string Created { get; set; }
    public required string LastUsed { get; set; }
    public required string Permissions { get; set; }
    public bool Active { get; set; }
}
