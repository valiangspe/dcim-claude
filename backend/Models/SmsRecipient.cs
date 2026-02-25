namespace backend.Models;
public class SmsRecipient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Severity { get; set; }
    public bool Enabled { get; set; }
}
