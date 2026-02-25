namespace backend.Models;
public class LanguageConfig
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Native { get; set; }
    public int Translated { get; set; }
    public int Total { get; set; }
    public double Completion { get; set; }
    public required string LastUpdated { get; set; }
}
