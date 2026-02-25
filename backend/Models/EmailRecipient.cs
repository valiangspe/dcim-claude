namespace backend.Models;
public class EmailRecipient
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Frequency { get; set; }
    public required string AlertTypes { get; set; }
    public bool Active { get; set; }
}
